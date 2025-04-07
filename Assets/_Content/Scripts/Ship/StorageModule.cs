using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Module storing all of the ship's items.
/// </summary>
[Serializable]
public class StorageModule : ShipModule
{
	#region Fields
	public Dictionary<string, int> StoredItems = new Dictionary<string, int>();
	public List<Item> ItemPrefabs = new List<Item>();
	[SerializeField]
	protected ItemSlot _storageSlot = null;
	[SerializeField]
	protected int _startingStorageCapacity = 50;
	// Tracks if the current slotted item was retrieved from storage
	private bool _itemRetrieved = false;
	/// <summary>
	/// True if the storage slot is empty
	/// </summary>
	public bool CanRetrieveItem = true;
	#endregion Fields

	#region Properties
	/// <summary>
	/// The maximum number of items this storage module can hold.
	/// </summary>
	public int StorageCapacity
	{
		get
		{
			return (int)GetModifiedValue(ModifierStatType.ModuleEfficiency, CoreFunctionEfficiency * _startingStorageCapacity);
		}
	}
	/// <summary>
	/// How full the storage currently is.
	/// </summary>
	public int StorageFillPercent
	{
		get
		{
			return 100 * StoredItems.Values.Sum() / StorageCapacity;
		}
	}
	#endregion Properties

	#region Methods
	/// <summary>
	/// Extracts an item from storage and places it in the storage slot if able.
	/// </summary>
	public bool RetrieveItem(string itemName)
	{
		bool result = false;
		if (CanRetrieveItem)
		{
			Item item = ItemPrefabs.First(i => i.Name == itemName);
			result = _storageSlot.SlottedItem = item;
		}

		UpdateStorageSlot();
		return result;
	}

	private void UpdateStorageSlot()
	{
		CanRetrieveItem = _storageSlot.SlottedItem == null;

		if (_itemRetrieved)
		{
			// Something is waiting to be collected
			_itemRetrieved = !CanRetrieveItem;
		}
		else if (!CanRetrieveItem && StoredItems.Count < StorageCapacity)
		{
			StoredItems[_storageSlot.SlottedItem.Name] += 1;
			_storageSlot.SlottedItem = null;
		}
	}
	#endregion Methods

	#region Events
	void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	
	void FixedUpdate()
	{
		// We do not call the base UI update because this module never turns off.
		UpdateStorageSlot();
		ModuleIdle();
	}
	#endregion Events
}
