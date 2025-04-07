using UnityEngine;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemEntryPrefab;
    [SerializeField] private Transform _itemEntryParent;

    private RecipeItemEntry[] _recipeItemEntries;

    private void Awake()
    {
        _recipeItemEntries = new RecipeItemEntry[5];

        for (int i = 0; i < _recipeItemEntries.Length; i++)
        {
            GameObject obj = Instantiate(_itemEntryPrefab, _itemEntryParent);
            RecipeItemEntry entry = obj.GetComponent<RecipeItemEntry>();
            entry.Initialize(this);
            _recipeItemEntries[i] = entry;
        }
    }

    public void UpdateUI(CraftingRecipe recipe)
    {
        for (int i = 0; i < _recipeItemEntries.Length; i++)
        {
            if (i < recipe.Ingredients.Count)
            {
                _recipeItemEntries[i].UpdateUI(recipe.Ingredients[i].Item, recipe.Ingredients[i].Amount);
            }
            else
            {
                _recipeItemEntries[i].UpdateUI(null);
            }
        }
    }
}
