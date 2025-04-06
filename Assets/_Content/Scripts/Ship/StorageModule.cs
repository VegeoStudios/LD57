using System;
using System.Collections.Generic;
using System.Linq;

public class StorageModule : ShipModule
{
	#region Properties
	public List<Item> StoredItems = new List<Item>();
	#endregion Properties

	#region Fields
	protected int _storageCapacity = 100;
	#endregion Fields

	#region Methods
	/// <summary>
	/// The maximum number of items this storage module can hold.
	/// </summary>
	public int StorageCapacity
	{
		get
		{
			return _storageCapacity;
		}
	}
	/// <summary>
	/// Attempts to store the passed item.
	/// </summary>
	/// <returns>Returns true if successful.</returns>
	public bool StoreItem(Item item)
	{
		if (StoredItems.Count == _storageCapacity)
		{
			return false;
		}

		StoredItems.Add(item);
		return true;
	}
	/// <summary>
	/// Remove an item from storage and give it to the player.
	/// </summary>
	public Item RetrieveItem(string itemName)
	{
		IEnumerable<Item> storedItemsWithThisName = StoredItems.Where(i => i.Name == itemName);

		if (!storedItemsWithThisName.Any())
		{
			return null;
		}

		Item itemToRetrieve = storedItemsWithThisName.LastOrDefault();
		StoredItems.Remove(itemToRetrieve);
		return itemToRetrieve;
	}
	/// <summary>
	/// Returns the amount of an item in this module.
	/// </summary>
	public int Count(string itemName)
	{
		return StoredItems.Where(i => i.Name == itemName).Count();
	}
	/// <summary>
	/// Destroys item objects as requested, usually for crafting.
	/// </summary>
	/// <returns>Number of items destroyed.</returns>
	public int DestroyItems(string itemName, int count)
	{
		List<Item> storedItemsWithThisName = StoredItems.Where(i => i.Name == itemName).ToList();
		int itemsToDelete = Math.Min(storedItemsWithThisName.Count, count);

		foreach (Item item in storedItemsWithThisName)
		{
			StoredItems.Remove(item);
		}

		return itemsToDelete;
	}
	#endregion Methods
}
