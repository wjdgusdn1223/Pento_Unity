using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AllCollectionScrollView　Scene　Controller
/// </summary>
public class AllCollectionScrollViewController : MonoBehaviour, IListener {

    //---------------------------------------------------
    //  Collection Button Prefab Object
    public GameObject CollectionButton;

    //  Collection Button Prefab ObjectのParent Object
    public Transform Content;

    //  Imageをもらったかどうか
    private bool imageArrived = false;

    //  サーバーからもらったimage
    private Texture2D[] image;

　　//  Image Count
    private int imageNum = 0;

    //---------------------------------------------------
    /// <summary>
    /// enterCollection c# property
    /// </summary>
    public int EC
    {
        get { return enterCollection; }
        set
        {
            enterCollection = value;

            //  ENTER_COLLECTION イベントをListenerに転送
            EventManager.Instance.PostNotification(
                EVENT_TYPE.ENTER_COLLECTION, this, enterCollection);
        }
    }

    //-----------------------------------------------------
    //  入場するパズル
    private int enterCollection;
    
    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static AllCollectionScrollViewController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static AllCollectionScrollViewController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("AllCollectionScrollViewControllerを実行");
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
        //  ENTER_COLLECTION　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.ENTER_COLLECTION, this);
        //  SERVER_RESPONSE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);
        //  IMAGE_ARRIVED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.IMAGE_ARRIVED, this);

        //  サーバーにUserBuyList, CollectionInfoを要請・セーブ
        requestUserCollectionInfo();

        //  Sound Play
        SoundController.Instance.PlaySound("Title Screen");
    }

    //-------------------------------------------------
    /// <summary>
    /// サーバーにUserBuyListを要請
    /// </summary>
    private void requestUserCollectionInfo()
    {
        //  サーバーに会員番号を転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("user_no", GameData.userNum);

        ServerRequest.Instance.POST(GameData.UnityURL, "GetCollectionInfo", post);
    }

    //--------------------------------------------
    /// <summary>
    /// パズルの情報を処理
    /// </summary>
    /// <param name="wwwText">もらった情報</param>
    private void GetUserCollectionInfo(string wwwText)
    {
        //  例外処理
        if (wwwText.Trim().Equals("false"))
        {
            //  Server Connecting Error イベントをListenerに転送
            GameController.Instance.ISC = -1;
            
            return;
        }

        string[] tmpArray1 = wwwText.Trim().Split('*');

        string[,] tmpCollectionInfo = new string[tmpArray1.Length, 4];

        GameData.CollectionInfo = new string[tmpArray1.Length, 4];

        image = new Texture2D[tmpArray1.Length];

        //  情報をArrayに変換
        for (int i = 0; i < tmpArray1.Length; i++)
        {
            string[] tmpArray2 = tmpArray1[i].Split(',');

            for (int j = 0; j < tmpArray2.Length; j++)
            {
                tmpCollectionInfo[i, j] = tmpArray2[j];
            }
        }

        //  ArrayをGameDataにセーブ
        for (int i = 0; i < tmpCollectionInfo.GetLength(0); i++)
        {
            for (int j = 0; j < tmpCollectionInfo.GetLength(1); j++)
            {
                GameData.CollectionInfo[i, j] = tmpCollectionInfo[i,j];
            }
        }

#if SHOW_DEBUG_MESSAGES
        for (int i = 0; i < GameData.CollectionInfo.GetLength(0); i++)
        {
            Debug.Log("Collectionの番号 : " + GameData.CollectionInfo[i, 0]);
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
        for (imageNum = 0; imageNum < GameData.CollectionInfo.GetLength(0); imageNum++)
        {
            Dictionary<string, string> post = new Dictionary<string, string>();

            ServerRequest.Instance.POST(GameData.baseURL, GameData.CollectionInfo[imageNum, 1], post);

            yield return new WaitUntil(() => imageArrived == true);
            imageArrived = false;
        }
        
        //  リストに項目を追加
        CreateCollectionButton();
    }

    //-------------------------------------------
    /// <summary>
    /// Collection Buttonをリストに追加
    /// </summary>
    public void CreateCollectionButton()
    {
        for (int i = 0; i < GameData.CollectionInfo.GetLength(0); i++)
        {
            //  Button Prefab Object Create
            GameObject tmpObject = Instantiate(CollectionButton) as GameObject;

            //  Button Objectの内にあるScript
            CollectionPrefab tmpPrefab = tmpObject.GetComponent<CollectionPrefab>();

            //  名前を設定
            tmpObject.name = ("CollectionGame_" + GameData.CollectionInfo[i, 0]);

            //  Imageを出力
            Rect rect = new Rect(0, 0, image[i].width, image[i].height);

            tmpPrefab.image.sprite = Sprite.Create(image[i], rect, new Vector2(0.5f, 0.5f));

            //  title text　設定
            tmpPrefab.title.text = GameData.CollectionInfo[i, 2];

            //  writer 設定
            tmpPrefab.creator.text += GameData.CollectionInfo[i, 3];

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
                requestUserCollectionInfo();
                break;
            case EVENT_TYPE.ENTER_COLLECTION:
                OnEnterCollection();
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("GetCollectionInfo"))
                {
                    GetUserCollectionInfo(((WWW)Param2).text);
                }
                break;
            case EVENT_TYPE.IMAGE_ARRIVED:
                if (((string)Param).Equals(GameData.CollectionInfo[imageNum, 1]))
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
        //  CollectionGame 画面に転換
        GameController.Instance.SN = "CollectionGame";
    }
}
