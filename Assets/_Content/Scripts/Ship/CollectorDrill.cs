using UnityEngine;

public class CollectorDrill : MonoBehaviour
{
    [SerializeField] private LayerMask _oreLayerMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ore"))
        {
            other.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
            other.transform.SetParent(transform);
        }
    }
}
