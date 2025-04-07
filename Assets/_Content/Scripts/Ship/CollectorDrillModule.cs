using UnityEngine;

public class CollectorDrillModule : ShipModule
{
    private const int _hitBufferSize = 20;

    public float CollectionRange = 25f;
    public float DrillSpeed = 1f;
    public float RotationSpeed = 5f;

    public float ScanTime = 1f;
    public int ScanTextureWidth = 90;
    public int ScanTextureHeight = 50;
    public float ScanValueMultiplier = 1f;
    public float ScanNoise = 4f;
    public float MaxShipSpeed = 0.2f;

    [SerializeField] private LayerMask _detectionLayerMask;
    public float MinAngle { get; private set; } = -45f;
    public float MaxAngle { get; private set; } = 45f;

    public Transform CollectionOrigin;
    public Transform Drill;

    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[_hitBufferSize];

    private Interactable _interactable;
    private CollectionMinigameUI _collectionMinigameUI;

    public bool DrillExtended => DrillDepth > 0.6f;
    public float DrillDepth { get; private set; } = 0.5f;
    public float DrillAngle { get; private set; } = 0f;
    public Texture2D ScanTexture { get; private set; }

    private int _currentScanIndex = 0;
    private float _currentScanProgress = 0f;
    private float _targetScanProgress = 0f;

    /// <summary>
    /// Current ore recovery efficiency
    /// </summary>
    public float DrillingEfficiency
    { 
        get
        {
            return IsActive ? 
                GetModifiedValue(ModifierStatType.ModuleEfficiency, CoreFunctionEfficiency * OperationalEfficiency) 
                : 0f;
        }
    }

    /// <summary>
    /// Collects ore!
    /// </summary>
    public void CollectOre(Ore ore)
    {
        float oreCollected = ore.Value * DrillingEfficiency;
		ShipSystemsManager.Instance.StorageModule.StoredItems[ore.Item.Name] += Mathf.FloorToInt(oreCollected);
        ShipSystemsManager.Instance.StorageModule.StoredItemsUIDirty = true;
	}

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _collectionMinigameUI = _interactable.InteractionUI.GetComponent<CollectionMinigameUI>();

        ScanTexture = new Texture2D(ScanTextureWidth, ScanTextureHeight);
        ScanTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < ScanTextureWidth; x++)
        {
            for (int y = 0; y < ScanTextureHeight; y++)
            {
                ScanTexture.SetPixel(x, y, Color.black);
            }
        }
        ScanTexture.Apply();
    }

	private void FixedUpdate()
	{
		UpdateUI();
	}

	private void Update()
    {
        if (_interactable.Interacting && IsActive)
        {
            ControlProcess();
            ScanningProcess();
        }
        else
        {
            DrillDepth -= Time.deltaTime * DrillSpeed * OperationalEfficiency;
        }

        DrillDepth = Mathf.Clamp(DrillDepth, 0.5f, CollectionRange);
        DrillAngle = Mathf.Clamp(DrillAngle, MinAngle, MaxAngle);

        Drill.localPosition = new Vector3(0f, DrillDepth, 0f);
        CollectionOrigin.localRotation = Quaternion.Euler(0f, 0f, DrillAngle);
    }

    private void ControlProcess()
    {
        if (Input.GetKey(KeyCode.W))
        {
            DrillDepth += Time.deltaTime * DrillSpeed * OperationalEfficiency;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            DrillDepth -= Time.deltaTime * DrillSpeed * OperationalEfficiency;
        }

        if (!DrillExtended)
        {
            if (Input.GetKey(KeyCode.A))
            {
                DrillAngle += Time.deltaTime * RotationSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                DrillAngle -= Time.deltaTime * RotationSpeed;
            }
        }
    }

    private void ScanningProcess()
    {
        _targetScanProgress += Time.deltaTime / ScanTime;

        
        while (_currentScanProgress < _targetScanProgress && _currentScanIndex < ScanTextureWidth)
        {
            Unity.Mathematics.float4 noise = new Unity.Mathematics.float4(Random.value, Random.value, Random.value, Random.value);
            noise *= noise * ScanNoise;
            Unity.Mathematics.float4 value = ScanRay(MinAngle + _currentScanIndex * (MaxAngle - MinAngle) / ScanTextureWidth) * ScanValueMultiplier + noise;

            for (int y = 0; y < ScanTextureHeight; y++)
            {
                Color color = new Color(y < value.x ? 1f : 0f, y < value.y ? 1f : 0f, y < value.z ? 1f : 0f);
                ScanTexture.SetPixel(ScanTextureWidth - _currentScanIndex, y, y < value.w ? Color.white : color);
            }

            ScanTexture.Apply();

            _currentScanIndex++;
            _currentScanProgress += 1f / ScanTextureWidth;
        }

        if (_currentScanIndex >= ScanTextureWidth)
        {
            _currentScanIndex = 0;
            _currentScanProgress = 0f;
            _targetScanProgress = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CollectionRange);
        Gizmos.color = Color.red;
        Vector3 direction = Quaternion.Euler(0, 0, MinAngle) * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + direction * CollectionRange);
        direction = Quaternion.Euler(0, 0, MaxAngle) * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + direction * CollectionRange);
    }

    private Unity.Mathematics.float4 ScanRay(float angle)
    {
        Unity.Mathematics.float4 output = default;

        Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
        int hits = Physics2D.RaycastNonAlloc(CollectionOrigin.position, direction, _hitBuffer, CollectionRange, _detectionLayerMask);

        for (int i = 0; i < hits; i++)
        {
            Ore ore = _hitBuffer[i].collider.GetComponent<Ore>();
            if (ore != null)
            {
                switch (ore.Item.OreType)
                {
                    case OreType.Metallic:
                        output.x += Mathf.Abs(_hitBuffer[i].normal.y);
                        break;
                    case OreType.Crystal:
                        output.y += Mathf.Abs(_hitBuffer[i].normal.y);
                        break;
                    case OreType.Liquid:
                        output.z += Mathf.Abs(_hitBuffer[i].normal.y);
                        break;
                    default:
                        output.w += Mathf.Abs(_hitBuffer[i].normal.y);
                        break;
                }
            }
        }

        return output;
    }
}
