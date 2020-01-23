using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", 
    menuName = "ScriptableObjects/Level Information", order = 1)]
public class LevelData : ScriptableObject
{
    public Vector2Int gridSize = new Vector2Int(5, 5);

    public TilePrefab[] tilePrefabs;
}

[System.Serializable]
public class TilePrefab
{
    public GameObject tilePrefab;
    [Tooltip("Probability to spawn on this level.")]
    public float probability;
}
