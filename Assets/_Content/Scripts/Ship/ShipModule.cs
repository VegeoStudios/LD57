using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class ShipModule : MonoBehaviour
{
	#region Fields
	[SerializeField]
	protected string _moduleName = string.Empty;
	[SerializeField]
	protected float _startingHeatGeneration = 1; // kWt
	[SerializeField]
	protected float _startingPowerDemand = 1; // kWe
	[SerializeField]
	protected float _startingPowerProduction = 0; // kWe
	[SerializeField]
	protected float _startingCoolingLoad = 0; // kWt

	public List<ItemSlot> ItemSlots = new List<ItemSlot>();
	#endregion Fields

	#region Methods
	/// <summary>
	/// Collects <see cref="Modifier"/> information from all slotted <see cref="Item"/>s.
	/// </summary>
	protected List<Modifier> GetModuleModifiers(ModifierStatType stat)
	{
		List<Modifier> modifiers = new List<Modifier>();

		foreach (ItemSlot slot in ItemSlots)
		{
			if (!(slot.SlottedItem is null))
			{
				IEnumerable<Modifier> itemModifiers = slot.SlottedItem.Modifiers.Where(m => m.ModifiedStat == stat);
				if (itemModifiers.Count() > 0)
				{
					modifiers.AddRange(itemModifiers);
				}
			}
		}

		return modifiers;
	}

	protected float GetModifiedValue(ModifierStatType stat, float value)
	{
		foreach (Modifier modifier in GetModuleModifiers(stat))
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
			return GetModifiedValue(ModifierStatType.ModuleHeatProduction, _startingHeatGeneration);
		}
	}
	/// <summary>
	/// Power produced by this module (kWe)
	/// </summary>
	public float PowerProduction
	{ 
		get
		{
			return GetModifiedValue(ModifierStatType.ModulePowerProduction, _startingPowerProduction);
		}
	}
    /// <summary>
    /// Power draw of this module (kWe)
    /// </summary>
    public float PowerDemand
	{
		get
		{
			return GetModifiedValue(ModifierStatType.ModulePowerDemand, _startingPowerDemand);
		}
	}
	/// <summary>
	/// Cooling load provided by module (kWt)
	/// </summary>
	public float CoolingLoad
	{ 
		get
		{
			return GetModifiedValue(ModifierStatType.ModuleCoolingLoad, _startingCoolingLoad);
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
