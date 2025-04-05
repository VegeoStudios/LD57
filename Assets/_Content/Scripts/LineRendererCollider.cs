using UnityEngine;

public class LineRendererCollider : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found on this GameObject.");
            return;
        }
        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider == null)
        {
            Debug.LogError("BoxCollider2D component not found on this GameObject.");
            return;
        }
    }

    private void LateUpdate()
    {
        UpdateCollider();
    }

    private void OnValidate()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();
        if (_boxCollider == null)
            _boxCollider = GetComponent<BoxCollider2D>();
        if (_lineRenderer != null && _boxCollider != null)
        {
            UpdateCollider();
        }
    }

    private void UpdateCollider()
    {
        if (_lineRenderer == null || _boxCollider == null)
            return;
        Vector3 start = _lineRenderer.GetPosition(0);
        Vector3 end = _lineRenderer.GetPosition(1);
        Vector3 center = (start + end) / 2;
        float width = Vector3.Distance(start, end);
        float height = _lineRenderer.startWidth;
        _boxCollider.offset = new Vector2(center.x, center.y);
        _boxCollider.size = new Vector2(width, height);
    }

}
