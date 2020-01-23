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

    public GameObject Spawn(GameObject prefab, Vector2 position)
    {
        GameObject temp;
        if(pool.Count == 0)
        {
            temp = Instantiate(prefab);
        }
        else
        {
            temp = pool.Dequeue();
            temp.SetActive(true);
        }
        temp.transform.position = position;
        return temp;
    }
}
