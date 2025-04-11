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
	public AudioSource UIClickSound;
	public AudioSource UISubmitSound;
	public Dictionary<string, int> StoredItems = new Dictionary<string, int>();
	public bool StoredItemsUIDirty = true;
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
	public void RetrieveItem(string itemName)
	{
		if (CanRetrieveItem)
		{
			Item item = ItemPrefabs.First(i => i.Name == itemName);
            if (StoredItems.ContainsKey(itemName) && StoredItems[itemName] > 0)
            {
                StoredItems[itemName] -= 1;
                StoredItemsUIDirty = true;
            }
            _storageSlot.SlottedItem = item;
			_itemRetrieved = true;
        }

		UpdateStorageSlot(true);
	}

	private void UpdateStorageSlot(bool verbose = false)
	{
		CanRetrieveItem = _storageSlot.SlottedItem == null;

		if (verbose) Debug.Log("CanRetrieveItem: " + CanRetrieveItem);
		if (verbose) Debug.Log("_itemRetrieved: " + _itemRetrieved);

        if (_itemRetrieved)
		{
			// Something is waiting to be collected
			_itemRetrieved = !CanRetrieveItem;
		}
		else if (!CanRetrieveItem && StoredItems.Count < StorageCapacity)
		{
			Debug.Log("Adding item to storage: " + _storageSlot.SlottedItem.Name);
            StoredItems[_storageSlot.SlottedItem.Name] += 1;
            StoredItemsUIDirty = true;
            _storageSlot.SlottedItem = null;
		}
	}

	public int AddItem(string itemName, int amount)
    {
        amount = Mathf.Min(amount, StorageCapacity - GetCurrentCapacity());
        if (StoredItems.ContainsKey(itemName))
        {
            StoredItems[itemName] += amount;
        }
        else
        {
            StoredItems.Add(itemName, amount);
        }
        if (amount > 0)
        {
            StoredItemsUIDirty = true;
        }
        return amount;
    }

	public int GetCurrentCapacity()
	{
        int currentCapacity = 0;
        foreach (KeyValuePair<string, int> item in StoredItems)
        {
            currentCapacity += item.Value;
        }
        return currentCapacity;
    }
    #endregion Methods

    #region Events
    void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
		foreach (Item item in ItemPrefabs)
		{
			StoredItems.Add(item.Name, 0);
		}

		// DEBUG
		/*
		AddItem("Iron", 20);
		AddItem("Manganese", 20);
		AddItem("Water", 20);
		AddItem("T1 Navigation Beacon", 5);
		*/
    }
	
	void FixedUpdate()
	{
		// We do not call the base UI update because this module never turns off.
		UpdateStorageSlot();
		ModuleIdle();
	}
	#endregion Events
}
