using UnityEngine;

public class WorldBackground : MonoBehaviour
{
    public float minX = -10f;
    public float maxX = 10f;

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.transform.position;

        if (position.x > maxX + 200)
        {
            gameObject.SetActive(false);
        }

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = 0f;
        transform.position = position;
    }
}
