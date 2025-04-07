using UnityEngine;

public class CollectorDrill : MonoBehaviour
{
    [SerializeField] private LayerMask _oreLayerMask;
    [SerializeField] private CollectorDrillModule _collectorDrillModule;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collectorDrillModule.IsActive) return;

        if (other.CompareTag("Ore"))
        {
            other.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
            other.transform.SetParent(transform);
        }
    }
}
