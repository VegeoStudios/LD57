using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Describes what kind of item this is.
/// </summary>
[Flags]
public enum ItemType
{
	Raw = 0,
	Coolant = 1,
	Fuel = 2,
	Upgrade = 4,
	TierCore = 8,
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

	// Stats
    public ItemType ItemType;
    public float CoolantValue = 0; // kWh
    public float FuelValue = 0; // kWh
	public List<Modifier> Modifiers = new List<Modifier>();
	#endregion Fields
}
