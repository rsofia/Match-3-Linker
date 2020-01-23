using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    Queue<GameObject> pool;
    public static SimplePool instance;

    private void Awake()
    {
        pool = new Queue<GameObject>();
        if(instance == null)
            instance = this;
    }

    public void InsertToQueue(GameObject obj)
    {
        pool.Enqueue(obj);
    }

    public GameObject Spawn(GameObject prefab, Transform parent, Vector2 position)
    {
        GameObject temp;
        if(pool.Count == 0)
        {
            temp = Instantiate(prefab, parent);
        }
        else
        {
            temp = pool.Dequeue();
            temp.SetActive(true);
            temp.transform.parent = parent;
        }
        temp.transform.localPosition = position;
        return temp;
    }
}
