using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMinigameUI : MonoBehaviour
{
    [SerializeField] private RawImage _graph;
    [SerializeField] private RectTransform _angleCursor;
    [SerializeField] private RectTransform _depthCursor;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Transform _tooFastNotif;

    private CollectorDrillModule _collectorDrillModule;

    private void Awake()
    {
        _collectorDrillModule = GetComponentInParent<CollectorDrillModule>();
    }

    private void Start()
    {
        _graph.texture = _collectorDrillModule.ScanTexture;
    }

    private void Update()
    {
        _tooFastNotif.gameObject.SetActive(ShipSystemsManager.Instance.CurrentSpeed > _collectorDrillModule.MaxShipSpeed);

        float graphWidth = _graph.rectTransform.rect.width;
        float graphHeight = _graph.rectTransform.rect.height;
        _angleCursor.anchoredPosition = new Vector2(Mathf.InverseLerp(_collectorDrillModule.MaxAngle, _collectorDrillModule.MinAngle, _collectorDrillModule.DrillAngle) * graphWidth, 0);
        _depthCursor.anchoredPosition = new Vector2(0, -Mathf.InverseLerp(_collectorDrillModule.CollectionRange, 0f, _collectorDrillModule.DrillDepth) * graphHeight);
        _countText.text = _collectorDrillModule.Drill.childCount.ToString();
    }
}
