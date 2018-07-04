using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection Game Controller
/// </summary>
public class CollectionGameController : MonoBehaviour {

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static CollectionGameController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static CollectionGameController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("CollectionGameControllerを実行");
        Debug.Log("パズルの番号 : " + GameData.CollectionNum);
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
        PentoGameController.Instance.startPentomino("Puzzle", GameData.CollectionNum, 0.1f, new Vector3(-0.899f, 2.303f, 1.247f), new Vector3(0f, 0f, 0f));

        //  Fade Out Effectを実行
        GameObject.Find("Canvas").transform.Find("Fade Effect").GetComponent<FadeEffect>().StartFade(1f);

        //  Sound Play
        SoundController.Instance.PlaySound("Edel");
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
