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
	void OnEnable()
	{
		UpdateSprite();
	}

	void Start()
	{
		if (IsPlayerItemSlot)
		{
			_instance = this;
		}

        SlottedItem = StartingItem;
    }
	#endregion Events

	#region Methods
	/// <summary>
	/// Determines if the passed item can be received by this slot.
	/// </summary>
	public virtual bool CanReceiveItem(Item item)
	{
		/*
		if (IsCoreSlot)
		{
			if (item == null) return false;
			return item.ItemType == ItemType.TierCore &&
                (SlottedItem == null || (SlottedItem.Tier < item.Tier));
        }
		return item == null || ((AllowedItems & item.ItemType) > 0);
		*/
        
		return AllowedItems.HasFlag(item.ItemType) &&
			(SlottedItem == null || (IsCoreSlot && item.Tier > SlottedItem.Tier));
		
    }
    /// <summary>
    /// Attempts to swap the item in this slot with the item in the player item slot.
    /// </summary>
    public virtual void AttemptSwapWithPlayer()
	{
		/*
		Item itemToGive = IsCoreSlot ? null : SlottedItem;
		Debug.Log("PlayerItemSlot.CanReceiveItem(itemToGive): " + PlayerItemSlot.CanReceiveItem(itemToGive));
        Debug.Log("CanReceiveItem(PlayerItemSlot.SlottedItem): " + CanReceiveItem(PlayerItemSlot.SlottedItem));
        if (PlayerItemSlot.CanReceiveItem(itemToGive) && CanReceiveItem(PlayerItemSlot.SlottedItem))
        {
            Item temp = itemToGive;
            SlottedItem = PlayerItemSlot.SlottedItem;
            PlayerItemSlot.SlottedItem = temp;
            UpdateSprite();
            PlayerItemSlot.UpdateSprite();
        }
		*/
        
		/*

        if (SlottedItem == null)
		{
			// This slot is empty, check if we can take player's item.
			if (PlayerItemSlot.SlottedItem != null && CanReceiveItem(PlayerItemSlot.SlottedItem))
			{
				SlottedItem = PlayerItemSlot.SlottedItem;
				PlayerItemSlot.SlottedItem = null;
			}
		}
		else if (IsCoreSlot)
		{
			// This is a core slot! Cores are destroyed when replaced.
			if (PlayerItemSlot.SlottedItem != null &&
				PlayerItemSlot.CanReceiveItem(SlottedItem) &&
				CanReceiveItem(PlayerItemSlot.SlottedItem))
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

		*/
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
