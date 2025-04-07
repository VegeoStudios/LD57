using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText = null;
    [SerializeField] private Image _itemIcon = null;

    private CraftingUI _craftingUI = null;
    public CraftingRecipe Recipe;
    public string Name;

    public void Initialize(CraftingUI craftingUI, CraftingRecipe Recipe)
    {
        _craftingUI = craftingUI;
        this.Recipe = Recipe;
        Name = Recipe.Name;

        _itemNameText.text = Name;
        _itemIcon.sprite = Recipe.Result.Item.Sprite;
    }

    public void Select()
    {
        _craftingUI?.SelectRecipe(this);
    }
}
