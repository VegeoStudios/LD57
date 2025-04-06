using System;

/// <summary>
/// Custom item slot to hold only core/tier items.
/// </summary>
[Serializable]
public class CoreSlot : ItemSlot
{
	#region Fields
	public new ItemType AllowedItems = ItemType.TierCore;
	#endregion Fields

	#region Methods
	/// <summary>
	/// Only core upgrades of a higher tier can be received.
	/// </summary>
	public override bool CanReceiveItem(Item item)
	{
		return item.Tier > SlottedItem.Tier && ((AllowedItems & item.ItemType) > 0);
	}
	/// <summary>
	/// Core upgrades are consumed when replaced.
	/// </summary>
	public override void AttemptSwapWithPlayer()
	{
		if (CanReceiveItem(PlayerItemSlot.Instance.SlottedItem))
		{
			SlottedItem = PlayerItemSlot.Instance.SlottedItem;
			PlayerItemSlot.Instance.SlottedItem = null;
			UpdateSprite();
			PlayerItemSlot.Instance.UpdateSprite();
		}
	}
	#endregion Methods
}
