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
    /// Collects ore!
    /// </summary>
    public void CollectOre(Ore ore)
    {
        float oreCollected = ore.Value * CoreFunctionEfficiency * OperationalEfficiency;
		ShipSystemsManager.Instance.StorageModule.StoredItems[ore.Item.Name] += Mathf.FloorToInt(oreCollected);
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
        if (_interactable.Interacting)
        {
            ControlProcess();
            ScanningProcess();
        }
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

        DrillDepth = Mathf.Clamp(DrillDepth, 0.5f, CollectionRange);
        DrillAngle = Mathf.Clamp(DrillAngle, MinAngle, MaxAngle);

        Drill.localPosition = new Vector3(0f, DrillDepth, 0f);
        CollectionOrigin.localRotation = Quaternion.Euler(0f, 0f, DrillAngle);
    }

    private void ScanningProcess()
    {
        _targetScanProgress += ScanTime * Time.deltaTime;

        
        while (_currentScanProgress < _targetScanProgress && _currentScanIndex < ScanTextureWidth)
        {
            float value = ScanRay(MinAngle + _currentScanIndex * (MaxAngle - MinAngle) / ScanTextureWidth) * ScanValueMultiplier + Random.value * ScanNoise;

            for (int y = 0; y < ScanTextureHeight; y++)
            {
                ScanTexture.SetPixel(ScanTextureWidth - _currentScanIndex, y, y < value ? Color.white : Color.black);
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

    private float ScanRay(float angle)
    {
        float output = 0f;

        Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
        int hits = Physics2D.RaycastNonAlloc(CollectionOrigin.position, direction, _hitBuffer, CollectionRange, _detectionLayerMask);

        for (int i = 0; i < hits; i++)
        {
            Ore ore = _hitBuffer[i].collider.GetComponent<Ore>();
            if (ore != null)
            {
                output += -_hitBuffer[i].normal.y;
            }
        }

        return output;
    }
}
