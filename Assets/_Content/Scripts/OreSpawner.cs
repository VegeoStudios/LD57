using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    public static OreSpawner Instance;

    public OreTableEntry[] OreTable;

    public float SpawnRate = 1f;

    public float DeletionDistance = 60f;

    private float _lastXPos;
    private BoxCollider2D _collider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("BoxCollider2D not found on OreSpawner.");
        }
    }

    private void Start()
    {
        _lastXPos = transform.position.x;
    }

    private void FixedUpdate()
    {
        float diff = transform.position.x - _lastXPos;

        float rand = Random.value;
        float threshold = Mathf.Clamp01(SpawnRate * diff);

        //Debug.Log($"Random: {rand}, Threshold: {threshold}, Diff: {diff}");

        if (rand < threshold)
        {
            SpawnOre();
        }

        _lastXPos = transform.position.x;
    }

    private void SpawnOre()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            2f
        );

        Item item = null;
        float totalWeight = 0f;
        foreach (OreTableEntry entry in OreTable)
        {
            totalWeight += entry.Weight;
        }
        float randomValue = Random.Range(0f, totalWeight);
        foreach (OreTableEntry entry in OreTable)
        {
            if (randomValue < entry.Weight)
            {
                item = entry.Item;
                break;
            }
            randomValue -= entry.Weight;
        }

        if (item != null)
        {
            float value = 1f;
            OreObjectPool.Instance.SpawnOre(spawnPosition, value, item);
        }
        else
        {
            Debug.LogError("No item found for spawning.");
        }
    }

    [System.Serializable]
    public struct OreTableEntry
    {
        public Item Item;
        public float Weight;
    }
}
