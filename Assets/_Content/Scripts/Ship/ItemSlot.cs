using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Item slots are members of <see cref="ShipModule"/>s and store <see cref="Item"/>s.
/// </summary>
[Serializable]
public class ItemSlot : MonoBehaviour
{
	#region Singleton
	protected static ItemSlot _instance = null;
	/// <summary>
	/// The item slot connected to the player.
	/// </summary>
	public static ItemSlot PlayerItemSlot
	{
		get
		{
			return _instance;
		}
	}
	#endregion Singleton

	#region Fields
	/// <summary>
	/// The currently slotted item object, if any.
	/// </summary>
	[SerializeField] private Item StartingItem = null;	
	private Item _slottedItem = null;
    public Item SlottedItem
	{
		get => _slottedItem;
        set
		{
			if (!IsPlayerItemSlot && _slottedItem != value)
			{
				// item was placed here, whoa dude!
				ShipSystemsManager.Instance.PlayUIClickSound();
			}

			_slottedItem = value;
			UpdateSprite();
        }
	}
	/// <summary>
	/// Describes what items are allowed in this slot.
	/// </summary>
	public ItemType AllowedItems = ItemType.Upgrade;
	/// <summary>
	/// Controls if this is the player's item slot or not.
	/// </summary>
	public bool IsPlayerItemSlot = false;
	#endregion Fields

	#region Properties
	/// <summary>
	/// Each module must have exactly one "core slot" to host tier upgrades.
	/// </summary>
	public bool IsCoreSlot
	{
		get
		{
			return AllowedItems == ItemType.TierCore;
		}
	}
	#endregion Properties

	#region References
	[SerializeField]
	protected SpriteRenderer _spriteRenderer = null;
	[SerializeField]
	protected Image _image = null;
    #endregion References

    #region Events
	void Start()
	{
		SlottedItem = StartingItem;
		UpdateSprite();

		if (IsPlayerItemSlot)
		{
			_instance = this;
		}	
    }
	#endregion Events

	#region Methods
	/// <summary>
	/// Determines if the passed item can be received by this slot.
	/// </summary>
	public virtual bool CanReceiveItem(Item item, bool allowSwaps = true)
	{
		if (item is null)
		{
			return false;
		}

		if (IsCoreSlot)
		{
			return item.Tier >= (SlottedItem?.Tier ?? 0);
		}

		return AllowedItems.HasFlag(item.ItemType) && (SlottedItem == null || allowSwaps);
    }
    /// <summary>
    /// Attempts to swap the item in this slot with the item in the player item slot.
    /// </summary>
    public virtual void AttemptSwapWithPlayer()
	{
        if (SlottedItem == null || IsCoreSlot)
		{
			// This slot is empty, check if we can take player's item.
			if (CanReceiveItem(PlayerItemSlot.SlottedItem))
			{
				SlottedItem = PlayerItemSlot.SlottedItem;
				PlayerItemSlot.SlottedItem = null;
			}
		}
		else if (PlayerItemSlot.SlottedItem == null)
		{
			// Player isn't carrying anything, can they pick up from this slot?
			if (PlayerItemSlot.CanReceiveItem(SlottedItem))
			{
				PlayerItemSlot.SlottedItem = SlottedItem;
				SlottedItem = null;
			}
		}
        else
        {
            if (PlayerItemSlot.CanReceiveItem(SlottedItem) && CanReceiveItem(PlayerItemSlot.SlottedItem))
            {
                Item temp = SlottedItem;
                SlottedItem = PlayerItemSlot.SlottedItem;
                PlayerItemSlot.SlottedItem = temp;
                UpdateSprite();
                PlayerItemSlot.UpdateSprite();
            }
        }

		UpdateSprite();
		PlayerItemSlot.UpdateSprite();
	}

	/// <summary>
	/// Updates the shown sprite
	/// </summary>
	public void UpdateSprite()
	{
        if (_spriteRenderer != null)
        {
            if (_slottedItem != null)
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
            if (_slottedItem != null)
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
