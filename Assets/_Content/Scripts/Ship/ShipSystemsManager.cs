using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// Singleton manager for the ship's systems, update routines, etc.
/// </summary>
[Serializable]
public class ShipSystemsManager : MonoBehaviour
{
	#region Singleton
	private static ShipSystemsManager _instance = null;
    /// <summary>
    /// Singleton instance of the game manager.
    /// </summary>
    public static ShipSystemsManager Instance
    { 
        get
        {
            return _instance;
        }
    }
	#endregion Singleton

    #region References
    [SerializeField] private Transform _shipHead;
    #endregion References

    #region Properties
    #region Heat
    /// <summary>
    /// External Temperature (C) - About +30C/1km 
    /// </summary>
    public float ExternalTemperature { get; protected set; }
    /// <summary>
    /// Internal Temperature (C)
    /// </summary>
    public float InternalTemperature { get; protected set; }
    /// <summary>
    /// Total heat into the ship from the environment (kWt)
    /// </summary>
    public float AmbientHeatInflux { get; protected set; }
    /// <summary>
    /// Total heat generated from ship systems (kWt)
    /// </summary>
    public float TotalSystemsHeat { get; protected set; }
	/// <summary>
	/// Total unspent coolant in cooling module (kWh)
	/// </summary>
	public float CoolantRemaining { get; protected set; }
	/// <summary>
	/// Total heat leaving the ship via cooling (kWt)
	/// </summary>
	public float TotalCoolingLoad { get; protected set; }
	/// <summary>
	/// Estimated time until out of coolant (time)
	/// </summary>
	public TimeSpan EstimatedCoolingDuration { get; protected set; }
	#endregion Heat

	#region Power
	/// <summary>
	/// Total unspent fuel in reactor (kWh)
	/// </summary>
	public float FuelRemaining { get; protected set; }
    /// <summary>
    /// Total power drawn from all systems (kWe)
    /// </summary>
    public float TotalPowerDemand { get; protected set; }
    /// <summary>
    /// Total power produced by reactor(s) (kWe)
    /// </summary>
    public float TotalPowerProduction { get; protected set; }
    /// <summary>
    /// Total efficiency of all ship systems (%)
    /// </summary>
    public float OperationalEfficiency { get; protected set; }
    /// <summary>
    /// Estimated time until out of fuel (time)
    /// </summary>
    public TimeSpan EstimatedPowerDuration { get; protected set; }
	#endregion Power

	#region Telemetry
	/// <summary>
	/// Current Fathom zone
	/// </summary>
	public int FathomCount { get; protected set; }
    /// <summary>
    /// Current depth of the ship's nose (m)
    /// </summary>
    public float Depth { get; protected set; }
    /// <summary>
    /// Current maximum allowed speed of ship (m/s)
    /// </summary>
    public float MaximumSpeed { get; protected set; }
    /// <summary>
    /// Current speed of ship (m/s)
    /// </summary>
    public float CurrentSpeed { get; protected set; }
    /// <summary>
    /// Throttle-set speed target (m/s)
    /// </summary>
	public float TargetSpeed { get; protected set; }
	/// <summary>
	/// Directional heading of ship (deg)
	/// </summary>
	public float CurrentHeading { get; protected set; }
    /// <summary>
    /// Target heading of ship (deg)
    /// </summary>
    public float TargetHeading { get; protected set; }
	#endregion Telemetry
	#endregion Properties

	#region Constants
    // Absolute
    private const float _targetInteriorTemperature = 25f; // C
	private const float _ambientTemperatureRate = 30f / 1000f; // C/m
    private const float _mass = 2000f * 1000f; // kg
    private const float _specificHeatCapacity = 300f; // J/kg-K
    private const float _thermalConductivity = 3.5f; // W/m-K
    private const float _surfaceArea = 500f; // m^2
    private const float _hullThickness = 1f; // m
    private const float _blackoutThreshold = 1.25f; // %
    private const int _fathomDepth = 3000; // m
    // Derived
    private const float _internalTemperatureChangeRate = 1000f / (_specificHeatCapacity * _mass); // C/kWt-s
    private const float _ambientHeatRate = _thermalConductivity * _surfaceArea / _hullThickness / 1000f; // kWt/K
	#endregion Constants

	#region Fields
	public StorageModule StorageModule = null;

	// Not shown in inspector
	private List<ShipModule> _shipModules = new List<ShipModule>();
    private ReactorModule _reactorModule = null;
    private CoolingModule _coolingModule = null;
    private EngineModule _engineModule = null;
	#endregion Fields

	#region Game Object Callbacks
	/// <summary>
	/// Registers a new <see cref="ShipModule"/> with this manager.
	/// </summary>
	public void Callback(ShipModule module)
    {
        if (!_shipModules.Contains(module))
        {
			_shipModules.Add(module);
		}
	}
	/// <summary>
	/// Registers the <see cref="ReactorModule"/> with this manager.
	/// </summary>
	public void Callback(ReactorModule module)
	{
        _reactorModule = module;
        Callback((ShipModule)module);
	}
	/// <summary>
	/// Registers the <see cref="CoolingModule"/> with this manager.
	/// </summary>
	public void Callback(CoolingModule module)
	{
		_coolingModule = module;
		Callback((ShipModule)module);
	}
    /// <summary>
    /// Registers the <see cref="global::StorageModule"/> with this manager.
    /// </summary>
    public void Callback(StorageModule module)
	{
        StorageModule = module;
		Callback((ShipModule)module);
	}
	/// <summary>
	/// Registers the <see cref="EngineModule"/> with this manager.
	/// </summary>
	public void Callback(EngineModule module)
	{
		_engineModule = module;
		Callback((ShipModule)module);
	}
	#endregion Game Object Callbacks

	#region Update Logic
	private void UpdatePower()
    {
        _reactorModule.TargetPowerProduction = TotalPowerDemand = _shipModules.Select(mod => mod.PowerDemand).Sum();
		FuelRemaining = _reactorModule.FuelRemaining;
		TotalPowerProduction = _reactorModule.PowerProduction;
		EstimatedPowerDuration = _reactorModule.EstimatedTimeRemaining;

        // Check to see if we are over the power limit.
        if (TotalPowerDemand > 0f && TotalPowerProduction < TotalPowerDemand)
        {
            float utilization = TotalPowerProduction / TotalPowerDemand;
            if (utilization < 1f)
            {
                OperationalEfficiency = 1f;
            }
            else if (utilization > _blackoutThreshold)
            {
                // Ship is going into blackout!
                OperationalEfficiency = 0f;
			}
            else
            {
                // Ship is over-stressed and module efficiency will be reduced.
                OperationalEfficiency = 2f - utilization;
            }

            _shipModules.ForEach(mod => mod.OperationalEfficiency = OperationalEfficiency);
        }
    }

    private void UpdateHeat()
    {
		ExternalTemperature = Depth * _ambientTemperatureRate + _targetInteriorTemperature;
		AmbientHeatInflux = (ExternalTemperature - InternalTemperature) * _ambientHeatRate;
		TotalSystemsHeat = _shipModules.Select(mod => mod.HeatGeneration).Sum();
		_coolingModule.TargetCoolingLoad = AmbientHeatInflux + TotalSystemsHeat;
		TotalCoolingLoad = _coolingModule.CoolingLoad;
        EstimatedCoolingDuration = _coolingModule.EstimatedTimeRemaining;
		CoolantRemaining = _coolingModule.CoolantRemaining;

		// Determine internal temperature change, if any
		float heatFlow = TotalSystemsHeat + AmbientHeatInflux - TotalCoolingLoad;
		float temperatureDelta = heatFlow * _internalTemperatureChangeRate * Time.fixedDeltaTime;
		InternalTemperature = Mathf.Clamp(InternalTemperature + temperatureDelta, _targetInteriorTemperature, float.MaxValue);
	}

    private void UpdateTelemetry()
    {
        float relativeDepth = _shipHead.position.x * 0.286f;
		Depth = relativeDepth + FathomCount * _fathomDepth;
        CurrentSpeed = _engineModule.CurrentSpeed;
        CurrentHeading = _engineModule.CurrentHeading;
        MaximumSpeed = _engineModule.MaximumSpeed;
        TargetSpeed = _engineModule.TargetSpeed;
        TargetHeading = _engineModule.TargetHeading;

        if (relativeDepth >= _fathomDepth)
        {
            NextFathom();
        }
	}

    /// <summary>
    /// Called when the next fathom is reached. Position resets/etc. happen now.
    /// </summary>
    private void NextFathom()
    {
        FathomCount++;

		// TODO
	}
	#endregion Update Logic

	#region Events
	void FixedUpdate()
    {
        UpdatePower();
        UpdateTelemetry();
        UpdateHeat();
    }

	void Awake()
	{
        _instance = this;
	}
	#endregion Events
}
