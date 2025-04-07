using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject _recipeEntryPrefab;
    [SerializeField] private Transform _recipeEntryParent;
    [SerializeField] private GameObject _ingredientEntryPrefab;
    [SerializeField] private Transform _ingredientEntryParent;

    private RecipeEntry[] _recipeEntries;
    private IngredientEntry[] _ingredientEntries;

    private RecipeEntry _selectedRecipeEntry;

    private void Start()
    {
        _recipeEntries = new RecipeEntry[ShipSystemsManager.Instance.FoundryModule.AvailableRecipes.Count];

        int i = 0;
        foreach (CraftingRecipe recipe in ShipSystemsManager.Instance.FoundryModule.AvailableRecipes)
        {
            GameObject itemEntry = Instantiate(_recipeEntryPrefab, _recipeEntryParent);
            RecipeEntry entry = itemEntry.GetComponent<RecipeEntry>();
            entry.Initialize(this, recipe);
            _recipeEntries[i] = entry;
            i++;
        }

        _ingredientEntries = new IngredientEntry[6];

        for (int j = 0; j < _ingredientEntries.Length; j++)
        {
            GameObject obj = Instantiate(_ingredientEntryPrefab, _ingredientEntryParent);
            IngredientEntry entry = obj.GetComponent<IngredientEntry>();
            entry.Initialize(this);
            _ingredientEntries[j] = entry;
        }
    }

    private void Update()
    {
        if (ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _recipeEntries.Length; i++)
        {
            _recipeEntries[i].gameObject.SetActive(_recipeEntries[i].Recipe.CanCraft);
        }

        for (int j = 0; j < _ingredientEntries.Length; j++)
        {
            if (_selectedRecipeEntry != null && _selectedRecipeEntry.Recipe.Ingredients.Count > j)
            {
                Item item = _selectedRecipeEntry.Recipe.Ingredients[j].Item;
                int count = _selectedRecipeEntry.Recipe.Ingredients[j].Amount;
                _ingredientEntries[j].UpdateUI(item, count);
                _ingredientEntries[j].gameObject.SetActive(true);
            }
            else
            {
                _ingredientEntries[j].gameObject.SetActive(false);
            }
        }
    }

    public void SelectRecipe(RecipeEntry recipe)
    {
        _selectedRecipeEntry = recipe;
        UpdateUI();
    }

    public void BeginCraft()
    {
        if (_selectedRecipeEntry == null)
            return;
        if (_selectedRecipeEntry.Recipe.CanCraft)
            ShipSystemsManager.Instance.FoundryModule.Craft(_selectedRecipeEntry.Recipe);
    }
}
