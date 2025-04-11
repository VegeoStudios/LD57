using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float MovementSpeed = 4f;
    public float Acceleration = 15f;
    public float Deceleration = 20f;

    [SerializeField] private LayerMask _groundLayer;

    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 MovementInput {  get; private set; }
    public bool IsMoving => MovementInput != Vector2.zero;
    public bool CanMove { get; private set; } = true;

    public List<MonoBehaviour> MovementRestrictors = new List<MonoBehaviour>();

    private PlayerInput _playerInput;

    private Collider2D _currentPlatformCollider;

    private Vector2 _velocity;
    private float _colliderRadius;
    private RaycastHit2D[] _hits;

    private void Awake()
    {
        Instance = this;

        Rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        _colliderRadius = GetComponent<CircleCollider2D>().radius;

        _hits = new RaycastHit2D[4];
    }

    public void AddMovementRestrictor(MonoBehaviour restrictor)
    {
        if (!MovementRestrictors.Contains(restrictor))
            MovementRestrictors.Add(restrictor);
        UpdateMovementCondition();
    }

    public void RemoveMovementRestrictor(MonoBehaviour restrictor)
    {
        if (MovementRestrictors.Contains(restrictor))
            MovementRestrictors.Remove(restrictor);
        UpdateMovementCondition();
    }

    private void UpdateMovementCondition()
    {
        if (MovementRestrictors.Count > 0)
        {
            DisableMovement();
        }
        else
        {
            EnableMovement();
        }
    }

    public void DisableMovement()
    {
        CanMove = false;
        MovementInput = Vector2.zero;
    }

    public void EnableMovement()
    {
        CanMove = true;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        HandlePlatformCheck();
        CalcMovement(dt);
        HandleCollisions(dt);

        transform.position += (Vector3)_velocity * dt;
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

    private void HandleCollisions(float dt)
    {
        int count = Rigidbody.Cast(_velocity.normalized, _hits, _velocity.magnitude * dt);

        for (int i = 0; i < count; i++)
        {
            Vector2 normal = _hits[i].normal;
            float dot = Vector2.Dot(_velocity, normal);
            if (dot < 0)
            {
                _velocity -= normal * dot;
            }
        }
    }

    private void CalcMovement(float dt)
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
    }

    public void OnMovement(InputValue value)
    {
        if (!CanMove)
            return;
        MovementInput = value.Get<Vector2>();
    }

    private void ResolveCollisions(Collision2D collision)
    {

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            Vector2 normal = contact.normal;

            float dot = Vector2.Dot(_velocity, normal);

            if (dot < 0)
            {
                _velocity -= normal * dot;
            }
        }
    }

    /*
    public void OnCollisionEnter2D(Collision2D collision)
    {
        ResolveCollisions(collision);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        ResolveCollisions(collision);
    }
    */
}
