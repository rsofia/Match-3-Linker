using System.Collections;
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
        ORANGE,
        OBSTACLE
    }

    private int score = 200;
    private int extraScore = 50; //extra score to add after 3 where selected
    public GameObject goTile; //tile gamebject, with mesh
    public TileType type;
    public int row, col;

    [Header("Particles")]
    public ParticleSystem particleDisappear; //particles to play when tile is merged
    [Tooltip("Adds a little extra time after the particles disappear to disable the tile")]
    public float paritcleExtraTime = 0.1f;

    [Header("UI")]
    public TextMesh text;

    [Header("Animation")]
    private bool isFalling = false;

    [HideInInspector]
    public LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public void SetRowCol(int r, int c)
    {
        row = r; col = c;
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButton(0) || (Input.touchCount > 0))
            GameManager.instance.AddToStack(this);
    }


    /// <summary>
    /// Function to initialize our tile
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    public virtual void OnBegin(int r, int c, Vector2 startPos, Vector2 goalPos)
    {
        row = r; col = c;
        gameObject.SetActive(true);
        particleDisappear.Stop();
        text.text = "";
        goTile.gameObject.SetActive(true);

        //FALL TO POSITION
        Fall(startPos, goalPos);
    }

    /// <summary>
    /// Called when they merge. 
    /// Stack index = order in which it was selected
    /// </summary>
    /// <param name="stackIndex"></param>
    public virtual void OnMerge(int stackIndex)
    {
        if (stackIndex < GameManager.minToConnect)
        {
            text.text = Helper.NumberToString(score);
            GameManager.instance.AddScore(score);
        }
        else
        {
            //add extra score if the line is bigger than 3
            int tomultiply = (stackIndex / GameManager.minToConnect);
            int extra = extraScore * tomultiply;
            text.text = Helper.NumberToString(score + extra);
            GameManager.instance.AddScore(score + extra);
        }
        StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        goTile.SetActive(false);
        ResetLine();
        particleDisappear.Play();
        //waits until the particle system has finished playing
        yield return new WaitForSeconds(particleDisappear.main.duration + paritcleExtraTime);
        gameObject.SetActive(false);
        //Return to pool
        SimplePool.instance.Insert(this);
    }

    #region LINE
    public virtual void SetLine(int x, int y)
    {
        line.SetPosition(1, new Vector3(x, y, 0));
    }

    /// <summary>
    /// Remove Selection
    /// </summary>
    public virtual void ResetLine()
    {
        line.SetPosition(1, Vector3.zero);
    }
    #endregion

    #region FALL
    public void Fall(Vector2 startPos, Vector2 goalPos)
    {
        transform.localPosition = startPos;
        if (!isFalling)
            StartCoroutine(Fall(goalPos, GameManager.instance.tileFallSpeed));
    }

    IEnumerator Fall(Vector2 goalPos, float speed, float minDistance = 0.2f,float deltaTime = 0.1f)
    {
        isFalling = true;
        float t = 0;
        do
        {
           // float yPos = curve.Evaluate(t);
            transform.position = Vector3.Lerp(transform.localPosition, goalPos, t);
            yield return new WaitForSeconds(deltaTime);
            t += deltaTime; //add the time

        } while (Vector2.Distance(transform.localPosition, goalPos) > minDistance); //run until the time of the last frame
        transform.localPosition = goalPos; //make sure its a the position we want it to 
        isFalling = false;
    }
    #endregion

    

    
}
