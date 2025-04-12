using UnityEngine;

public class JettisonHatch : MonoBehaviour
{
    [SerializeField] private ItemSlot _inputSlot;
    [SerializeField] private AudioSource _jettisonSound;

    private void FixedUpdate()
    {
        if (_inputSlot.SlottedItem != null)
        {
            _jettisonSound.Play();
			if (_inputSlot.SlottedItem.ItemType == ItemType.None &&
                _inputSlot.SlottedItem.Tier == ShipSystemsManager.Instance.CurrentTier)
            {
                VictoryScreen.StartVictoryScreen();
                return;
            }
            else
            {
                _inputSlot.SlottedItem = null;
                _inputSlot.UpdateSprite();
            }
        }
    }
}
