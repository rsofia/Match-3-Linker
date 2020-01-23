﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TileBase : MonoBehaviour
{
    public enum TileType
    {
        GREEN,
        BLUE,
        RED,
        PURPLE,
        ORANGE
    }

    private int score = 200;
    private int extraScore = 50; //extra score to add after 3 where selected
    public ParticleSystem particleDisappear; //particles to play when tile is merged
    public GameObject goTile; //tile gamebject, with mesh
    public TileType type;

    public int row, col;
    [HideInInspector]
    public LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public virtual void OnBegin(int r, int c)
    {
        row = r; col = c;
        Debug.Log("On Begin");
        gameObject.SetActive(true);
        particleDisappear.Stop();
    }

    private void OnMouseEnter()
    {
        if(Input.GetMouseButton(0) || (Input.touchCount > 0))
            GameManager.instance.AddToStack(this);
    }


    /// <summary>
    /// Called when they merge. 
    /// Stack index = order in which it was sellected
    /// </summary>
    /// <param name="stackIndex"></param>
    public virtual void OnMerge(int stackIndex)
    {
        Debug.Log("On merge");
        if (stackIndex < GameManager.minToConnect)
            GameManager.instance.AddScore(score);
        else
        {
            //add extra score if the line is bigger than 3
            int tomultiply = (stackIndex / GameManager.minToConnect);
            int extra = extraScore * tomultiply;
            Debug.Log("Extra score: " + extra + " from index " + stackIndex);
            GameManager.instance.AddScore(score + extra);
        }
        StartCoroutine(Die());
   }


    private IEnumerator Die()
    {
        goTile.SetActive(false);
        particleDisappear.Play();
        //waits until the partycle sistem has finished playing
        yield return new WaitForSeconds(particleDisappear.time);
        gameObject.SetActive(false);
        //Return to pool
        SimplePool.instance.InsertToQueue(gameObject);
    }

    
}
