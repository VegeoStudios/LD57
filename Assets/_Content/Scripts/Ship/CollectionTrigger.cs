using UnityEngine;

public class CollectionTrigger : MonoBehaviour
{
    private CollectorDrillModule _collectorDrillModule;

    private void Awake()
    {
        _collectorDrillModule = GetComponentInParent<CollectorDrillModule>();
        if (_collectorDrillModule == null)
        {
            Debug.LogError("CollectorDrillModule not found in parent.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ore"))
        {
            Ore ore = other.GetComponent<Ore>();
            if (ore != null)
            {
                _collectorDrillModule.CollectOre(ore);
                OreObjectPool.Instance.ReturnObject(ore);
            }
        }
    }
}
