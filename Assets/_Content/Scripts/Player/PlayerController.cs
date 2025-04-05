using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 4f;
    public float Acceleration = 15f;
    public float Deceleration = 20f;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 MovementInput {  get; private set; }
    public bool IsMoving => MovementInput != Vector2.zero;
    public bool CanMove => _playerInput.enabled;

    private PlayerInput _playerInput;

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
        if (IsMoving)
        {
            Vector2 targetVelocity = MovementInput * MovementSpeed;
            Vector2 velocity = Rigidbody.linearVelocity;
            float acceleration = velocity.magnitude > 0 ? Acceleration : Deceleration;
            Rigidbody.linearVelocity = Vector2.MoveTowards(velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            Rigidbody.linearVelocity = Vector2.MoveTowards(Rigidbody.linearVelocity, Vector2.zero, Deceleration * Time.fixedDeltaTime);
        }
    }

    public void OnMovement(InputValue value)
    {
        if (!CanMove)
            return;
        MovementInput = value.Get<Vector2>();
    }
}
