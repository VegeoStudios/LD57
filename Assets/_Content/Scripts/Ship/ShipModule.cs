using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

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
	protected float _startingHeatGeneration = 1f; // kWt
	[SerializeField]
	protected float _startingPowerDemand = 1f; // kWe
	[SerializeField]
	protected float _coreFunctionalEfficiency = 1f; // Core function, affected by tier
	public List<ItemSlot> ItemSlots = new List<ItemSlot>();
	public Button PowerToggleButton = null;
	public TextMeshProUGUI PowerButtonText = null;

    // Not shown in inspector
    [SerializeField] protected bool _isActive = true;
    protected bool _isActiveUIState = true;
	#endregion Fields

	#region Methods
	/// <summary>
	/// Very simple method to invert the active state.
	/// </summary>
	public void TogglePowerState()
	{
		_isActive = !_isActive;
	}

	/// <summary>
	/// Must be called with other updates by child classes.
	/// </summary>
	protected void UpdateUI()
	{
		if (_isActiveUIState != IsActive)
		{
			// A toggle has occurred.
			if (IsActive)
			{
				// Power is on
				PowerToggleButton.image.color = new Color(0f, 0.784f, 0.196f, 0.5f);
				PowerButtonText.text = "Power: On";
			}
			else
			{
				// Power is off
				PowerToggleButton.image.color = new Color(0.784f, 0f, 0.196f, 0.5f);
				PowerButtonText.text = "Power: Off";
			}

			// Finalize the toggle.
			_isActiveUIState = _isActive = IsActive;
		}
	}

	/// <summary>
	/// Collects <see cref="Modifier"/> information from all slotted <see cref="Item"/>s.
	/// </summary>
	protected List<Modifier> GetModuleModifiers(ModifierStatType stat)
	{
		List<Modifier> modifiers = new List<Modifier>();

		if (!ItemSlots.Contains(CoreSlot))
		{
			if (!(CoreSlot.SlottedItem == null))
			{
				IEnumerable<Modifier> itemModifiers = CoreSlot.SlottedItem.Modifiers.Where(m => m.ModifiedStat == stat);
				if (itemModifiers.Count() > 0)
				{
					modifiers.AddRange(itemModifiers);
				}
			}
		}

		foreach (ItemSlot slot in ItemSlots)
		{
			if (!(slot.SlottedItem == null))
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
	/// Core item controlling base efficiency value a.k.a tier.
	/// </summary>
	public ItemSlot CoreSlot
	{ 
		get
		{
			return ItemSlots.First(i => i.IsCoreSlot);
		}
	}
	/// <summary>
	/// The current tier of this module.
	/// </summary>
	public int Tier
	{
		get
		{
			return CoreSlot.SlottedItem?.Tier ?? 0;
		}
	}
	/// <summary>
	/// Determines if this module is active or not.
	/// </summary>
	public bool IsActive
	{ 
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
		}
	}
	/// <summary>
	/// The relative efficiency of this module depending on power grid status (%)
	/// </summary>
	public float OperationalEfficiency;
	/// <summary>
	/// Core stat modifier depending on module tier upgrades (%)
	/// </summary>
	/// <remarks>Not affected by enabled-ness.</remarks>
	public float CoreFunctionEfficiency
	{
		get
		{
			return GetModifiedValue(ModifierStatType.CoreFunction, _coreFunctionalEfficiency);
		}
	}
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

	#region Events
	void FixedUpdate()
	{
		UpdateUI();
	}

	void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Events
}
