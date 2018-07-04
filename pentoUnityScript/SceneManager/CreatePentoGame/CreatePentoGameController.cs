using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CreatePentoGame Scene Controller
/// </summary>
public class CreatePentoGameController : MonoBehaviour {

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static CreatePentoGameController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static CreatePentoGameController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("CreatePentoGameControllerを実行");
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
        //  PentominoGameControllerを実行
        PentoGameController.Instance.startPentomino("Create", 0, 0.1f, new Vector3(-10.4f, 4f, 4.2f), new Vector3(70f, -20f, 0f));

        //  Fade Out Effectを実行
        GameObject.Find("Canvas").transform.Find("Fade Effect").GetComponent<FadeEffect>().StartFade(1f);

        //  Sound Play
        SoundController.Instance.PlaySound("Port");
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
