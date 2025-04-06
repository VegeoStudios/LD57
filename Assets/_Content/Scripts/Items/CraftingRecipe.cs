using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores an instruction or result for a <see cref="CraftingRecipe"/>
/// </summary>
[System.Serializable]
public struct CraftingComponent
{
	/// <summary>
	/// The item required for or produced by this step.
	/// </summary>
	public Item Item;
	/// <summary>
	/// How much of the item is involved.
	/// </summary>
	public int Amount;

	public CraftingComponent(Item item, int amount)
	{
		Item = item;
		Amount = amount;
	}
}

/// <summary>
/// Crafting recipes are instructions for the foundry module to craft items.
/// </summary>
[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Items/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
	#region Fields
	public CraftingComponent Result;
    public List<CraftingComponent> Ingredients = new List<CraftingComponent>();

	/// <summary>
	/// Time required to craft this recipe (s)
	/// </summary>
    public float CraftingTime = 2f;
	#endregion Fields

	#region Properties
	/// <summary>
	/// Returns the name of the result component.
	/// </summary>
	public string Name
	{
		get
		{
			return Result.Item.Name;
		}
	}
	#endregion Properties
}
