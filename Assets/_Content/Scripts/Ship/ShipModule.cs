using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// A ship module is an interactable core system of the ship.
/// </summary>
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
	public List<ItemSlot> ItemSlots = new List<ItemSlot>();
	// Not shown in inspector
	protected bool _isActive = true;
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

	protected virtual void ModuleIdle()
	{
		OperationalEfficiency = 1f;
	}
	#endregion Methods

	#region Properties
	/// <summary>
	/// Determines if this module is active or not.
	/// </summary>
	public bool IsActive
	{ 
		get
		{
			return _isActive && OperationalEfficiency > 0.1f;
		}
		set
		{
			_isActive = value;
		}
	}
	/// <summary>
	/// The relative efficiency of this module (%)
	/// </summary>
	public float OperationalEfficiency;
	/// <summary>
	/// Heat output of this module (kWt)
	/// </summary>
	public float HeatGeneration
	{ 
		get
		{
			return IsActive ? GetModifiedValue(ModifierStatType.ModuleHeatProduction, _startingHeatGeneration) :
				0f;
		}
	}
    /// <summary>
    /// Power draw of this module (kWe)
    /// </summary>
    public float PowerDemand
	{
		get
		{
			return IsActive ? GetModifiedValue(ModifierStatType.ModulePowerDemand, _startingPowerDemand) :
				0f;
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
