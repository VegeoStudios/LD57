using UnityEngine;

public class WorldTransition : MonoBehaviour
{
    public float[] Positions;
    public float DepthScaling = 0.5f;

    private int index = 0;

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.transform.position;

        if (position.x > Positions[index] / DepthScaling + 100)
        {
            index++;
            if (index >= Positions.Length)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        position.x = Positions[index];
        position.z = 0f;
        transform.position = position;
    }
}
