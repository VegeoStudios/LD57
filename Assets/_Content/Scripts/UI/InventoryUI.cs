using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemEntryPrefab;
    [SerializeField] private Transform _itemEntryParent;

    private StorageItemEntry[] _itemEntries;

    private bool _noLongerDirty = false;

    private void Awake()
    {
        _itemEntries = new StorageItemEntry[ShipSystemsManager.Instance.StorageModule.ItemPrefabs.Count];

        int i = 0;
        foreach (Item item in ShipSystemsManager.Instance.StorageModule.ItemPrefabs)
        {
            GameObject itemEntry = Instantiate(_itemEntryPrefab, _itemEntryParent);
            StorageItemEntry entry = itemEntry.GetComponent<StorageItemEntry>();
            entry.Initialize(this);
            entry.UpdateUI(item, ShipSystemsManager.Instance.StorageModule.StoredItems[item.Name]);
            _itemEntries[i] = entry;
            i++;
        }
    }

    private void OnEnable()
    {
        SortItems();
    }

    private void Update()
    {
        if (ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty)
        {
            UpdateItems();
        }
    }

    private void LateUpdate()
    {
        if (_noLongerDirty)
        {
            ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty = false;
            _noLongerDirty = false;
        }
    }

    private void SortItems()
    {
        int min = int.MaxValue;
        int minIndex = 0;

        for (int indices = _itemEntries.Length; indices > 0; indices--)
        {
            for (int i = 0; i < indices; i++)
            {
                if (_itemEntries[i].Count < min)
                {
                    min = _itemEntries[i].Count;
                    minIndex = i;
                }
            }
            _itemEntries[minIndex].transform.SetAsFirstSibling();
        }
    }

    private void UpdateItems()
    {
        foreach (var entry in _itemEntries)
        {
            string itemName = entry.Name;
            if (ShipSystemsManager.Instance.StorageModule.StoredItems.TryGetValue(itemName, out int count))
            {
                entry.UpdateUI(ShipSystemsManager.Instance.StorageModule.ItemPrefabs.Find(i => i.Name == itemName), count);
            }
        }
        _noLongerDirty = true;
    }

    public void SelectItem(Item item)
    {
        Debug.Log($"Selected item: {item.Name}");
        ShipSystemsManager.Instance.StorageModule.RetrieveItem(item.Name);
    }
}
