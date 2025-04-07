using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    public static OreSpawner Instance;

    public OreTable[] Stages;

    public float SpawnRate = 1f;

    public float DeletionDistance = 60f;

    private float _lastXPos;
    private BoxCollider2D _collider;

    private int _currentStageIndex = 0;

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
        if (_currentStageIndex < Stages.Length - 1 &&
            transform.position.x > Stages[_currentStageIndex + 1].Depth)
        {
            _currentStageIndex++;
        }

        if (_currentStageIndex < 0) return; // Don't spawn ores on surface

        float diff = transform.position.x - _lastXPos;

        float rand = Random.value;
        float threshold = Mathf.Clamp01(Stages[_currentStageIndex].SpawnRate * diff);

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
        foreach (OreTableEntry entry in Stages[_currentStageIndex].Entries)
        {
            totalWeight += entry.Weight;
        }
        float randomValue = Random.Range(0f, totalWeight);
        foreach (OreTableEntry entry in Stages[_currentStageIndex].Entries)
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
            float value = Random.value;
            value *= value * 6f;
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

    [System.Serializable]
    public struct OreTable
    {
        public int Depth;
        public float SpawnRate;
        public OreTableEntry[] Entries;
    }
}
