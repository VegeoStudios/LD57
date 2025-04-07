using UnityEngine;

public class WorldTransition : MonoBehaviour
{
    public float[] Positions;

    private int index = 0;

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.transform.position;

        if (position.x > Positions[index] + 100)
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
