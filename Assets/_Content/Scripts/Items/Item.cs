using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public int Tier;
    public Sprite Sprite;

    public ItemFlags Flags;

    public float CoolantValue = -1;
    public float FuelValue = -1;
    public float UpgradeValue = -1;

    [Flags]
    public enum ItemFlags
    {
        None = 1,
        Raw = 2,
        Crafted = 4,
        Coolant = 8,
        Fuel = 16,
        GenericUpgrade = 32,
        ReactorUpgrade = 64,
        HelmUpgrade = 128,
        DrillUpgrade = 512,
        CoolingUpgrade = 1024
    }
}
