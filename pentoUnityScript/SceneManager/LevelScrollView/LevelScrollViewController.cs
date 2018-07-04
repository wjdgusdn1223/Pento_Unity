using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LevelScrollView Scene Controller
/// </summary>
public class LevelScrollViewController : MonoBehaviour, IListener
{

    //---------------------------------------------------
    //  Puzzle Button Prefab Object
    public GameObject PuzzleButton;

    //  Puzzle Button Prefab ObjectのParent Object
    public Transform Content;

    //  Imageをもらったかどうか
    private bool imageArrived = false;

    //  サーバーからもらったimage
    private Texture2D[] image;

    //  Image Count
    private int imageNum = 0;
    //---------------------------------------------------
    /// <summary>
    /// enterPuzzle c# property
    /// </summary>
    public int EP
    {
        get { return enterPuzzle; }
        set
        {
            enterPuzzle = value;

            //  ENTER_PUZZLE イベントをListenerに転送
            EventManager.Instance.PostNotification(
                EVENT_TYPE.ENTER_PUZZLE, this, enterPuzzle);
        }
    }

    //-----------------------------------------------------
    //  入場するパズル
    private int enterPuzzle;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static LevelScrollViewController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static LevelScrollViewController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("LevelScrollViewControllerを実行");
        Debug.Log("Level : " + GameData.LevelNum);
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
        //SoundController.Instance.StopSound("test");

        //  SERVER_CONNECTED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTED, this);
        //  ENTER_PUZZLE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.ENTER_PUZZLE, this);
        //  SERVER_RESPONSE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);
        //  IMAGE_ARRIVED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.IMAGE_ARRIVED, this);

        //  サーバーに今のレベルのパズル情報を要請・セーブ
        requestPuzzleInfo();

        //  Sound Play
        SoundController.Instance.PlaySound("Title Screen");
    }

    //-------------------------------------------------
    /// <summary>
    /// サーバーにUserBuyListを要請
    /// </summary>
    private void requestPuzzleInfo()
    {
        //  レベルの番号をサーバーに転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("level", GameData.LevelNum.ToString());

        ServerRequest.Instance.POST(GameData.UnityURL, "GetLevelInfo", post);
    }

    //--------------------------------------------
    /// <summary>
    /// パズルの情報を処理
    /// </summary>
    /// <param name="wwwText">サーバーからもらった情報</param>
    private void GetLevelInfo(string wwwText)
    {
        //  例外処理
        if (wwwText.Trim().Equals("false"))
        {
            //  Server Connecting Error イベントをListenerに転送
            GameController.Instance.ISC = -1;
            return;
        }

        string[] tmpArray1 = wwwText.Split('*');

        string[,] tmpLevelInfo = new string[tmpArray1.Length, 4];

        GameData.LevelInfo = new string[tmpArray1.Length, 4];

        image = new Texture2D[tmpArray1.Length];

        //  情報をArrayに変換
        for (int i = 0; i < tmpArray1.Length; i++)
        {
            string[] tmpArray2 = tmpArray1[i].Split(',');

            for (int j = 0; j < tmpArray2.Length; j++)
            {
                tmpLevelInfo[i, j] = tmpArray2[j];
            }
        }
    
        //  ArrayをGameDataにセーブ
        for (int i = 0; i < tmpLevelInfo.GetLength(0); i++)
        {
            for (int j = 0; j < tmpLevelInfo.GetLength(1); j++)
            {
                GameData.LevelInfo[i, j] = tmpLevelInfo[i, j];
            }
        }

#if SHOW_DEBUG_MESSAGES
        for (int i = 0; i < GameData.LevelInfo.GetLength(0); i++)
        {
            Debug.Log("Level " + GameData.LevelNum + " のパズル : " + GameData.LevelInfo[i, 0]);
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
        for (imageNum = 0; imageNum < GameData.LevelInfo.GetLength(0); imageNum++)
        {
            Dictionary<string, string> post = new Dictionary<string, string>();

            ServerRequest.Instance.POST(GameData.baseURL, GameData.LevelInfo[imageNum, 2], post);

            yield return new WaitUntil(() => imageArrived == true);
            imageArrived = false;
        }
        //  リストに項目を追加
        CreatePuzzleButton();
    }

    //-------------------------------------------
    /// <summary>
    /// Puzzle Buttonをリストに追加
    /// </summary>
    public void CreatePuzzleButton()
    {
        for (int i = 0; i < GameData.LevelInfo.GetLength(0); i++)
        {
            //  Button Prefab Object Create
            GameObject tmpObject = Instantiate(PuzzleButton) as GameObject;

            //  Button Objectの内にあるScript
            PuzzlePrefab tmpPrefab = tmpObject.GetComponent<PuzzlePrefab>();

            //  名前を設定
            tmpObject.name = ("PuzzleGame_" + GameData.LevelInfo[i, 0]);

            //  Imageを出力
            Rect rect = new Rect(0, 0, image[i].width, image[i].height);

            tmpPrefab.image.sprite = Sprite.Create(image[i], rect, new Vector2(0.5f, 0.5f));

            //  title text　設定
            tmpPrefab.title.text = GameData.LevelInfo[i, 1];

            //  Summarize 設定
            tmpPrefab.summary.text += GameData.LevelInfo[i, 3];

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
                requestPuzzleInfo();
                break;
            case EVENT_TYPE.ENTER_PUZZLE:
                OnEnterCollection();
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("GetLevelInfo"))
                {
                    GetLevelInfo(((WWW)Param2).text.Trim());
                }
                break;
            case EVENT_TYPE.IMAGE_ARRIVED:
                if (((string)Param).Equals(GameData.LevelInfo[imageNum, 2]))
                {
                    image[imageNum] = ((WWW)Param2).texture;

                    imageArrived = true;
                }
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// パズルゲームに入るイベント
    /// </summary>
    private void OnEnterCollection()
    {
        //  LevelGame 画面に転換
        GameController.Instance.SN = "LevelGame";
    }
}
