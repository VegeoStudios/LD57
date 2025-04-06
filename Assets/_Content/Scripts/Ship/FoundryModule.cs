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
	protected float _targetProcessingTime = 1f;
	protected CraftingRecipe _currentCraftingRecipe;
	#endregion Fields

	#region Properties
	
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
	public bool Craft(CraftingRecipe recipe)
	{
		// Something is in the way!
		if (!(_outputSlot.SlottedItem is null))
		{
			return false;
		}

		// Check the warehouse for each crafting component.
		foreach (CraftingComponent component in recipe.Ingredients)
		{
			int foundQuantity = 0;
			ShipSystemsManager.Instance.StorageModules.ForEach(m => foundQuantity += m.Count(component.Item.Name));

			if (foundQuantity < component.Amount)
			{
				return false;
			}

			int consumedQuantity = 0;
			foreach (StorageModule storage in ShipSystemsManager.Instance.StorageModules)
			{
				consumedQuantity += storage.DestroyItems(component.Item.Name, component.Amount - consumedQuantity);
				if (consumedQuantity >= component.Amount)
				{
					// We've done it!
					break;
				}
			}

			// This probably isn't right and we need to instantiate a game object or something
			_outputSlot.SlottedItem = component.Item;
		}

		return true;
	}

	private void UpdateCraftingProgress()
	{
		
	}

	protected override void ModuleIdle()
	{
		base.ModuleIdle();
		_currentCraftingRecipe = null;
		_currentProcessingTimeElapsed = 0f;
		_targetProcessingTime = 1f;
	}
	#endregion Methods

	void FixedUpdate()
	{
		if (IsActive)
		{
			UpdateCraftingProgress();
		}
		else
		{
			ModuleIdle();
		}
	}
}
