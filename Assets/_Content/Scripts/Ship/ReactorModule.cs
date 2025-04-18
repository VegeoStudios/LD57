using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Reactor module controls the power production on the ship.
/// </summary>
[Serializable]
public class ReactorModule : ShipModule
{
	#region Fields
	[SerializeField]
	protected float _startingReactorPower = 24000f; // kWe
	[SerializeField]
	protected float _startingReactorFuel = 4000f; // kWh
	[SerializeField]
	protected ItemSlot _fuelSlot = null;

	// Not shown in inspector
	protected const float _tolerance = 0.001f; // kWe
	protected float _currentFuel; // kWh
	protected float _currentPowerProduction; //kWe
	protected float _currentTargetPower; // kWe
	protected TimeSpan _timeRemaining; // time
	#endregion Fields

	#region Properties
	/// <summary>
	/// An estimate of how long the reactor can run at its current power with its current fuel (time)
	/// </summary>
	public TimeSpan EstimatedTimeRemaining
	{
		get
		{
			return _timeRemaining;
		}
	}
	/// <summary>
	/// Total fuel left in this module (MWh)
	/// </summary>
	public float FuelRemaining
	{
		get
		{
			return _currentFuel;
		}
	}
	/// <summary>
	/// Power produced by this module (kWe)
	/// </summary>
	public float PowerProduction
	{
		get
		{
			return _currentPowerProduction;
		}
	}
	/// <summary>
	/// <see cref="ShipSystemsManager"/> sets this value to the required amount.
	/// </summary>
	public float TargetPowerProduction
	{
		set
		{
			_currentTargetPower = value;
		}
	}
	/// <summary>
	/// Maximum power production the reactor is capable of (kWe)
	/// </summary>
	public float MaximumPowerProduction
	{
		get
		{
			return GetModifiedValue(ModifierStatType.ModuleEfficiency, CoreFunctionEfficiency * _startingReactorPower);
		}
	}
	#endregion Properties

	#region Methods
	private void UpdateReactor()
	{
		// Adjust produced power based on demand
		if (_currentTargetPower >= MaximumPowerProduction)
		{
			// Demand exceeds capacity.
			_currentPowerProduction = MaximumPowerProduction;
		}
		else if (!IsActive || _currentTargetPower <= _tolerance || _currentFuel < _tolerance)
		{
			// The reactor is off. 
			_currentPowerProduction = 0f;
		}
		else
		{
			// Things are Gucci
			_currentPowerProduction = _currentTargetPower;
		}
	}

	private void UpdateFuel()
	{
		// Check if fuel is being loaded
		if (!(_fuelSlot.SlottedItem == null) && _fuelSlot.SlottedItem.ItemType == ItemType.Fuel)
		{
			_currentFuel += _fuelSlot.SlottedItem.FuelValue;
			_fuelSlot.SlottedItem = null;
		}

		float fuelConsumed = Time.fixedDeltaTime * _currentPowerProduction / (OperationalEfficiency * CoreFunctionEfficiency * 3600f);

		if (!IsActive || fuelConsumed <= _tolerance)
		{
			return;
		}

		_currentFuel = Mathf.Max(_currentFuel - fuelConsumed, 0f);
	}

	private void UpdateTime()
	{
		if (!IsActive || _currentPowerProduction < _tolerance)
		{
			// Avoid a DIV0
			_timeRemaining = new TimeSpan(0, 0, 0);
			return;
		}

		_timeRemaining = new TimeSpan(0, 0, (int)(3600f * _currentFuel / _currentPowerProduction));
	}

	protected override void ModuleIdle()
	{
		base.ModuleIdle();
		_currentPowerProduction = 0f;
		_timeRemaining = new TimeSpan(0, 0, 0);
	}
	#endregion Methods

	#region Events
	void FixedUpdate()
	{
		UpdateUI();
		if (IsActive)
		{
			UpdateReactor();
			UpdateFuel();
			UpdateTime();
		}
		else
		{
			ModuleIdle();
		}
	}

	void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
		_currentFuel = _startingReactorFuel;
	}
	#endregion Events
}
