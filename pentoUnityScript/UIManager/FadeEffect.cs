using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade Effectを制御
/// </summary>
public class FadeEffect : MonoBehaviour
{
    //  Fade Effectが実行中かどうか
    public bool isFading = false;

    //  Fade Effectの程度
    private float fades;


    //---------------------------------------------------
    /// <summary>
    /// Fade In・Out Effectを実行
    /// </summary>
    /// <param name="degree">Fade Effectの程度</param>
    public void StartFade(float degree)
    {
        if (!isFading)
        {
            //  タイプによって違うEffectを実行
            switch (gameObject.name)
            {
                case "Story Title Panel":
                case "Story Narration Panel":
                case "Fade Effect":
                    if (gameObject.GetComponent<Image>().color.a >= degree)
                    {
                        fades = degree;
                        StartCoroutine(FadeAnimation("out", degree));                        
                    }
                    else if (gameObject.GetComponent<Image>().color.a <= 0f)
                    {
                        fades = 0f;
                        StartCoroutine(FadeAnimation("in", degree));
                    }
                    break;
                case "Story Title":
                case "Story Narration":
                    if (gameObject.GetComponent<Text>().color.a >= degree)
                    {
                        fades = degree;
                        StartCoroutine(FadeAnimation("out", degree));
                    }
                    else if (gameObject.GetComponent<Text>().color.a <= 0f)
                    {
                        fades = 0f;
                        StartCoroutine(FadeAnimation("in", degree));
                    }
                    break;
            }
        }
    }

    //---------------------------------------------------
    /// <summary>
    /// Fade In・Out Effect Animation
    /// </summary>
    /// <param name="type">Fade In・Out</param>
    /// <param name="degree">Fade Effectの程度</param>
    IEnumerator FadeAnimation(string type, float degree)
    {
        isFading = true;

        Image tmpImg;

        Text tmpTxt;

        //  Fade In・Out Effect 
        while (true)
        {
            switch (type)
            {
                case "out":
                    fades -= 0.03f;
                    break;
                case "in":
                    fades += 0.03f;
                    break;
            }

            //  タイプによって違うEffectを実行
            switch (gameObject.name)
            {
                case "Story Title Panel":
                case "Story Narration Panel":
                case "Fade Effect":
                    tmpImg = gameObject.GetComponent<Image>();
                    tmpImg.color = new Color(tmpImg.color.r, tmpImg.color.g, tmpImg.color.b, fades);
                    break;
                case "Story Title":
                case "Story Narration":
                    tmpTxt = gameObject.GetComponent<Text>();
                    tmpTxt.color = new Color(tmpTxt.color.r, tmpTxt.color.g, tmpTxt.color.b, fades);
                    break;
            }
            if (fades >= degree || fades <= 0f)
                break;

            yield return new WaitForSeconds(0.01f);
        }

        isFading = false;

        if (StoryUIController.Instance)
        {
            // UI_STEP_COMPLETE イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_STEP_COMPLETE, this);
        }
    }


}
