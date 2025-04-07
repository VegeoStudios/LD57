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
	Coolant = 1,
	Fuel = 2,
	Upgrade = 4,
	TierCore = 8,
	Raw = 16,
}

/// <summary>
/// Describes what kind of ore this is, if applicable.
/// </summary>
public enum OreType
{ 
	None = 0,
	Crystal = 1,
	Metallic = 2,
	Liquid = 4,
}

/// <summary>
/// Items are carried by the player and used for crafting.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
[Serializable]
public class Item : ScriptableObject
{
	#region Fields
	// UI
	public string Name;
    public string Description;
    public int Tier;
    public Sprite Sprite;
	public Color Color;

	// Stats
	public OreType OreType = OreType.None;
	public ItemType ItemType;
    public float CoolantValue = 0; // kWh
    public float FuelValue = 0; // kWh
	public List<Modifier> Modifiers = new List<Modifier>();
	#endregion Fields
}
