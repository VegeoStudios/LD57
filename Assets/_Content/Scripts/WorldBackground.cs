using UnityEngine;

public class WorldBackground : MonoBehaviour
{
    public float DepthScaling = 0.5f;
    public float minX = -10f;
    public float maxX = 10f;

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.transform.position;

        if (position.x > maxX + 200)
        {
            gameObject.SetActive(false);
        }

        position.x = Mathf.Clamp(position.x, minX / DepthScaling + 50, maxX / DepthScaling - 50);
        position.z = 0f;
        transform.position = position;
    }
}
