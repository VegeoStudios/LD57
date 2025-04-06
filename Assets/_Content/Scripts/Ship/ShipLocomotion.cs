using UnityEngine;

public class ShipLocomotion : MonoBehaviour
{
    public Transform Head;
    public ChainLink[] Bodies; 
    public float Speed;
    public float RotationSpeed;
    public float TargetRotation;
    public float MinBodyDistance;
    public float MaxBodyDistance;

    private void Update()
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

        DoMovement();
    }

    private void DoMovement()
    {
        Vector3 lastPosition = Head.position;

        // Head movement
        Head.rotation = Quaternion.RotateTowards(Head.rotation, Quaternion.Euler(0, 0, TargetRotation), Speed * RotationSpeed * Time.deltaTime);
        Head.position += Head.right * Speed * Time.deltaTime;

        // Body movement
        for (int i = 0; i < Bodies.Length; i++)
        {
            ChainLink body = Bodies[i];
            Vector3 target = body.TargetPosition.position;
            body.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross((target - body.transform.position).normalized, Vector3.back));
            target -= body.transform.right * MinBodyDistance;
            float dist = Vector3.Distance(body.transform.position, target);
            body.transform.position = Vector3.MoveTowards(body.transform.position, target, Mathf.Lerp(0f, dist, dist / MaxBodyDistance));
        }
    }
}
