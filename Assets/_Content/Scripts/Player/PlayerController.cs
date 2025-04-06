using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 4f;
    public float Acceleration = 15f;
    public float Deceleration = 20f;

    [SerializeField] private LayerMask _groundLayer;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 MovementInput {  get; private set; }
    public bool IsMoving => MovementInput != Vector2.zero;
    public bool CanMove => _playerInput.enabled;

    private PlayerInput _playerInput;

    private Collider2D _currentPlatformCollider;

    private Vector2 _velocity;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    public void DisableMovement()
    {
        _playerInput.enabled = false;
        MovementInput = Vector2.zero;
    }

    public void EnableMovement()
    {
        _playerInput.enabled = true;
    }

    private void FixedUpdate()
    {
        HandlePlatformCheck();
        DoMovement(Time.deltaTime);
    }

    private void HandlePlatformCheck()
    {
        // parent transform to the ground
        Collider2D groundCollider = Physics2D.OverlapPoint(transform.position, _groundLayer);

        if (groundCollider == _currentPlatformCollider)
            return;

        _currentPlatformCollider = groundCollider;

        transform.SetParent(_currentPlatformCollider?.transform, true);
    }

    private void DoMovement(float dt)
    {
        transform.localRotation = Quaternion.identity;
        if (IsMoving)
        {
            Vector2 targetVelocity = transform.TransformDirection(MovementInput) * MovementSpeed;
            float acceleration = _velocity.magnitude > 0 ? Acceleration : Deceleration;
            _velocity = Vector2.MoveTowards(_velocity, targetVelocity, acceleration * dt);
        }
        else
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, Deceleration * dt);
        }

        Rigidbody.MovePosition(Rigidbody.position + _velocity * dt);
    }

    public void OnMovement(InputValue value)
    {
        if (!CanMove)
            return;
        MovementInput = value.Get<Vector2>();
    }
}
