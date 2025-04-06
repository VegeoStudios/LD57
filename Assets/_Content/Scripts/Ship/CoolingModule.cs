using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Cooling module provides cooling to the ship.
/// </summary>
[Serializable]
public class CoolingModule : ShipModule
{
	#region Fields
	[SerializeField]
	protected float _startingCoolingLoad = 9000f; // kWt
	[SerializeField]
	protected float _startingCoolant = 1500f; // kWh
	[SerializeField]
	protected ItemSlot _coolantSlot = null;

	// Not shown in inspector
	protected const float _tolerance = 0.001f; // kWe
	protected float _currentCoolant; // kWh
	protected float _currentCoolingLoad; //kWe
	protected float _currentTargetCooling; // kWe
	protected TimeSpan _timeRemaining; // time
	#endregion Fields

	#region Properties
	/// <summary>
	/// An estimate of how long the module can run at its current demand with its current coolant (time)
	/// </summary>
	public TimeSpan EstimatedTimeRemaining
	{
		get
		{
			return _timeRemaining;
		}
	}
	/// <summary>
	/// Total coolant left in this module (kWh)
	/// </summary>
	public float CoolantRemaining
	{
		get
		{
			return _currentCoolant;
		}
	}
	/// <summary>
	/// Cooling load by this module (kWt)
	/// </summary>
	public float CoolingLoad
	{
		get
		{
			return _currentCoolingLoad;
		}
	}
	/// <summary>
	/// <see cref="ShipSystemsManager"/> sets this value to the required amount.
	/// </summary>
	public float TargetCoolingLoad
	{
		set
		{
			_currentTargetCooling = value;
		}
	}
	/// <summary>
	/// Maximum cooling load the module is capable of (kWt)
	/// </summary>
	public float MaximumCoolingLoad
	{
		get
		{
			return GetModifiedValue(ModifierStatType.ModuleEfficiency, CoreFunctionEfficiency * _startingCoolingLoad);
		}
	}
	#endregion Properties

	#region Construction
	public CoolingModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
		_currentCoolant = _startingCoolant;
	}
	#endregion Construction

	#region Methods
	private void UpdateCooling()
	{
		// Adjust cooling load based on demand
		if (_currentTargetCooling >= MaximumCoolingLoad)
		{
			// Demand exceeds capacity.
			_currentCoolingLoad = MaximumCoolingLoad;
		}
		else if (!IsActive || _currentTargetCooling <= _tolerance || _currentCoolant < _tolerance)
		{
			// The module is off. 
			_currentCoolingLoad = 0f;
		}
		else
		{
			// Things are Gucci
			_currentCoolingLoad = _currentTargetCooling;
		}
	}

	private void UpdateCoolant()
	{
		// Check if coolant is being loaded
		if (!(_coolantSlot.SlottedItem is null) && _coolantSlot.SlottedItem.ItemType == ItemType.Coolant)
		{
			_currentCoolant += _coolantSlot.SlottedItem.CoolantValue;
			Destroy(_coolantSlot.RemoveSlottedItem().GetComponent<GameObject>());
		}

		float coolantConsumed = Time.fixedDeltaTime * _currentCoolingLoad * OperationalEfficiency / 3600f;

		if (!IsActive || coolantConsumed <= _tolerance)
		{
			return;
		}

		_currentCoolant = Mathf.Max(_currentCoolant - coolantConsumed, 0f);
	}

	private void UpdateTime()
	{
		if (!IsActive || _currentCoolingLoad < _tolerance)
		{
			// Avoid a DIV0
			_timeRemaining = new TimeSpan(0, 0, 0);
			return;
		}

		_timeRemaining = new TimeSpan(0, 0, (int)(3600f * _currentCoolant / _currentCoolingLoad));
	}

	protected override void ModuleIdle()
	{
		base.ModuleIdle();
		_currentCoolingLoad = 0f;
		_timeRemaining = new TimeSpan(0, 0, 0);
	}
	#endregion Methods

	private void FixedUpdate()
	{
		if (IsActive)
		{
			UpdateCooling();
			UpdateCoolant();
			UpdateTime();
		}
		else
		{
			ModuleIdle();
		}
	}
}
