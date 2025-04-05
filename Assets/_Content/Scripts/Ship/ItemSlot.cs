using System;
using UnityEngine;

/// <summary>
/// Item slots are members of <see cref="ShipModule"/>s and store <see cref="Item"/>s.
/// </summary>
[Serializable]
public class ItemSlot : MonoBehaviour
{
	#region Properties
	/// <summary>
	/// The currently slotted item object, if any.
	/// </summary>
	public Item SlottedItem { get; protected set; } = null;
	/// <summary>
	/// Describes what items are allowed in this slot.
	/// </summary>
	public ItemType AllowedItems { get; protected set; } = ItemType.None;
	/// <summary>
	/// Reference to the UI object representing this slot.
	/// </summary>
	public GameObject ItemSlotGameObject;
	#endregion Properties

	#region Methods
	/// <summary>
	/// Determines if the passed item can be received by this slot.
	/// </summary>
	public bool CanReceiveItem(Item item)
	{
		if (AllowedItems.HasFlag(item.ItemType) && SlottedItem is null)
		{
			return true;
		}

		return false;
	}
	/// <summary>
	/// Removes the slotted item if it exists.
	/// </summary>
	/// <returns>The item that was slotted.</returns>
	public Item RemoveSlottedItem()
	{
		Item removed = SlottedItem;
		SlottedItem = null;
		return removed;
	}
	/// <summary>
	/// Attempts to insert the passed item.
	/// </summary>
	/// <returns>True if successful.</returns>
	public bool InsertItem(Item item)
	{
		if (CanReceiveItem(item))
		{
			SlottedItem = item;
			return true;
		}

		return false;
	}
	#endregion Methods
}
