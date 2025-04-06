using UnityEngine;

public class SpinMotion : MonoBehaviour
{
    [SerializeField] private float _spinSpeed = 360f; // Degrees per second

    private void Update()
    {
        // Rotate the object around its Y-axis at the specified speed
        transform.Rotate(Vector3.forward, _spinSpeed * Time.deltaTime);
    }
}
