using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", 
    menuName = "ScriptableObjects/Level Information", order = 1)]
public class LevelData : ScriptableObject
{
    public Vector2Int gridSize = new Vector2Int(5, 5);

    public Vector2Int Grid
    {
        get
        {
            return gridSize;
        }
        set
        {
            gridSize = value;
            SetBoard(gridSize);
        }
    }

    public TilePrefab[] tilePrefabs;
    public TilePrefab obstacle;

    [HideInInspector]
    public GridInfo[] boardInfo;

    public LevelData()
    {
        SetBoard(gridSize.x, gridSize.y);
    }

    public int GetIndex(int row, int col)
    {
        return (gridSize.y * col + row);
    }

    public void SetBoard(int x, int y)
    {
        gridSize.x = x;
        gridSize.y = y;
        boardInfo = new GridInfo[gridSize.x * gridSize.y];
    }

    public void SetBoard(Vector2Int val)
    {
        gridSize = val;
        boardInfo = new GridInfo[gridSize.x * gridSize.y];
    }
}

[System.Serializable]
public class TilePrefab
{
    public GameObject tilePrefab;
    [Tooltip("Probability to spawn on this level.")]
    public float probability;
}
