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
    public void PlayUIClickSound()
    {
        StorageModule.UIClickSound.Play();
    }

    public void PlayUISubmitSound()
    {
		StorageModule.UISubmitSound.Play();
	}

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
    /// <summary>
    /// Time until losing the game from over  heating (s)
    /// </summary>
    public float OverheatTimeRemaining { get; protected set; }
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
    public int FathomCount { get; protected set; } = 0;
    /// <summary>
    /// The current tier of the game, determined by fathom count.
    /// </summary>
    public int CurrentTier { get; protected set; } = 1;
    /// <summary>
    /// Display name of the current tier.
    /// </summary>
    public string CurrentTierName 
    { 
        get
        {
            switch (CurrentTier)
            {
				case 1:
					{
						return "Crust Strata";
					}
				case 2:
					{
						return "Lithosphere";
					}
                case 3:
                    {
                        return "Asthenosphere";
					}
                default:
					{
						return "Mantle";
					}
			}
        }
    }
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

    public bool PowerAlert = false;
    public bool HeatAlert = false;
	#endregion Telemetry
	#endregion Properties

	#region Constants
    // Absolute
    private const float _targetInteriorTemperature = 25f; // C
    private const float _maxInteriorTemperature = 50f; // C
    private const float _heatTimeToFail = 30; // s
	private const float _ambientTemperatureRate = 30f / 1000f; // C/m
    private const float _mass = 1500f * 1000f; // kg
    private const float _specificHeatCapacity = 100f; // J/kg-K
    private const float _thermalConductivity = 8f; // W/m-K
    private const float _surfaceArea = 750f; // m^2
    private const float _hullThickness = 0.3f; // m
    private const float _blackoutThreshold = 1.25f; // %
    private const int _fathomDepth = 1000; // m
    private const int _fathomsPerTier = 3;
    // Derived
    private const float _internalTemperatureChangeRate = 1000f / (_specificHeatCapacity * _mass); // C/kWt-s
    private const float _ambientHeatRate = _thermalConductivity * _surfaceArea / _hullThickness / 1000f; // kWt/K
	#endregion Constants

	#region Fields
    public AudioSource AlarmHeat = null;
    public AudioSource AlarmPower = null;
	public StorageModule StorageModule = null;
    public FoundryModule FoundryModule = null;

    private float _heatTimeElapsed = 0f;

    // Not shown in inspector
    public List<ShipModule> ShipModules { get; private set; } = new List<ShipModule>();
    public ReactorModule ReactorModule { get; private set; } = null;
    public CoolingModule CoolingModule { get; private set; } = null;
    public EngineModule EngineModule { get; private set; } = null;
	#endregion Fields

	#region Game Object Callbacks
	/// <summary>
	/// Registers a new <see cref="ShipModule"/> with this manager.
	/// </summary>
	public void Callback(ShipModule module)
    {
        if (!ShipModules.Contains(module))
        {
			ShipModules.Add(module);
		}
	}
    /// <summary>
    /// Registers the <see cref="global::ReactorModule"/> with this manager.
    /// </summary>
    public void Callback(ReactorModule module)
	{
        ReactorModule = module;
        Callback((ShipModule)module);
	}
    /// <summary>
    /// Registers the <see cref="global::CoolingModule"/> with this manager.
    /// </summary>
    public void Callback(CoolingModule module)
	{
		CoolingModule = module;
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
    /// Registers the <see cref="global::EngineModule"/> with this manager.
    /// </summary>
    public void Callback(EngineModule module)
	{
		EngineModule = module;
		Callback((ShipModule)module);
	}
	#endregion Game Object Callbacks

	#region Update Logic
	private void UpdatePower()
    {
        ReactorModule.TargetPowerProduction = TotalPowerDemand = ShipModules.Where(mod => mod.IsActive).Select(mod => mod.PowerDemand).Sum();
		FuelRemaining = ReactorModule.FuelRemaining;
		TotalPowerProduction = ReactorModule.PowerProduction;
		EstimatedPowerDuration = ReactorModule.EstimatedTimeRemaining;
		OperationalEfficiency = 1f;

        if (!ReactorModule.IsActive)
        {
			// Ship is going into blackout!
			foreach (ShipModule module in ShipModules.Where(m => m != ReactorModule))
			{
				module.IsActive = false;
			}

            AlarmHeat.Stop();
            AlarmPower.Stop();
            PowerAlert = false;
            HeatAlert = false;
		}
		else
		{
            float utilization = TotalPowerDemand / ReactorModule.MaximumPowerProduction;
            if (utilization > _blackoutThreshold)
            {
				// Ship is going into blackout!
				ReactorModule.IsActive = false;
			}
            else if (utilization > 1f)
            {
                // Ship is over-stressed and module efficiency will be reduced.
                OperationalEfficiency = 2f - utilization;
                if (!AlarmPower.isPlaying)
                {
					AlarmPower.Play();
                    PowerAlert = true;
				}
			}
            else
            {
				AlarmPower.Stop();
				PowerAlert = false;
			}
        }

		ShipModules.ForEach(mod => mod.OperationalEfficiency = OperationalEfficiency);
	}

    private void UpdateHeat()
    {
		ExternalTemperature = Depth * _ambientTemperatureRate + _targetInteriorTemperature;
		AmbientHeatInflux = (ExternalTemperature - InternalTemperature) * _ambientHeatRate;
		TotalSystemsHeat = ShipModules.Where(mod => mod.IsActive).Select(mod => mod.HeatGeneration).Sum();
        float hvacCoolingLoad = Mathf.Clamp((ExternalTemperature - InternalTemperature) * 75, 0f, float.MaxValue);
		CoolingModule.TargetCoolingLoad = AmbientHeatInflux + TotalSystemsHeat + hvacCoolingLoad;
		TotalCoolingLoad = CoolingModule.CoolingLoad;
        EstimatedCoolingDuration = CoolingModule.EstimatedTimeRemaining;
		CoolantRemaining = CoolingModule.CoolantRemaining;

        if (InternalTemperature > _maxInteriorTemperature)
        {
            // Heat warning.
            _heatTimeElapsed += Time.fixedDeltaTime;
            if (!AlarmHeat.isPlaying)
            {
				AlarmHeat.Play();
                HeatAlert = true;
			}
		}
        else
        {
            _heatTimeElapsed -= Time.fixedDeltaTime * 0.1f;
			AlarmHeat.Stop();
			HeatAlert = false;
		}

		_heatTimeElapsed = Mathf.Clamp(_heatTimeElapsed, 0f, _heatTimeToFail);
        OverheatTimeRemaining = _heatTimeToFail - _heatTimeElapsed;
        if (OverheatTimeRemaining <= 0f)
        {
            // Game over!
            // TODO
            Application.Quit();
        }

		// Determine internal temperature change, if any
		float heatFlow = TotalSystemsHeat + AmbientHeatInflux - TotalCoolingLoad;
		float temperatureDelta = heatFlow * _internalTemperatureChangeRate * Time.fixedDeltaTime;
		InternalTemperature = Mathf.Clamp(InternalTemperature + temperatureDelta, _targetInteriorTemperature, float.MaxValue);
	}

    private bool _isSurfaced = true;
    private void UpdateTelemetry()
    {
		Depth = _shipHead.position.x;
		float relativeDepth = Depth - (FathomCount - 1) * _fathomDepth;
		CurrentSpeed = EngineModule.CurrentSpeed;
        CurrentHeading = EngineModule.CurrentHeading;
        MaximumSpeed = EngineModule.MaximumSpeed;
        TargetSpeed = EngineModule.TargetSpeed;
        TargetHeading = EngineModule.TargetHeading;

        if (Depth <= 0f && _isSurfaced)
        {
            _isSurfaced = false;
			NextFathom();
		}

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
		CurrentTier = 1 + FathomCount / _fathomsPerTier;
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
