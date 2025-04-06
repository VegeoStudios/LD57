using UnityEngine;

public class ChainLink : MonoBehaviour
{
    public Transform TargetPosition;
    public Rigidbody2D Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        if (Rigidbody == null)
        {
            Debug.LogError("Rigidbody2D component is missing on the ChainLink object.");
        }
    }
}
