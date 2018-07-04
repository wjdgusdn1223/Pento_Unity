using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI Scale Animation
/// </summary>
public class UIScaleAnimation : MonoBehaviour , IListener
{
    //  Animation Speed
    public float speed = 2.5f;

    //  Animation Curve 1
    public AnimationCurve animationCurve1 = new AnimationCurve();

    //  Animation Curve 2

    public AnimationCurve animationCurve2 = new AnimationCurve();

    //  Animation time range
    [Range(0f, 2f)]
    public float t = 0.0f;

    //  Courutineが活性化されているかどうか
    private bool isPlaying = false;

    //---------------------------------------------------------------
    // Use this for initialization
    private void Start()
    {

    }

    //---------------------------------------------------------------
    /// <summary>
    /// Scale Animation Activate
    /// </summary>
    public bool StartScaleAnimation(float time, bool step = true)
    {
        if (isPlaying)
        {
            StopAllCoroutines();
            transform.localScale = Vector3.zero;

            t = 0f;

            //  Animation Activate
            StartCoroutine(animation(step, time));

            return false;
        }
        else
        {
            //  Animation Activate
            StartCoroutine(animation(step, time));

            return true;
        }
    }

    //---------------------------------------------------------------
    /// <summary>
    /// Scale Animation
    /// </summary>
    IEnumerator animation(bool step, float time)
    {
        isPlaying = true;

        //  Scale Change
        while (t < 2f)
        {
            float c = animationCurve1.Evaluate(t);
            Vector3 scale = new Vector3(c, c, c);
            transform.localScale = scale;
            t += Time.deltaTime * speed;

            yield return new WaitForSeconds(0.01f);
        }

        //  delay time
        yield return new WaitForSeconds(time);

        t = 0f;

        //  Scale Change
        while (t < 2f)
        {
            float c = animationCurve2.Evaluate(t);
            Vector3 scale = new Vector3(c, c, c);
            transform.localScale = scale;
            t += Time.deltaTime * speed;

            yield return new WaitForSeconds(0.01f);
        }

        t = 0f;

        if (step)
        {
            //  STEP_COMPLETE　イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
        }

        isPlaying = false;

        //  WRONG_BLOCK　イベントの場合Effectが終わったのを知らせる
        if (gameObject.name.Equals("Wrong"))
        {
            PentoGameController.Instance.WEO = true;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// イベントが発生する時、実行
    /// </summary>
    /// <param name="Event_Type">イベントのタイプ</param>
    /// <param name="Sender">受けるcomponent</param>
    /// <param name="Param">first parameter</param>
    /// <param name="Param2">second parameter</param>
    public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null, object Param2 = null)
    {

    }
}
