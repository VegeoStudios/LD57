using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public int Tier;
    public Sprite Sprite;

    public int CoolingValue = -1;
    public int FuelValue = -1;

    public Dictionary<Item, int>[] CraftingRecipe;
    public int CraftingTier = -1;
}
