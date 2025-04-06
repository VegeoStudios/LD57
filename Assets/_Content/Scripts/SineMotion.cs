using UnityEngine;

public class SineMotion : MonoBehaviour
{
    [SerializeField] private Vector3 _motion;
    [SerializeField] private float _speed;
    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = _startPosition + _motion * Mathf.Sin(Time.time * _speed);
    }
}
