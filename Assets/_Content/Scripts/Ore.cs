using UnityEngine;

public class Ore : MonoBehaviour
{
    public float Value = 1f;
    public Item Item = null;

    private void FixedUpdate()
    {
        if (OreSpawner.Instance.transform.position.x - transform.position.x > OreSpawner.Instance.DeletionDistance)
        {
            OreObjectPool.Instance.ReturnObject(this);
        }
    }
}
