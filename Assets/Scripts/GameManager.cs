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

    private Stack<TileBase> order = new Stack<TileBase>();

    public enum GridInfo
    {
        Obstacle, //cannot detect input here
        Playable //contains a normal tile
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

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
        for (int row = 0; row < levelData.gridSize.x; row++)
        {

            for (int col = 0; col < levelData.gridSize.y; col++)
            {
                //for now, it will be a square grid, 
                //TODO figure out how to make the levels uneven

                //Create different type of tiles following their odds
                CreateRandomTileAt(row, col);
                
            }
        }
        canAdd = true;
    }

    private void CreateRandomTileAt(int row, int col)
    {
        float prob = 0;
        GameObject prefab = levelData.tilePrefabs[Random.Range(0, levelData.tilePrefabs.Length)].tilePrefab; //will be the first by default
        for (int i = 0; i < levelData.tilePrefabs.Length; i++)
        {
            int odds = Random.Range(0, 100); //Spawn will be completely random
            if (odds < ((levelData.tilePrefabs[i].probability + prob) * 10))
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
        Vector2 goalPos = new Vector2(row + (spaceBetween * row), col + (spaceBetween * col));
        Vector2 startPos = new Vector2(row, startTilePosition);
        TileBase b = SimplePool.instance.Spawn(prefab, grid, startPos).GetComponent<TileBase>();
        board[row, col] = new BoardPiece(GridInfo.Playable, b, goalPos);
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
            //remove from board
            board[tile.row, tile.col].tile = null;
            yield return new WaitForSeconds(timeMerge);
        }
        canAdd = true;

        StartCoroutine(Collapse());
    }

    /// <summary>
    /// After a merge, collapse tiles
    /// </summary>
    /// <returns></returns>
    public IEnumerator Collapse()
    {
        //recorrer todo el arreglo, de abajo para arriba
        for(int row = 0; row < levelData.gridSize.x; row++)
        {
            for(int col = 0; col < levelData.gridSize.y; col++)
            {
                if (board[row, col] == null && board[row, col].info != GridInfo.Playable)
                    continue;
                if(board[row, col].tile == null)
                {
                    //collapse the whole column
                    int current = col;
                    do
                    {
                        int c = current + 1;
                        if (c < levelData.gridSize.y)
                        {
                            if (board[row, c].tile != null)
                            {
                                board[row, col].tile = board[row, c].tile;
                                board[row, col].tile.SetRowCol(row, col);
                                board[row, col].tile.Fall(board[row, col].tile.transform.localPosition, board[row, col].pos);
                                board[row, c].tile = null;
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
                //If it is indeed a playable are and it does not have a tile, create one
                if(board[row, col].info == GridInfo.Playable && board[row, col].tile == null)
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
