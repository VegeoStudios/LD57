using System;
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
	#endregion Fields

	#region Properties
	/// <summary>
	/// Total coolant left in this module (kWh)
	/// </summary>
	public float CoolantRemaining
	{
		get
		{
			// Loop through the stored list of fuel items and get their fuel amount.
			return _startingCoolant;
		}
	}
	/// <summary>
	/// Cooling load produced by this module (kWt)
	/// </summary>
	public float CoolingLoad
	{
		get
		{
			return GetModifiedValue(ModifierStatType.ModuleEfficiency, _startingCoolingLoad);
		}
	}
	#endregion Properties

	#region Construction
	public CoolingModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Construction

	private void Update()
	{
		// Process consuming coolant
	}
}
