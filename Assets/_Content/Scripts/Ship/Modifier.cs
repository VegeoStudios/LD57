using System;
using UnityEngine;

/// <summary>
/// Defines the type of stat this object applies a modifier to.
/// </summary>
[Flags]
public enum ModifierStatType
{ 
    None = 0,
    ModulePowerDemand = 1,
    ModuleHeatProduction = 2,
    ModuleEfficiency = 4,
}

/// <summary>
/// Defines how this mod is applied to a value.
/// </summary>
[Flags]
public enum ModifierType
{ 
    /// <summary>
    /// Adds the magnitude to the value
    /// </summary>
    Additive = 0,
    /// <summary>
    /// Adds a percent of the value to itself
    /// </summary>
    AdditivePercent = 1,
    /// <summary>
    /// Multiplies by the value
    /// </summary>
    Multiplicative = 2,
    /// <summary>
    /// Value override
    /// </summary>
    Absolute = 4,
}

/// <summary>
/// Modifier class to apply changes to ship stats.
/// </summary>
[Serializable]
public class Modifier : MonoBehaviour
{
	#region Fields
	/// <summary>
	/// The amount of modifier to apply. For percents, use decimals.
	/// </summary>
	public float Magnitude;
    /// <summary>
    /// Which stat is affected by this mod.
    /// </summary>
    public ModifierStatType ModifiedStat;
    /// <summary>
    /// Type of modifier this is.
    /// </summary>
    public ModifierType ModifierType;
    #endregion Fields

    #region Methods
    /// <summary>
    /// Applies the stored modifier to the passed value.
    /// </summary>
    public float GetModifiedValue(float value)
    {
        switch (ModifierType)
        {
            case ModifierType.Additive:
                {
                    return value + Magnitude;
                }
			case ModifierType.AdditivePercent:
				{
                    return value + (value * Magnitude);
				}
			case ModifierType.Multiplicative:
				{
                    return value * Magnitude;
				}
            case ModifierType.Absolute:
                {
                    return Magnitude;
                }
            default:
                {
                    return value;
                }
		}
    }
	#endregion Methods
}