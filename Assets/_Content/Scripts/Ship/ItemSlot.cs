using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
	public Item SlottedItem = null;
	/// <summary>
	/// Describes what items are allowed in this slot.
	/// </summary>
	public ItemType AllowedItems = ItemType.None;
	#endregion Properties

	#region References
	[SerializeField]
	private SpriteRenderer _spriteRenderer = null;
	[SerializeField]
	private Image _image = null;
    #endregion References

    #region Events
	private void OnEnable()
	{
		UpdateSprite();
	}
    #endregion Events

    #region Methods
    /// <summary>
    /// Determines if the passed item can be received by this slot.
    /// </summary>
    public bool CanReceiveItem(Item item)
	{
		return item == null || ((AllowedItems & item.ItemType) > 0);
	}
	/// <summary>
	/// Removes the slotted item if it exists.
	/// </summary>
	/// <returns>The item that was slotted.</returns>
	public Item RemoveSlottedItem()
	{
		Item removed = SlottedItem;
		SlottedItem = null;

		UpdateSprite();

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

			UpdateSprite();

            return true;
		}

		return false;
	}

    /// <summary>
    /// Attempts to swap the item in this slot with the item in the player item slot.
    /// </summary>
    public virtual void AttemptSwapWithPlayer()
	{
        if (PlayerItemSlot.Instance.CanReceiveItem(SlottedItem) && CanReceiveItem(PlayerItemSlot.Instance.SlottedItem))
		{
            Item temp = SlottedItem;
			SlottedItem = PlayerItemSlot.Instance.SlottedItem;
			PlayerItemSlot.Instance.SlottedItem = temp;
			UpdateSprite();
			PlayerItemSlot.Instance.UpdateSprite();
		}
	}

	protected void UpdateSprite()
	{
        if (_spriteRenderer != null)
        {
            if (SlottedItem != null)
            {
                _spriteRenderer.sprite = SlottedItem.Sprite;
                _spriteRenderer.enabled = true;
            }
            else
            {
                _spriteRenderer.sprite = null;
                _spriteRenderer.enabled = false;
            }
        }

        if (_image != null)
        {
            if (SlottedItem != null)
            {
                _image.sprite = SlottedItem.Sprite;
                _image.enabled = true;
            }
            else
            {
                _image.sprite = null;
                _image.enabled = false;
            }
        }
    }
	#endregion Methods
}
