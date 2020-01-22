using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score;
    public static GameManager instance;

    int[,] board;

    public LevelData levelData;

    private enum GridInfo
    {
        Obstacle, //cannot detect input here
        Playable //contains a tile
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        board = new int[levelData.gridSize.x, levelData.gridSize.y];
    }

    private void CreateBoard()
    {
        for(int row = 0; row < levelData.gridSize.x; row++)
        {

            for (int col = 0; col < levelData.gridSize.y; col++)
            {
                //for now, it will be a square grid, 
                //TODO figure out how to make the levels uneven
                board[row, col] = (int)GridInfo.Playable;
            }
        }
    }

    #region SCORE
    /// <summary>
    /// Returns the scores
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Adds this amount to the score. +=
    /// </summary>
    /// <param name="valueToAdd"></param>
    public void AddScore(int valueToAdd)
    {
        score += valueToAdd;
    }
    #endregion

}
