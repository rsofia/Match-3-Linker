using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridInfo
{
    Playable, //contains a normal tile
    Obstacle //cannot detect input here
}


public class GameManager : MonoBehaviour
{
    private int score;
    public const int minToConnect = 3; //minimum tiles that must be connected
    public static GameManager instance;


    public LevelData levelData;

    [Header("Game Goal")]
    public int scoreGoal = 1000;

    [Header("Board")]
    public Transform grid;
    [Tooltip("Space between tiles")]
    public float spaceBetween = 0.25f;
    [Tooltip("Where (y) will the tile fall from")]
    public float startTilePosition;
    [Tooltip("How fast will the tile fall")]
    public float tileFallSpeed = 1;

    BoardPiece[,] board;
    private bool canAdd = false; //can a new tile be selecter right now?

    [Header("Timers")]
    [Tooltip("Time in between tiles when merging")]
    public float timeMerge = 0.1f;
    [Tooltip("Time in between old tiles collapsing and new ones appearing")]
    public float timeForNew = 0.25f;

    [Header("UI")]
    public UIManager ui;

    private LinkedList<TileBase> order = new LinkedList<TileBase>();

    [HideInInspector]
    public bool isGameOver = false; //can the game keep going?
   

    private void Awake()
    {
        if (instance == null)
            instance = this;

        board = new BoardPiece[levelData.gridSize.x, levelData.gridSize.y];
        CreateBoard();
    }

    /// <summary>
    /// Reloads game and resets score.
    /// </summary>
    public void Replay()
    {
        //clear board
        for (int row = 0; row < levelData.gridSize.x; row++)
        {

            for (int col = 0; col < levelData.gridSize.y; col++)
            {
               StartCoroutine(board[row, col].tile.Die());
            }
        }

        score = 0;
        ui.DisplayScore(0);
        board = new BoardPiece[levelData.gridSize.x, levelData.gridSize.y];
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
        ui.SetGoal("get a score of: " + Helper.NumberToString(scoreGoal));
        for (int row = 0; row < levelData.gridSize.x; row++)
        {

            for (int col = 0; col < levelData.gridSize.y; col++)
            {
                //fill with regular tiles
                if(levelData.boardInfo[levelData.GetIndex(row, col)] == GridInfo.Playable)
                {
                    //Create different type of tiles following their odds
                    CreateRandomTileAt(row, col);
                }
                //Fill with obstacle
                else if (levelData.boardInfo[levelData.GetIndex(row, col)] == GridInfo.Obstacle)
                {
                    CreateObstacle(row, col);
                }
            }
        }
        canAdd = true;
        isGameOver = false;
    }

    private void CreateRandomTileAt(int row, int col)
    {
        float prob = 0;
        int index = Random.Range(0, levelData.tilePrefabs.Length);
        for (int i = 0; i < levelData.tilePrefabs.Length; i++)
        {
            int odds = Random.Range(0, 100); //Spawn will be completely random
            if (odds < ((levelData.tilePrefabs[i].probability + prob) * 10))
            {
                index = i;
                break; //end cicle
            }
            else
            {
                //accumulated probability
                prob += levelData.tilePrefabs[i].probability;
            }
        }
        Vector2 goalPos = new Vector2(col + (spaceBetween * col), row + (spaceBetween * row));
        Vector2 startPos = new Vector2(col, startTilePosition);
        TileBase b = SimplePool.instance.Spawn(levelData.tilePrefabs[index].tilePrefab,grid, startPos).GetComponent<TileBase>();
        board[row, col] = new BoardPiece(GridInfo.Playable, b, goalPos);
        b.OnBegin(row, col, startPos, goalPos);
    }

    private void CreateObstacle(int row, int col)
    {
        GameObject prefab = levelData.obstacle.tilePrefab;
        Vector2 goalPos = new Vector2(col + (spaceBetween * col), row + (spaceBetween * row));
        Vector2 startPos = new Vector2(col, startTilePosition);
        TileBase b = SimplePool.instance.Spawn(prefab, grid, startPos).GetComponent<TileBase>();
        board[row, col] = new BoardPiece(GridInfo.Obstacle, b, goalPos);
        b.OnBegin(row, col, startPos, goalPos);
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
        ui.DisplayScore(score);

        if(score >= scoreGoal)
        {
            ui.UnlockGoal(score);
            isGameOver = true;
        }
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
        //If the list is empty, add anything
        if(order.Count == 0)
        {
            order.AddLast(tile);
        }
        else
        {
            //Only add if they are the same type and if it's not already here and it's ajacent
            bool isAdjacent = (tile.row <= (order.Last.Value.row + 1) && tile.row >= (order.Last.Value.row - 1)) &&
                                (tile.col <= (order.Last.Value.col + 1) && tile.col >= (order.Last.Value.col - 1));
            if (order.Last.Value.type == tile.type && !order.Contains(tile) && isAdjacent)
            {
                //DISPLAY CONNECTION
                //draw line renderer to here
                TileBase prev = order.Last.Value;
                int y = tile.row - prev.row;
                int x = tile.col - prev.col;

                prev.SetLine(x, y);

                //Insert
                order.AddLast(tile);
            }
            //if the tile we are trying to add is the same as the second to last, remove the last one
            //a.k.a. retracing
            else if(order.Last.Previous != null &&  tile == order.Last.Previous.Value)
            {
                //reset lines
                order.Last.Value.ResetLine();
                order.Last.Previous.Value.ResetLine();
                order.RemoveLast();
            }
        }
    }

    

    /// <summary>
    /// Merge the tiles and Remove everything from the LinkedList.
    /// </summary>
    public IEnumerator MergeTiles()
    {
        canAdd = false;
        int count = order.Count;
       int index = count;

       for(int i = 0; i < count; i++, index--)
        {
            //Start Removing from the first to the last
            TileBase tile = order.First.Value;
            order.RemoveFirst();
            tile.OnMerge(i);
            //remove from board
            board[tile.row, tile.col].tile = null;
            yield return new WaitForSeconds(timeMerge);
        }
        canAdd = true;

        StartCoroutine(Collapse());
    }

    /// <summary>
    /// After a merge, collapse tiles. And then add new pieces.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Collapse()
    {
        //recorrer todo el arreglo, de abajo para arriba
        for(int row = 0; row < levelData.gridSize.x; row++)
        {
            for(int col = 0; col < levelData.gridSize.y; col++)
            {
                if (board[row, col] == null)// || board[row, col].info != GridInfo.Playable)
                    continue;
                if(board[row, col].tile == null)
                {
                    //collapse the whole column
                    int current = row;
                    do
                    {
                        int r = current + 1;
                        if (r < levelData.gridSize.x)
                        {
                            if (board[r, col].tile != null)
                            {
                                board[row, col].tile = board[r, col].tile;
                                board[row, col].tile.SetRowCol(row, col);
                                board[row, col].tile.Fall(board[row, col].tile.transform.localPosition, board[row, col].pos);
                                board[r, col].tile = null;
                                break;
                            }
                            else
                            {
                                current++;
                            }
                        }
                        else
                            break;
                    } while (board[row, col].tile == null);
                }

            }
        }

        yield return new WaitForSeconds(timeForNew);
        //Add new pieces
        for(int row = 0; row < levelData.gridSize.x; row++)
        {
            for(int col = 0; col < levelData.gridSize.y; col++)
            {
                if(board[row, col].tile == null)
                {
                    CreateRandomTileAt(row, col);
                }
            }
        }

        yield return null;
    }

    /// <summary>
    /// Removes line connecting tiles, and clears the stack.
    /// </summary>
    public void ClearOrder()
    {
        canAdd = false;
        int count = order.Count;
        // int index = count;

        for (int i = 0; i < count; i++)
        {
            TileBase tile = order.Last.Value;
            order.RemoveLast();
            tile.ResetLine();
        }
        canAdd = true;
    }
    
    /// <summary>
    /// Class to create the board and holds its info
    /// </summary>
    private class BoardPiece
    {
        public GridInfo info;
        public TileBase tile;
        public Vector2 pos;

        public BoardPiece(GridInfo info, TileBase tile, Vector2 pos) 
        {
            this.info = info;
            this.tile = tile;
            this.pos = pos;
        }
    }

}
