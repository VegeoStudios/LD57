using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] private GameObject _craftingentryPrefab;
    [SerializeField] private Transform _craftingEntryParent;

    private CraftingEntry[] _craftEntries;

    private void Awake()
    {
        _craftEntries = new CraftingEntry[ShipSystemsManager.Instance.FoundryModule.AvailableRecipes.Count];

        int i = 0;
        foreach (CraftingRecipe recipe in ShipSystemsManager.Instance.FoundryModule.AvailableRecipes)
        {
            GameObject itemEntry = Instantiate(_craftingentryPrefab, _craftingEntryParent);
            CraftingEntry entry = itemEntry.GetComponent<CraftingEntry>();
            entry.Initialize(this, recipe);
            _craftEntries[i] = entry;
            i++;
        }
    }

    private void Update()
    {
        if (ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty)
        {
            UpdateRecipes();
        }
    }

    private void UpdateRecipes()
    {
        
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        Debug.Log($"Selected recipe: {recipe.Result.Item.Name}");
    }
}
