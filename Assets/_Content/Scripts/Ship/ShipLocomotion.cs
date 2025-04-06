using UnityEngine;

public class ShipLocomotion : MonoBehaviour
{
    public Rigidbody2D Head;
    public ChainLink[] Bodies;

    protected float _speed;
    public float TargetRotation;
    public float TargetSpeed;
    public float MinBodyDistance;
    public float MaxBodyDistance;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            TargetRotation = 20;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            TargetRotation = -20;
        }
        else
        {
            TargetRotation = 0;
        }

        UpdateSpeed(Time.fixedDeltaTime);
        DoMovement(Time.fixedDeltaTime);
    }

    private void UpdateSpeed(float dt)
    {
        _speed = Mathf.Lerp(_speed, TargetSpeed, dt);
    }

    private void DoMovement(float dt)
    {
        Vector3 lastPosition = Head.position;

        // Head movement
        Head.MoveRotation(Quaternion.RotateTowards(Head.transform.rotation, Quaternion.Euler(0, 0, TargetRotation), _speed * dt));
        Head.MovePosition(Head.transform.position + Head.transform.right * _speed * dt);

        // Body movement
        for (int i = 0; i < Bodies.Length; i++)
        {
            ChainLink body = Bodies[i];
            Vector3 target = body.TargetPosition.position;
            //body.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross((target - body.transform.position).normalized, Vector3.back));
            body.Rigidbody.MoveRotation(Quaternion.RotateTowards(body.transform.rotation, Quaternion.LookRotation(Vector3.forward, Vector3.Cross((target - body.transform.position).normalized, Vector3.back)), _speed * dt));
            target -= body.transform.right * MinBodyDistance;
            float dist = Vector3.Distance(body.transform.position, target);
            //body.transform.position = Vector3.MoveTowards(body.transform.position, target, Mathf.Lerp(0f, dist, dist / MaxBodyDistance));
            body.Rigidbody.MovePosition(Vector3.MoveTowards(body.transform.position, target, Mathf.Lerp(0f, dist, dist / MaxBodyDistance)));
        }
    }
}
