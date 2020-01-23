using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score;
    public const int minToConnect = 3; //minimum tiles that must be connected
    public static GameManager instance;


    public LevelData levelData;

    [Header("Board")]
    public Transform grid;
    [Tooltip("Space between tiles")]
    public float spaceBetween = 0.25f;
    int[,] board;
    private bool canAdd = false; //can a new tile be selecter right now?

    [Header("Timers")]
    [Tooltip("Time in between tiles when merging")]
    public float timeMerge = 0.1f;

    private Stack<TileBase> order = new Stack<TileBase>();

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
        CreateBoard();
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (order.Count >= minToConnect)
                StartCoroutine(MergeTiles());
            else
                ClearOrder();
        }
    }

    private void CreateBoard()
    {
        canAdd = false;
        GameObject prefab = levelData.tilePrefabs[0].tilePrefab; //will be the first by default
        for (int row = 0; row < levelData.gridSize.x; row++)
        {

            for (int col = 0; col < levelData.gridSize.y; col++)
            {
                //for now, it will be a square grid, 
                //TODO figure out how to make the levels uneven
                board[row, col] = (int)GridInfo.Playable;

                //Create different type of tiles following their odds
                int odds = Random.Range(0, 1); //Spawn will be completely random
                float prob = 0;
                for(int i = 0; i < levelData.tilePrefabs.Length; i++)
                {
                    if(odds < ((levelData.tilePrefabs[i].probability + prob) * 10))
                    {
                        prefab = levelData.tilePrefabs[i].tilePrefab;
                        break; //end cicle
                    }
                    else
                    {
                        //accumulated probability
                        prob += levelData.tilePrefabs[i].probability;
                    }
                }
               TileBase b = SimplePool.instance.Spawn(prefab, new Vector2(row + (spaceBetween * row), col + (spaceBetween * col))).GetComponent<TileBase>();
                b.OnBegin(row, col);
                
            }
        }
        canAdd = true;
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

    /// <summary>
    /// Insert a tile into the order stack.
    /// </summary>
    /// <param name="tile"></param>
    public void AddToStack(TileBase tile)
    {
        if (!canAdd)
            return;
        //If the stack is empty, add anything
        if(order.Count == 0)
        {
            order.Push(tile);
        }
        else
        {
            //Only add if they are the same type and if it's not already here and it's ajacent
            bool isAdjacent = (tile.row <= (order.Peek().row + 1) && tile.row >= (order.Peek().row - 1)) &&
                                (tile.col <= (order.Peek().col + 1) && tile.col >= (order.Peek().col - 1));
            if (order.Peek().type == tile.type && !order.Contains(tile) && isAdjacent)
            {
                //TODO DISPLAY CONNECTION
                //draw line renderer to here
                TileBase prev = OrderPeek();
                int x = tile.row - prev.row;
                int y = tile.col - prev.col;
    
                prev.line.SetPosition(1, new Vector3(x, y, 0));

                //Insert in stack
                order.Push(tile);
            }
        }
    }

    /// <summary>
    /// Remove everything from stack.
    /// </summary>
    public IEnumerator MergeTiles()
    {
        canAdd = false;
        int count = order.Count;
       int index = count;

       for(int i = 0; i < count; i++, index--)
        {
            TileBase tile = order.Pop();
            tile.OnMerge(index);
            yield return new WaitForSeconds(timeMerge);
        }
        canAdd = true;
    }

    /// <summary>
    /// Removes line connecting tiles, and clears the stack.
    /// </summary>
    public void ClearOrder()
    {
        canAdd = false;
        int count = order.Count;
        int index = count;

        for (int i = 0; i < count; i++, index--)
        {
            TileBase tile = order.Pop();
            tile.line.SetPosition(1, Vector3.zero); //reset line
        }
        canAdd = true;
    }

    public int OrderCount()
    {
        return order.Count;
    }

    public TileBase OrderPeek()
    {
        return order.Peek();
    }



}
