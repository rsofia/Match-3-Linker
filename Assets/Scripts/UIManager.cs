﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Score")]
    public Text score;
    public AnimationCurve scoreAnimIn;
    private bool txtIsOnLerp = false;

    [Header("Replay")]
    public Button btnReplay;
    public AnimationCurve replayAnim;
    private bool btnReplayIsOnLerp = false;

    
    /// <summary>
    /// Displays the score on UI. Calls text zoom in animation.
    /// </summary>
    /// <param name="s"></param>
    public void DisplayScore(int s)
    {
        score.text = "$" + Helper.NumberToString(s); //formar with thousands separator

        //make sure to only run once until it's over
        if(!txtIsOnLerp)
            StartCoroutine(LerpSize(scoreAnimIn, score.rectTransform));
    }

    public void Replay()
    {
        if (!btnReplayIsOnLerp)
            StartCoroutine(LerpSize(replayAnim, btnReplay.GetComponent<RectTransform>()));
        StartCoroutine(GameManager.instance.Replay());
    }

    /// <summary>
    /// Lerps the scale of a rect transform with a given Animation Curve.
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="rect"></param>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    IEnumerator LerpSize(AnimationCurve curve, RectTransform rect, float deltaTime = 0.01f)
    {
        txtIsOnLerp = true;
        float t = 0;
        do
        {
            float size = curve.Evaluate(t);
            rect.localScale = new Vector3(size, size, size);
            yield return new WaitForSeconds(deltaTime);
            t += deltaTime; //add the time

        } while (t < curve[curve.length-1].time); //run until the time of the last frame
        txtIsOnLerp = false;
    }

}
