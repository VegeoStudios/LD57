using UnityEngine;

public class OreObjectPool : ObjectPool<Ore>
{
    public static OreObjectPool Instance { get; private set; }

    public Sprite[] MetalSprites;
    public Sprite[] CrystalSprites;
    public Sprite[] LiquidSprites;
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

        Sprite[] sprites;
        switch (item.OreType)
        {
            case OreType.Crystal:
                sprites = CrystalSprites;
                break;
            case OreType.Metallic:
                sprites = MetalSprites;
                break;
            case OreType.Liquid:
                sprites = LiquidSprites;
                break;
            default:
                sprites = MetalSprites;
                return null;
        }

        obj.Value = value;
        obj.Item = item;
        int tierIndex = Mathf.Clamp(Mathf.FloorToInt(Mathf.Lerp(0, ColliderRadii.Length, value / 6)), 0, ColliderRadii.Length - 1);
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[tierIndex];
        spriteRenderer.color = item.Color;
        obj.GetComponent<CircleCollider2D>().radius = ColliderRadii[tierIndex];
        obj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        return obj;
    }
}
