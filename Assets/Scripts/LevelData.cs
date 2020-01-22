using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", 
    menuName = "ScriptableObjects/Level Information", order = 1)]
public class LevelData : ScriptableObject
{
    public Vector2Int gridSize = new Vector2Int(5, 5);
}
