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
	AnyModifier = GenericModifier | ReactorModifier | HelmModifier | DrillModifier | CoolingModifier,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class Item : ScriptableObject
{
	#region Fields
	public string Name;
    public string Description;
    public int Tier;
    public Sprite Sprite;

    public ItemType ItemType;

    public float CoolantValue = -1;
    public float FuelValue = -1;
	public List<Modifier> Modifiers = new List<Modifier>();
	#endregion Fields
}
