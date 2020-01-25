using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    List<TileBase> pool;
    public static SimplePool instance;

    private void Awake()
    {
        pool = new List<TileBase>();
        if(instance == null)
            instance = this;
    }

    public void Insert(TileBase obj)
    {
        pool.Add(obj);
    }

    public TileBase Spawn(GameObject prefab, Transform parent, Vector2 position)
    {
        TileBase temp;
        if(pool.Count == 0)
        {
            temp = Instantiate(prefab, parent).GetComponent<TileBase>();
        }
        else
        {
            TileBase.TileType t = prefab.GetComponent<TileBase>().type;
            temp = pool.Find(x => x.type == t);
            if(temp == null)
            {
                temp = Instantiate(prefab, parent).GetComponent<TileBase>();
            }
            else
            {
                temp.gameObject.SetActive(true);
                temp.transform.parent = parent;
                pool.Remove(temp);
            }
            
        }
        temp.transform.localPosition = position;
        return temp;
    }
}
