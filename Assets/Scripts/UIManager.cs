using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image backgroud;
    [Header("Score")]
    public AnimUI scoreAnim;
    public TextMeshProUGUI score;

    [Header("Replay")]
    public AnimUI replayAnim;

    [Header("Goal")]
    public TextMeshProUGUI goalDisplay;
    public TextMeshProUGUI txtYourScore;
    public AnimUI goalUnlockedAnim;

    [Header("Quit")]
    public AnimUI quitAnim;
    [Tooltip("Time in seconds to Quit game after presset Exit button.")]
    public float timeToQuit = 3.0f;

    private void Awake()
    {
        backgroud.gameObject.SetActive(false);
        goalUnlockedAnim.rect.gameObject.SetActive(false);
        quitAnim.rect.gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the score on UI. Calls text zoom in animation.
    /// </summary>
    /// <param name="s"></param>
    public void DisplayScore(int s)
    {
        score.text = "$" + Helper.NumberToString(s); //formar with thousands separator

        //make sure to only run once until it's over
        if(!scoreAnim.isOnLerp)
            StartCoroutine(LerpSize(scoreAnim));
    }

    public void Replay()
    {
        if (!replayAnim.isOnLerp)
            StartCoroutine(LerpSize(replayAnim));
        GameManager.instance.Replay();
    }

    public void PlayAgain()
    {
        backgroud.gameObject.SetActive(false);
        quitAnim.rect.gameObject.SetActive(false);
        GameManager.instance.Replay();
    }

    public void SetGoal(string goal)
    {
        goalDisplay.text = "Goal: " + goal;
    }

    public void UnlockGoal(int score)
    {
        backgroud.gameObject.SetActive(true);
        goalUnlockedAnim.rect.gameObject.SetActive(true);
        txtYourScore.text = "Your score: " + Helper.NumberToString(score);
        if (!goalUnlockedAnim.isOnLerp)
            StartCoroutine(LerpSize(goalUnlockedAnim));
    }

    public void ExitGame()
    {
        backgroud.gameObject.SetActive(true);
        quitAnim.rect.gameObject.SetActive(true);
        StartCoroutine(Quit());

    }

    /// <summary>
    /// Lerps the scale of a rect transform with a given Animation Curve.
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="rect"></param>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    IEnumerator LerpSize(AnimUI anim, float deltaTime = 0.01f)
    {
        anim.isOnLerp = true;
        float t = 0;
        do
        {
            float size = anim.curve.Evaluate(t);
            anim.rect.localScale = new Vector3(size, size, size);
            yield return new WaitForSeconds(deltaTime);
            t += deltaTime; //add the time

        } while (t < anim.curve[anim.curve.length-1].time); //run until the time of the last frame

        anim.isOnLerp = false;
    }

    IEnumerator Quit()
    {
        // WINDOW THANKING THEM
        if (!quitAnim.isOnLerp)
            StartCoroutine(LerpSize(quitAnim));
        yield return new WaitForSeconds(timeToQuit);

        Application.Quit();
    }

    [System.Serializable]
    public class AnimUI
    {
        public RectTransform rect;
        public AnimationCurve curve;
        [HideInInspector]
        public bool isOnLerp;
    }
}
