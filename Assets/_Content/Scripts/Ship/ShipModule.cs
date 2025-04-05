using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class ShipModule : MonoBehaviour
{
	#region Fields
	[SerializeField]
	private string _moduleName = string.Empty;
	[SerializeField]
	private float _startingHeatGeneration = 1;
	[SerializeField]
	private float _startingPowerDemand = 1;
	[SerializeField]
	private float _startingPowerProduction = 0;
	[SerializeField]
	private float _startingCoolingLoad = 0;
	[SerializeField]
	private List<AppliesModifier> _modifiers = new List<AppliesModifier>();
	#endregion Fields

	#region Methods
	private float GetModifiedValue(ModifiedStat stat, float value)
	{
		foreach (AppliesModifier modifier in _modifiers.Where(m => m.ModifiedStat == stat))
		{
			value = modifier.GetModifiedValue(value);
		}

		return value;
	}
	#endregion Methods

	#region Properties
	/// <summary>
	/// Heat output of this module (kWt)
	/// </summary>
	public float HeatGeneration
	{ 
		get
		{
			return GetModifiedValue(ModifiedStat.ModuleHeatProduction, _startingHeatGeneration);
		}
	}
	/// <summary>
	/// Power produced by this module (kWe)
	/// </summary>
	public float PowerProduction
	{ 
		get
		{
			return GetModifiedValue(ModifiedStat.ModulePowerProduction, _startingPowerProduction);
		}
	}
    /// <summary>
    /// Power draw of this module (kWe)
    /// </summary>
    public float PowerDemand
	{
		get
		{
			return GetModifiedValue(ModifiedStat.ModulePowerDemand, _startingPowerDemand);
		}
	}
	/// <summary>
	/// Cooling load provided by module (kWt)
	/// </summary>
	public float CoolingLoad
	{ 
		get
		{
			return GetModifiedValue(ModifiedStat.ModuleCoolingLoad, _startingCoolingLoad);
		}
	}
	/// <summary>
	/// Name of this module
	/// </summary>
	public string Name
	{ 
		get
		{
			return _moduleName;
		}
	}
	#endregion Properties

	#region Construction
	public ShipModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Construction
}
