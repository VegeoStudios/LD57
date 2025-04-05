using System;
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
	#endregion Fields

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
	public float PowerProduction
	{
		get
		{
			return GetModifiedValue(ModifierStatType.ModuleEfficiency, _startingReactorPower);
		}
	}
	#endregion Properties

	#region Construction
	public ReactorModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Construction

	#region Methods
	public void UpdateFuel()
	{

	}
	#endregion Methods

	private void FixedUpdate()
	{
		UpdateFuel();
	}
}
