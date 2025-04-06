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
            return _instance ?? (_instance = new ShipSystemsManager());
        }
        set
        {
            _instance = value;
        }
    }
	#endregion Singleton

	#region Construction
    /// <summary>
    /// Creates a new ship systems manager.
    /// </summary>
    public ShipSystemsManager() : base()
    {
        Instance = this;
	}
    #endregion Construction

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
    /// Total heat leaving the ship via cooling (kWt)
    /// </summary>
    public float TotalCoolingLoad { get; protected set; }
	#endregion Heat

	#region Power
    /// <summary>
    /// Total unspent fuel in reactor (MWh)
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
    public TimeSpan EstimatedRuntime { get; protected set; }
	#endregion Power

	#region Environment
    /// <summary>
    /// Current Fathom zone
    /// </summary>
    public int FathomCount { get; protected set; }
    /// <summary>
    /// Name of the current Fathom Region
    /// </summary>
    public string FathomRegionName { get; protected set; }
    /// <summary>
    /// Current depth of the ship's nose (m)
    /// </summary>
    public float Depth { get; protected set; }
	#endregion Environment

	#region Telemetry
    /// <summary>
    /// Current maximum allowed speed of ship (m/s)
    /// </summary>
    public float MaximumSpeed { get; protected set; }
    /// <summary>
    /// Current speed of ship (m/s)
    /// </summary>
    public float CurrentSpeed { get; protected set; }
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
    // Derived
    private const float _internalTemperatureChangeRate = 1000f / (_specificHeatCapacity * _mass); // C/kWt-s
    private const float _ambientHeatRate = _thermalConductivity * _surfaceArea / _hullThickness / 1000f; // kWt/K
	#endregion Constants

	#region Fields
	private List<ShipModule> _shipModules = new List<ShipModule>();
    private ReactorModule _reactorModule = null;
    private CoolingModule _coolingModule = null;
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
	#endregion Game Object Callbacks

	#region Update Logic
	private void UpdatePower()
    {
        _reactorModule.TargetPowerProduction = TotalPowerDemand = _shipModules.Select(mod => mod.PowerDemand).Sum();
		FuelRemaining = _reactorModule.FuelRemaining;
		TotalPowerProduction = _reactorModule.PowerProduction;
        EstimatedRuntime = _reactorModule.EstimatedTimeRemaining;

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
		Depth = _shipHead.position.x * 0.286f;
		ExternalTemperature = Depth * _ambientTemperatureRate + _targetInteriorTemperature;
		AmbientHeatInflux = (ExternalTemperature - InternalTemperature) * _ambientHeatRate;
		TotalSystemsHeat = _shipModules.Select(mod => mod.HeatGeneration).Sum();
		_coolingModule.TargetCoolingLoad = AmbientHeatInflux + TotalSystemsHeat;
		TotalCoolingLoad = _coolingModule.CoolingLoad;
 
        // Determine internal temperature change, if any
        float heatFlow = TotalSystemsHeat + AmbientHeatInflux - TotalCoolingLoad;
		float temperatureDelta = heatFlow * _internalTemperatureChangeRate * Time.fixedDeltaTime;
		InternalTemperature = Mathf.Clamp(InternalTemperature + temperatureDelta, _targetInteriorTemperature, float.MaxValue);
	}
	#endregion Update Logic

    void FixedUpdate()
    {
        UpdatePower();
        UpdateHeat();
    }
}
