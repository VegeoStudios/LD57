using UnityEngine;

public class OreObjectPool : ObjectPool<Ore>
{
    public static OreObjectPool Instance { get; private set; }

    public Sprite[] OreSprites;
    public float[] ColliderRadii;

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

        InitializePool();
    }

    public Ore SpawnOre(Vector3 position, float value, Item item)
    {
        Ore obj = GetObject();

        position.z = 2f;
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        obj.Value = value;
        obj.Item = item;
        int tierIndex = Mathf.Clamp(Mathf.FloorToInt(Mathf.Lerp(0, OreSprites.Length, value / 6)), 0, OreSprites.Length - 1);
        obj.GetComponent<SpriteRenderer>().sprite = OreSprites[tierIndex];
        obj.GetComponent<CircleCollider2D>().radius = ColliderRadii[tierIndex];
        obj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        return obj;
    }
}
