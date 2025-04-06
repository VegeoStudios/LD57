using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _playerController;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponentInParent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Reset the rotation of the player visual to prevent it from rotating with the player
        transform.rotation = Quaternion.identity;

        // Set the animator's Y direction
        if (_playerController != null)
        {
            _animator.SetBool("Walking", _playerController.IsMoving);
            if (_playerController.IsMoving)
            {
                _spriteRenderer.flipX = _playerController.MovementInput.x < 0;
                _animator.SetFloat("DirectionY", _playerController.MovementInput.y);
            }
        }
    }
}
