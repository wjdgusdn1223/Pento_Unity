using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// StoryScrollView Scene Controller
/// </summary>
public class StoryScrollViewController : MonoBehaviour,IListener {

    //---------------------------------------------------
    //  Story Button Prefab Object
    public GameObject StoryButton;

    //  Story Button Prefab ObjectのParent Object
    public Transform Content;

    //  Imageをもらったかどうか
    private bool imageArrived = false;

    //  サーバーからもらったimage
    private Texture2D[] image;

    //  Image Count
    private int imageNum = 0;
    //---------------------------------------------------
    /// <summary>
    /// enterStory c# property
    /// </summary>
    public int ES
    {
        get { return enterStory; }
        set
        {
            enterStory = value;

            //  ENTER_STORY イベントをListenerに転送
            EventManager.Instance.PostNotification(
                EVENT_TYPE.ENTER_STORY, this, enterStory);
        }
    }

    //-----------------------------------------------------
    //  User Story Buy List
    private string[] userStoryList;

    //  入場するSTORY
    private int enterStory;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryScrollViewController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryScrollViewController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryScrollViewControllerを実行");
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
        //  SERVER_CONNECTED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTED, this);
        //  ENTER_STORY　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.ENTER_STORY, this);
        //  SERVER_RESPONSE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);
        //  IMAGE_ARRIVED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.IMAGE_ARRIVED, this);

        //  サーバーにUserStoryBuyList・Story情報を要請・セーブ
        requestUserStoryInfo();

        //  Sound Play
        SoundController.Instance.PlaySound("Title Screen");
    }

    //-------------------------------------------------
    /// <summary>
    /// サーバーにUserStoryBuyListを要請
    /// </summary>
    private void requestUserStoryInfo()
    {
        //  会員番号を番号をサーバーに転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("user_no", GameData.userNum);

        ServerRequest.Instance.POST(GameData.UnityURL, "GetMyStoryNum", post);
    }

    //-------------------------------------------------
    /// <summary>
    /// サーバーにStoryの情報を要請
    /// </summary>
    private void requestStoryInfo()
    {
        //  サーバーにStoryの情報を要請
        Dictionary<string, string> post = new Dictionary<string, string>();

        ServerRequest.Instance.POST(GameData.UnityURL, "GetStoryInfo", post);

    }

    //--------------------------------------------
    /// <summary>
    /// Storyの情報を処理
    /// </summary>
    /// <param name="wwwText">サーバーからもらった情報</param>
    private void GetStoryInfo(string wwwText)
    {
        //  例外処理
        if (wwwText.Trim().Equals("false"))
        {
            //  Server Connecting Error イベントをListenerに転送
            GameController.Instance.ISC = -1;
        }

        string[] tmpArray1 = wwwText.Split('*');

        string[,] tmpStoryInfo = new string[tmpArray1.Length, 4];

        GameData.storyInfo = new string[tmpArray1.Length, 4];

        image = new Texture2D[tmpArray1.Length];

        //  情報をArrayに変換
        for (int i = 0; i < tmpArray1.Length; i++)
        {
            string[] tmpArray2 = tmpArray1[i].Split(',');

            for (int j = 0; j < tmpArray2.Length; j++)
            {
                tmpStoryInfo[i, j] = tmpArray2[j];
            }
        }

        //  ArrayをGameDataにセーブ
        for (int i = 0; i < tmpStoryInfo.GetLength(0); i++)
        {
            for (int j = 0; j < tmpStoryInfo.GetLength(1); j++)
            {
                GameData.storyInfo[i, j] = tmpStoryInfo[i, j];
            }
        }

#if SHOW_DEBUG_MESSAGES
        foreach (string storyNum in userStoryList)
        {
            Debug.Log("Story Num : " + storyNum);
        }
#endif      
        //  Imageをサーバーに要請
        StartCoroutine(requestImage());
    }

    //-------------------------------------------
    /// <summary>
    /// Imageをサーバーに要請
    /// </summary>
    IEnumerator requestImage()
    {
        //  Imageの住所を使ってImageをサーバーに要請
        for (imageNum = 0; imageNum < GameData.storyInfo.GetLength(0); imageNum++)
        {
            Dictionary<string, string> post = new Dictionary<string, string>();

            ServerRequest.Instance.POST(GameData.baseURL, GameData.storyInfo[imageNum, 2], post);

            yield return new WaitUntil(() => imageArrived == true);
            imageArrived = false;
        }
        //  リストに項目を追加
        CreateStoryButton();
    }

    //-------------------------------------------
    /// <summary>
    /// Story Buttonをリストに追加
    /// </summary>
    public void CreateStoryButton()
    {
        for (int i = 0; i < GameData.storyInfo.GetLength(0); i++)
        {
            //  Button Prefab Object Create
            GameObject tmpObject = Instantiate(StoryButton) as GameObject;

            //  Button Objectの内にあるScript
            StoryPrefab tmpPrefab = tmpObject.GetComponent<StoryPrefab>();

            //  名前を設定
            tmpObject.name = ("StoryGame_" + GameData.storyInfo[i, 0]);

            //  title text　設定
            tmpPrefab.title.text = GameData.storyInfo[i, 1];

            //  Imageを出力
            Rect rect = new Rect(0, 0, image[i].width, image[i].height);

            tmpPrefab.image.sprite = Sprite.Create(image[i], rect, new Vector2(0.5f, 0.5f));

            //  Summarize 設定
            tmpPrefab.summary.text = GameData.storyInfo[i, 3];

            //  Parent Objectを設定
            tmpObject.transform.SetParent(Content);

            //  サイズを１に設定
            tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);
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
                requestUserStoryInfo();
                break;
            case EVENT_TYPE.ENTER_STORY:
                OnEnterStory(Param);
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("GetStoryInfo"))
                {
                    GetStoryInfo(((WWW)Param2).text);
                }
                else if (((string)Param).Equals("GetMyStoryNum"))
                {
                    //  받은 구독 정보 배열로서 반환
                    userStoryList = ((WWW)Param2).text.Trim().Split(',');

                    requestStoryInfo();
                }
                break;
            case EVENT_TYPE.IMAGE_ARRIVED:
                if (((string)Param).Equals(GameData.storyInfo[imageNum, 2]))
                {
                    image[imageNum] = ((WWW)Param2).texture;

                    imageArrived = true;
                }
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Storyゲームに入るイベント
    /// </summary>
    /// <param name="storyNum">Storyの番号</param>
    private void OnEnterStory(object storyNum)
    {
        //  入場ができる場合 
        if (Array.Exists(userStoryList, element => element.Equals(storyNum.ToString())))
        {
            //  StoryGame 画面に転換
            GameController.Instance.SN = "StoryGame";
        }
        else // 入場ができない場合 
        {
            //   StoryWarning　ウィンドウを活性化
            GameObject.Find("Canvas").transform.Find("StoryWarning").gameObject.SetActive(true);
        }
    }
}
