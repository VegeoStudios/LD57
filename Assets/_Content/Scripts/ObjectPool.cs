using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 50;
    private Queue<T> pool;

    private void Awake()
    {
        InitializePool();
    }

    protected void InitializePool()
    {
        Debug.Log("Creating object pool for " + typeof(T).Name);
        pool = new Queue<T>();
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            pool.Enqueue(obj);
        }
    }

    private T CreateNewObject()
    {
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj.GetComponent<T>();
    }

    public T GetObject()
    {
        if (pool.Count == 0)
        {
            T obj = CreateNewObject();
            pool.Enqueue(obj);
        }
        T item = pool.Dequeue();
        ((MonoBehaviour)(object)item).gameObject.SetActive(true);
        return item;
    }

    public void ReturnObject(T item)
    {
        (item as MonoBehaviour).transform.SetParent(transform);
        ((MonoBehaviour)(object)item).gameObject.SetActive(false);
        pool.Enqueue(item);
    }
}
