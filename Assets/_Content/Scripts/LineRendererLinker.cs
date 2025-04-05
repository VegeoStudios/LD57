using UnityEngine;

public class LineRendererLinker : MonoBehaviour
{
    public Transform[] Points;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found on this GameObject.");
            return;
        }
        if (Points == null || Points.Length < 2)
        {
            Debug.LogError("Two points are required to link the LineRenderer.");
            return;
        }
    }

    private void LateUpdate()
    {
        UpdateLineRenderer();
    }

    private void OnValidate()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer != null)
        {
            UpdateLineRenderer();
        }
    }

    private void UpdateLineRenderer()
    {
        if (_lineRenderer == null || Points == null || Points.Length < 2)
            return;
        _lineRenderer.positionCount = Points.Length;

        for (int i = 0; i < Points.Length; i++)
        {
            _lineRenderer.SetPosition(i, transform.InverseTransformPoint(Points[i].position));
        }
    }
}
