using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Item slots are members of <see cref="ShipModule"/>s and store <see cref="Item"/>s.
/// </summary>
[Serializable]
public class ItemSlot : MonoBehaviour
{
	#region Fields
	/// <summary>
	/// The currently slotted item object, if any.
	/// </summary>
	public Item SlottedItem = null;
	/// <summary>
	/// Describes what items are allowed in this slot.
	/// </summary>
	public ItemType AllowedItems = ItemType.Upgrade;
	#endregion Fields

	#region References
	[SerializeField]
	protected SpriteRenderer _spriteRenderer = null;
	[SerializeField]
	protected Image _image = null;
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
    public virtual bool CanReceiveItem(Item item)
	{
		return (SlottedItem is null) && ((AllowedItems & item.ItemType) > 0);
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

	/// <summary>
	/// Updates the shown sprite
	/// </summary>
	public void UpdateSprite()
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
