using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CameraAnchor anchor;

    private void Update()
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
