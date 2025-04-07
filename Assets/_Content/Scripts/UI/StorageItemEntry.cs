using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageItemEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText = null;
    [SerializeField] private Image _itemIcon = null;
    [SerializeField] private TextMeshProUGUI _itemCountText = null;

    private InventoryUI _inventoryUI = null;
    public Item Item;
    public string Name;
    public int Count;

    public void Initialize(InventoryUI inventoryUI)
    {
        _inventoryUI = inventoryUI;
    }

    public void UpdateUI(Item item, int count)
    {
        Item = item;
        Name = item.Name;
        Count = count;
        _itemNameText.text = item.Name;
        _itemIcon.sprite = item.Sprite;
        _itemCountText.text = count.ToString();
        if (Count > 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Select()
    {
        _inventoryUI?.SelectItem(Item);
    }
}
