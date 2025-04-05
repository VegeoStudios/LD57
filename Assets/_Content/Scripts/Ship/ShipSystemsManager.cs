using System.Collections.Generic;
using System;
using UnityEngine;

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

	#region Properties
	#region Heat
	/// <summary>
	/// External Temperature (C) - About +30C/1km 
	/// </summary>
	public float ExternalTemperature {  get; protected set; }
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
    private const float TargetInteriorTemperature = 25; // C
	private const float AmbientTemperatureRate = 30 / 1000; // C/m
    private const float Mass = 2000 * 1000; // kg
    private const float SpecificHeatCapacity = 300; // J/kg-K
    private const float ThermalConductivity = 2; // W/m-K
    private const float SurfaceArea = 500; // m^2
    private const float HullThickness = 1; // m
    // Derived
    private const float InternalTemperatureChangeRate = 1000 / (SpecificHeatCapacity * Mass); // C/kWt-s
    private const float AmbientHeatRate = ThermalConductivity * SurfaceArea / HullThickness / 1000; // kWt/K
	#endregion Constants

	#region Fields
	private List<ShipModule> _shipModules = new List<ShipModule>();
    private ReactorModule _reactorModule = null;
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
	#endregion Game Object Callbacks

	#region Update Logic
	private void UpdatePower()
    {
        float totalProduction = 0;
        float totalDemand = 0;

        // Update remaining fuel from ReactorModule
        FuelRemaining = _reactorModule.FuelRemaining;

        foreach (ShipModule module in _shipModules)
        {
            totalProduction += module.PowerProduction;
            totalDemand += module.PowerDemand;
        }

        TotalPowerProduction = totalProduction;
        TotalPowerDemand = totalDemand;
    }

    private void UpdateHeat()
    {
        // Update ambient values
        ExternalTemperature = Depth * AmbientTemperatureRate;
		AmbientHeatInflux = (ExternalTemperature - InternalTemperature) * AmbientHeatRate;

		// Sum heat generation and cooling loads
		float totalGeneration = 0;
		float totalCooling = 0;

		foreach (ShipModule module in _shipModules)
		{
			totalGeneration += module.HeatGeneration;
			totalCooling += module.CoolingLoad;
		}

		TotalSystemsHeat = totalGeneration;
        TotalCoolingLoad = totalCooling;

        // Determine internal temperature change, if any
        float heatFlow = TotalSystemsHeat + AmbientHeatInflux - TotalCoolingLoad;
		float temperatureDelta = heatFlow * InternalTemperatureChangeRate * Time.fixedDeltaTime;
		InternalTemperature = Mathf.Clamp(InternalTemperature + temperatureDelta, TargetInteriorTemperature, float.MaxValue);
	}
	#endregion Update Logic

    void FixedUpdate()
    {
        UpdatePower();
        UpdateHeat();
    }
}
