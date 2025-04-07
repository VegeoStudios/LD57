using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItemEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText = null;
    [SerializeField] private Image _itemIcon = null;
    [SerializeField] private TextMeshProUGUI _itemCountText = null;

    private RecipeUI _recipeUI = null;
    public Item Item;
    public string Name;
    public string Count;

    public void Initialize(RecipeUI recipeUI)
    {
        _recipeUI = recipeUI;
    }

    public void UpdateUI(Item item, int count = 0)
    {
        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Item = item;
        Name = item.Name;
        Count = count + "/" + ShipSystemsManager.Instance.StorageModule.StoredItems[item.Name];
        _itemNameText.text = item.Name;
        _itemIcon.sprite = item.Sprite;
        _itemCountText.text = Count;
    }
}
