using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Describes what kind of item this is.
/// </summary>
[Flags]
public enum ItemType
{
	None = 0,
	Raw = 1,
	Crafted = 2,
	Coolant = 4,
	Fuel = 8,
	GenericModifier = 16,
	ReactorModifier = 32,
	HelmModifier = 64,
	DrillModifier = 128,
	CoolingModifier = 256,
}

/// <summary>
/// Items are carried by the player and used for crafting.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
[Serializable]
public abstract class Item : ScriptableObject
{
	#region Fields
	// UI
	public string Name;
    public string Description;
    public int Tier;
    public Sprite Sprite;

	// Stats
    public ItemType ItemType;
    public float CoolantValue = 0; // kWh
    public float FuelValue = 0; // kWh
	public List<Modifier> Modifiers = new List<Modifier>();
	#endregion Fields
}
