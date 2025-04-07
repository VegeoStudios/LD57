using UnityEngine;
using UnityEngine.EventSystems;

public class UIWheel : MonoBehaviour
{
    public float value;
    public float Min;
    public float Max;

    public float MinAngle;
    public float MaxAngle;

    public float Sensitivity;

    private bool _grabbed;
    private float angle;
    

    private void Update()
    {
        if (_grabbed)
        {
            value += Input.GetAxis("Mouse X") * Sensitivity;
            value = Mathf.Clamp(value, Min, Max);
        }

        angle = Mathf.Lerp(MinAngle, MaxAngle, (value - Min) / (Max - Min));
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void PointerUp(BaseEventData eventData)
    {
        _grabbed = false;
    }

    public void PointerDown(BaseEventData eventData)
    {
        _grabbed = true;
    }
}
