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
	[SerializeField]
	protected ItemSlot _outputSlot = null;
	[SerializeField]
	protected List<CraftingRecipe> _availableRecipes = new List<CraftingRecipe>();

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
			if (_currentCraftingRecipe is null || !IsActive)
			{
				return 0f;
			}

			return GetModifiedValue(ModifierStatType.ModuleEfficiency,
				_currentCraftingRecipe.CraftingTime / OperationalEfficiency);
		}
	}
	#endregion Properties

	#region Construction
	public FoundryModule() : base()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Construction

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

		_currentCraftingRecipe = recipe;
	}

	private void UpdateCraftingProgress()
	{
		if (!(_currentCraftingRecipe is null))
		{
			if (_currentProcessingTimeElapsed >= CurrentTargetProcessingTime)
			{
				if (!(_outputSlot.SlottedItem is null))
				{
					// Something is in the way.
					return;
				}

				_outputSlot.InsertItem(_currentCraftingRecipe.Result.Item);
				_currentCraftingRecipe = null;
				_currentProcessingTimeElapsed = 0f;
			}

			_currentProcessingTimeElapsed += Time.deltaTime;
		}
	}

	private void UpdateCraftableRecipes()
	{
		// Slot is full or we're crafting stuff.
		if (!(_outputSlot.SlottedItem is null) || !(_currentCraftingRecipe is null))
		{
			_availableRecipes.ForEach(r => r.CanCraft = false);
			return;
		}

		foreach (CraftingRecipe recipe in _availableRecipes)
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

	void FixedUpdate()
	{
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
}
