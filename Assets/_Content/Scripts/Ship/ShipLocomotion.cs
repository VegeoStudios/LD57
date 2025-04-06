using UnityEngine;

public class ShipLocomotion : MonoBehaviour
{
    public Rigidbody2D Head;
    public ChainLink[] Bodies; 
    public float Speed;
    public float RotationSpeed;
    public float TargetRotation;
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

        DoMovement(Time.fixedDeltaTime);
    }

    private void DoMovement(float dt)
    {
        Vector3 lastPosition = Head.position;

        // Head movement
        //Head.rotation = Quaternion.RotateTowards(Head.rotation, Quaternion.Euler(0, 0, TargetRotation), Speed * RotationSpeed * Time.deltaTime);
        //Head.position += Head.right * Speed * Time.deltaTime;
        Head.MoveRotation(Quaternion.RotateTowards(Head.transform.rotation, Quaternion.Euler(0, 0, TargetRotation), Speed * RotationSpeed * dt));
        Head.MovePosition(Head.transform.position + Head.transform.right * Speed * dt);

        // Body movement
        for (int i = 0; i < Bodies.Length; i++)
        {
            ChainLink body = Bodies[i];
            Vector3 target = body.TargetPosition.position;
            //body.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross((target - body.transform.position).normalized, Vector3.back));
            body.Rigidbody.MoveRotation(Quaternion.RotateTowards(body.transform.rotation, Quaternion.LookRotation(Vector3.forward, Vector3.Cross((target - body.transform.position).normalized, Vector3.back)), Speed * RotationSpeed * dt));
            target -= body.transform.right * MinBodyDistance;
            float dist = Vector3.Distance(body.transform.position, target);
            //body.transform.position = Vector3.MoveTowards(body.transform.position, target, Mathf.Lerp(0f, dist, dist / MaxBodyDistance));
            body.Rigidbody.MovePosition(Vector3.MoveTowards(body.transform.position, target, Mathf.Lerp(0f, dist, dist / MaxBodyDistance)));
        }
    }
}
