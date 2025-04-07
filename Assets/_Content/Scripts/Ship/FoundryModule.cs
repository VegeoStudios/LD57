using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Processes crafting of items from raw materials.
/// </summary>
[Serializable]
public class FoundryModule : ShipModule
{
    #region Fields
    public List<CraftingRecipe> AvailableRecipes = new List<CraftingRecipe>();
    [SerializeField]
	protected ItemSlot _outputSlot = null;

	// Not shown in inspector
	protected float _currentProcessingTimeElapsed = 0f;
	protected CraftingRecipe _currentCraftingRecipe = null;

    #endregion Fields

    #region Properties
    /// <summary>
    /// The current time total the crafter is counting up to (s)
    /// </summary>
    public float CurrentTargetProcessingTime
	{
		get
		{
			if (_currentCraftingRecipe == null || !IsActive)
			{
				return 0f;
			}

			return GetModifiedValue(ModifierStatType.ModuleEfficiency,
				_currentCraftingRecipe.CraftingTime * OperationalEfficiency * CoreFunctionEfficiency);
		}
	}
	#endregion Properties

	#region Methods
	/// <summary>
	/// Crafts the passed recipe if able. Assumes crafting is legal.
	/// </summary>
	public void Craft(CraftingRecipe recipe)
	{
		// Check the storage module for each crafting component.
		foreach (CraftingComponent component in recipe.Ingredients)
		{
			ShipSystemsManager.Instance.StorageModule.StoredItems[component.Item.Name] -= component.Amount;
		}

		ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty = true;

        _currentCraftingRecipe = recipe;
	}

	private void UpdateCraftingProgress()
	{
		if (!(_currentCraftingRecipe == null))
		{
			if (_currentProcessingTimeElapsed >= CurrentTargetProcessingTime)
			{
				if (!(_outputSlot.SlottedItem == null))
				{
					// Something is in the way.
					return;
				}

				_outputSlot.SlottedItem = _currentCraftingRecipe.Result.Item;
				_outputSlot.UpdateSprite();
				_currentCraftingRecipe = null;
				_currentProcessingTimeElapsed = 0f;

				UpdateCraftableRecipes();

				ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty = true;
            }

			_currentProcessingTimeElapsed += Time.fixedDeltaTime;
		}
	}

	private void UpdateCraftableRecipes()
	{
		// Slot is full or we're crafting stuff.
		if (!(_outputSlot.SlottedItem == null) || !(_currentCraftingRecipe == null))
		{
			AvailableRecipes.ForEach(r => r.CanCraft = false);
			return;
		}

		foreach (CraftingRecipe recipe in AvailableRecipes)
		{
			recipe.CanCraft = true;
			// Check the storage module for each crafting component.
			foreach (CraftingComponent component in recipe.Ingredients)
			{
				int storedQuantity = ShipSystemsManager.Instance.StorageModule.StoredItems[component.Item.Name];
				if (storedQuantity < component.Amount)
				{
					// We are inextricably poor
					recipe.CanCraft = false;
					break;
				}
			}
		}
	}
	#endregion Methods

	#region Events
	void FixedUpdate()
	{
		UpdateUI();
		if (IsActive)
		{
			UpdateCraftableRecipes();
			UpdateCraftingProgress();
		}
		else
		{
			ModuleIdle();
		}
	}

    void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Events
}
