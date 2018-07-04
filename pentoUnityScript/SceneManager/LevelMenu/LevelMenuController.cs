using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//---------------------------------------------------
/// <summary>
/// LevelMenu Scene Controller
/// </summary>
public class LevelMenuController : MonoBehaviour, IListener {

    //---------------------------------------------------
    /// <summary>
    /// enterLevel c# property
    /// </summary>
    public int EL
    {
        get { return enterLevel; }
        set
        {
            enterLevel = value;

            //  ENTER_LEVEL　イベントをListenerに転送
            EventManager.Instance.PostNotification(
                EVENT_TYPE.ENTER_LEVEL, this, enterLevel);
        }
    }

    //---------------------------------------------------
    //  User Level
    private int userLevel;

    //  入場するレベル
    private int enterLevel;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static LevelMenuController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static LevelMenuController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake () {
        #if SHOW_DEBUG_MESSAGES
            Debug.Log("LevelMenuControllerを実行");
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
        //  SERVER_CONNECTED いべんとを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTED, this);
        //  ENTER_LEVEL いべんとを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.ENTER_LEVEL, this);
        //  SERVER_RESPONSE いべんとを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);

        //  User Level Check
        UserLevelRequest();

        //  Sound Play
        SoundController.Instance.PlaySound("Title Screen");
    }

    //---------------------------------------------------
    /// <summary>
    /// User Levelをサーバーに要請
    /// </summary>
    private void UserLevelRequest()
    {
        //  会員番号をサーバーに転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("user_no", GameData.userNum);

        ServerRequest.Instance.POST(GameData.UnityURL, "GetUserLevel", post);
    }

    //---------------------------------------------------
    /// <summary>
    /// User Level Check
    /// </summary>
    /// <param name="tmpLevel">サーバーからもらったレベル番号</param>
    private void UserLevelCheck(int tmpLevel)
    {
        //  サーバー例外処理
        if (tmpLevel == -1)
        {
            //  Server Connecting Error イベントをListenerに転送
            GameController.Instance.ISC = tmpLevel;
        }
        else
        {
            userLevel = tmpLevel;
            //userLevel = 5;

            #if SHOW_DEBUG_MESSAGES
                Debug.Log("User Level : " + userLevel);
            #endif
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
        switch (Event_Type)
        {
            case EVENT_TYPE.SERVER_CONNECTED:
                UserLevelRequest();
                break;
            case EVENT_TYPE.ENTER_LEVEL:
                OnEnterLevel(Param);
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("GetUserLevel"))
                {
                    UserLevelCheck(Convert.ToInt32(((WWW)Param2).text));
                }
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// 入場するレベルとユーザーのレベルをチェック
    /// </summary>
    private void OnEnterLevel(object Level)
    {
        //  入場ができる場合
        if ((int)Level <= userLevel)
        {
            //  LevelScrollViewの画面に転換
            GameController.Instance.SN = "LevelScrollView";
        }
        else // 入場ができない場合
        {
            //   LevelWarningウィンドウを活性化
            GameObject.Find("Canvas").transform.Find("LevelWarning").gameObject.SetActive(true);
        }
    }
}
