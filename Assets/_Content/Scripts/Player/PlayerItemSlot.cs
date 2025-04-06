using UnityEngine;

public class PlayerItemSlot : ItemSlot
{
    public static PlayerItemSlot Instance { get; private set; }

    private void Awake()
    {
        UpdateSprite();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void AttemptSwapWithPlayer()
    {
        return;
    }
}
