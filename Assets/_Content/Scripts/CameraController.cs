using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CameraAnchor anchor;

    [SerializeField] private float _smoothing = 2f;
    [SerializeField] private float _smoothingDistance = 5f;

    private Vector3 _lastPosition;

    private void LateUpdate()
    {
        ParentedSmoothingUpdate();
        _lastPosition = transform.parent.localPosition;
    }

    private void ParentedSmoothingUpdate()
    {
        Vector3 targetDirection = transform.parent.localPosition - _lastPosition;
        Vector3 targetPosition = targetDirection * _smoothingDistance;
        targetPosition.z = -10f;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, _smoothing * Time.deltaTime);
    }

    private void AnchorUpdate()
    {
        if (anchor == null)
            return;

        Vector3 targetPosition = anchor.transform.position;
        targetPosition.z = transform.position.z;

        if (anchor.LerpToPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, anchor.LerpSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }    
}
