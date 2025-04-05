using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ReactorModule : ShipModule
{
	#region Fields
	[SerializeField]
	protected float _startingReactorFuelConsumption = 1;
	[SerializeField]
	protected float _startingReactorFuel = 1;
	[SerializeField]
	protected float _startingMaximumFuelItems = 10;

	protected List<GameObject> _fuelItems = new List<GameObject>();
	#endregion Fields

	#region Methods
	/// <summary>
	/// Consumes a fuel item, if able.
	/// </summary>
	public bool AddFuel(GameObject FuelItem)
	{
		// Extract the fuel amount from the object and add it to the reactor
		if (_fuelItems.Count < _startingMaximumFuelItems)
		{
			_fuelItems.Add(FuelItem);
			return true;
		}

		// if the reactor is full, no more fuel can be added
		return false;
	}
	#endregion Methods

	#region Properties
	/// <summary>
	/// Total fuel left in this module (MWh)
	/// </summary>
	public float FuelRemaining
	{
		get
		{
			// Loop through the stored list of fuel items and get their fuel amount.
			return _startingReactorFuel;
		}
	}
	/// <summary>
	/// Power produced by this module (kWe)
	/// </summary>
	public float FuelConsumption
	{
		get
		{
			return GetModifiedValue(ModifiedStat.ReactorFuelConsumption, _startingReactorFuelConsumption);
		}
	}
	#endregion Properties

	#region Construction
	public ReactorModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Construction

	private void Update()
	{
		// Process consuming reactor fuel
	}
}
