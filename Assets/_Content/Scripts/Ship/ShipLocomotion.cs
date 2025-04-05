using UnityEngine;

public class ShipLocomotion : MonoBehaviour
{
    public float Speed;

    private void Update()
    {
        transform.position += transform.right * Speed * Time.deltaTime;
    }
}
