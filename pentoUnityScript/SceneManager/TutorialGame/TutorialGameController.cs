using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TutorialGame Scene Controller
/// </summary>
public class TutorialGameController : MonoBehaviour {

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static TutorialGameController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static TutorialGameController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("TutorialGameControllerを実行");
#endif

        //  sceneにinstanceが存在するかを検査
        //  存在する場合,消滅させる。
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        //  このinstanceを唯一のobjectにする。
        instance = this;
    }

    //---------------------------------------------------
    // Use this for initialization
    private void Start()
    {
        //  StoryControllerを実行
        StoryController.Instance.StartStory("Tutorial");

        //  Fade Out Effectを実行
        GameObject.Find("Canvas").transform.Find("Fade Effect").GetComponent<FadeEffect>().StartFade(1f);

        //  Sound Play
        SoundController.Instance.PlaySound("Maple");
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
        switch (Event_Type)
        {

        }
    }
}
