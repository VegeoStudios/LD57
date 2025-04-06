using UnityEngine;

public class CollectorDrillModule : ShipModule
{
    private const int _hitBufferSize = 20;

    public float CollectionRange = 25f;
    public float ScanSpeed = 1f;
    public float DrillSpeed = 1f;
    public float RotationSpeed = 5f;

    [SerializeField] private LayerMask _detectionLayerMask;
    [SerializeField] private float _minAngle = -45f;
    [SerializeField] private float _maxAngle = 45f;

    [SerializeField] private Transform _collectionOrigin;
    [SerializeField] private Transform _drill;

    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[_hitBufferSize];

    private Interactable _interactable;
    private CollectionMinigameUI _collectionMinigameUI;

    public bool DrillExtended => DrillDepth > 0.6f;
    public float DrillDepth { get; private set; } = 0.5f;
    public float DrillAngle { get; private set; } = 0f;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _collectionMinigameUI = _interactable.InteractionUI.GetComponent<CollectionMinigameUI>();
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
        DrillAngle = Mathf.Clamp(DrillAngle, _minAngle, _maxAngle);

        _drill.localPosition = new Vector3(0f, DrillDepth, 0f);
        _collectionOrigin.localRotation = Quaternion.Euler(0f, 0f, DrillAngle);
    }

    private void ScanningProcess()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CollectionRange);
        Gizmos.color = Color.red;
        Vector3 direction = Quaternion.Euler(0, 0, _minAngle) * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + direction * CollectionRange);
        direction = Quaternion.Euler(0, 0, _maxAngle) * transform.up;
        Gizmos.DrawLine(transform.position, transform.position + direction * CollectionRange);
    }

    public void CollectOre(Ore ore)
    {
        // Logic to collect ore
        Debug.Log($"Collected ore: {ore.name}");
        // Add ore to inventory or perform other actions
    }

    private float DoScanRay(float angle)
    {
        float output = 0f;

        Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
        int hits = Physics2D.RaycastNonAlloc(_collectionOrigin.position, direction, _hitBuffer, CollectionRange, _detectionLayerMask);

        for (int i = 0; i < hits; i++)
        {
            Ore ore = _hitBuffer[i].collider.GetComponent<Ore>();
            if (ore != null)
            {
                output += ore.Value;
            }
        }

        return output;
    }
}
