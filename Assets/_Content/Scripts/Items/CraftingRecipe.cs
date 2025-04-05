using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Items/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public CraftingComponent Result;
    public List<CraftingComponent> Ingredients = new List<CraftingComponent>();
    public float CraftingTime = 2f;

    [System.Serializable]
    public struct CraftingComponent
    {
        public Item Item;
        public int Amount;
        public CraftingComponent(Item item, int amount)
        {
            this.Item = item;
            this.Amount = amount;
        }
    }
}
