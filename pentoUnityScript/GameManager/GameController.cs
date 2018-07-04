using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

//-------------------------------------------------
/// <summary>
/// ゲームのController
/// </summary>
public class GameController : MonoBehaviour, IListener {

    //-------------------------------------------------
    //  Loading Object
    public GameObject loading;

    //-------------------------------------------------
    /// <summary>
    /// isPlayingの C# property
    /// </summary>
    public int IP
    {
        get { return isPlaying; }
        set
        {
            isPlaying = Mathf.Clamp(value, -1, 1);

            //  IS_PLAYINGイベントを転送する
            EventManager.Instance.PostNotification
                (EVENT_TYPE.IS_PLAYING, this, isPlaying);
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// isBoardConnectingの C# property
    /// </summary>
    public int IBC
    {
        get { return isBoardConnecting; }
        set
        {
            isBoardConnecting = Mathf.Clamp(value, -1, 1);

            //  BOARD_CONNECTING_CHECKイベントを転送する
            EventManager.Instance.PostNotification
                (EVENT_TYPE.BOARD_CONNECTING_CHECK, this, isBoardConnecting);
        }
    }
    //-------------------------------------------------
    /// <summary>
    /// isServerConnectingの C# property
    /// </summary>
    public int ISC
    {
        get { return isServerConnecting; }
        set
        {
            isServerConnecting = Mathf.Clamp(value, -1, 1);

            //  SERVER_CONNECTING_CHECKイベントを転送する
            EventManager.Instance.PostNotification(
                EVENT_TYPE.SERVER_CONNECTING_CHECK, this, isServerConnecting);
        }
    }
    //-------------------------------------------------
    /// <summary>
    /// sceneNameの C# property
    /// </summary>
    public string SN
    {
        get { return sceneName; }
        set
        {
            sceneName = value;

            //  SCENE_CHANGEイベントを転送する
            EventManager.Instance.PostNotification(
                EVENT_TYPE.SCENE_CHANGE, this, sceneName);
        }
    }
    //-------------------------------------------------
    //  Arduinoとのコネクトの状態
    private int isBoardConnecting = 0;

    //  サーバーとのコネクトの状態
    private int isServerConnecting = 0;

    //  シリアル番号が有効かどうか
    private int isPlaying = 0;

    //  現在のScene Name
    private string sceneName = string.Empty;

    //  ローディングしているかどうか
    private bool isLoading = false;

    //-------------------------------------------------
    // Use this for initialization
    private void Awake () {
        //  sceneにinstanceが存在するかを検査
        //  存在する場合,消滅させる。
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        //  このinstanceを唯一のobjectにする。
        instance = this;

        //  画面が変えても消滅しない
        DontDestroyOnLoad(gameObject);
    }

    //-------------------------------------------------
    // Use this for initialization
    private void Start()
    {
        //  IS_PLAYING イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.IS_PLAYING, this);
        //  BOARD_CONNECTING_CHECK イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.BOARD_CONNECTING_CHECK, this);
        //  SERVER_CONNECTING_CHECK イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTING_CHECK, this);
        //  SERVER_ERROR イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_ERROR, this);
        //  SCENE_CHANGE イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.SCENE_CHANGE, this);
        //  SAVE_FAILED イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.SAVE_FAILED, this);

        //  IS_PLAYINGイベント
        IP = 1;

        //  ボードにコネクトする
        if (!BoardRequester.Instance.EnterTheArduino())
        {
            //  コネクトに失敗
            IBC = -1;
        }
        else
        {
            //  コネクト完了
            IBC = 1;
        }
    }

    //-------------------------------------------------
    public static GameController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }
    //-------------------------------------------------
    private static GameController instance = null;

    //-------------------------------------------------
    /// <summary>
    /// Loading Object 活性化・非活性化
    /// </summary>
    /// <param name="setActive">活性化・非活性化</param>
    public void LoadingSwitch(bool setActive){
        if (setActive)
        {
            //  ローディング中
            isLoading = true;

            //  Create blankBlock object 
            GameObject tmpObject = Instantiate(loading) as GameObject;
            tmpObject.GetComponent<Renderer>().sortingOrder = 2;

            //  Parent Object 設定
            tmpObject.transform.SetParent(Camera.main.transform);

            //  座標を設定
            tmpObject.transform.localPosition = new Vector3(0, -0.55f, 2);

            //  大きさを設定
            tmpObject.transform.localScale = new Vector3(1, 1, 1);

            //  名前を設定
            tmpObject.name = "Loading";

            //  rotationを設定
            tmpObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            //  Loading object を破壊
            Destroy(Camera.main.transform.Find("Loading").gameObject);

            //  ローディング完了
            isLoading = false;
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
            case EVENT_TYPE.SERVER_ERROR:
           
            #if SHOW_DEBUG_MESSAGES
                Debug.LogError("Server Error");
#endif

                //  Server Error　ウィンドウを活性化
                transform.Find("Warning Canvas").transform.Find("ServerErrorWarning").gameObject.SetActive(true);
                break;
            case EVENT_TYPE.BOARD_CONNECTING_CHECK:
                //  ボードとのコネクトをチェック
                OnBoardConnectingCheck(Param);
                break;
            case EVENT_TYPE.SERVER_CONNECTING_CHECK:
                //  さーばーとのコネクトをチェック
                OnServerConnectingCheck(Param);
                break;
            case EVENT_TYPE.IS_PLAYING:
                //  ゲームの実行・終了
                IsPlaying(Param);
                break;
            case EVENT_TYPE.SCENE_CHANGE:
                //  Fade Effectをチェック
                FadeEffectCheck(Param);
                break;
            case EVENT_TYPE.SAVE_FAILED:
                
#if SHOW_DEBUG_MESSAGES
                Debug.LogError("세이브 실패");
#endif

                //  Save Failed　ウィンドウを活性化
                transform.Find("Warning Canvas").transform.Find("SaveWarning").gameObject.SetActive(true);
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Board Connectingチェック
    /// </summary>
    /// <param name="isConnecting">ボードとコネクトしているかどうか</param>
    private void OnBoardConnectingCheck(object isConnecting)
    {
        //  コネクトに失敗
        if ((int)isConnecting == -1)
        {
            #if SHOW_DEBUG_MESSAGES
                Debug.LogError("ボードとのコネクトが切れる");
            #endif

            //  Game Pause
            Time.timeScale = 0;
            //  BoardWarning　ウィンドウを活性化
            transform.Find("Warning Canvas").transform.Find("BoardWarning").gameObject.SetActive(true);
        }
        else    //  ボードにコネクト完了
        {
            #if SHOW_DEBUG_MESSAGES
                Debug.Log("ボードにコネクト完了");
            #endif

            //  ゲーム進行
            Time.timeScale = 1;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Server Connectingチェック
    /// </summary>
    /// <param name="isConnecting">サーバーとコネクトしているかどうか</param>
    private void OnServerConnectingCheck(object isConnecting)
    {
        //  コネクトに失敗
        if ((int)isConnecting == -1)
        {
            #if SHOW_DEBUG_MESSAGES
                Debug.LogError("サーバーとのコネクトが切れる");
#endif

            //  ServerWarning　ウィンドウを活性化 
            transform.Find("Warning Canvas").transform.Find("ServerWarning").gameObject.SetActive(true);
        }
        else    //  サーバーにコネクト完了
        {
            #if SHOW_DEBUG_MESSAGES
                Debug.Log("サーバーにコネクト完了");
#endif

            //  SERVER_CONNECTEDイベントを転送
            EventManager.Instance.PostNotification(EVENT_TYPE.SERVER_CONNECTED, this);
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// ゲーム実行・終了イベント
    /// </summary>
    private void IsPlaying(object ip)
    {
        //実行状態
        if ((int)ip == 1)
        {
            #if SHOW_DEBUG_MESSAGES
                Debug.Log("ゲーム実行");
            #endif
        }
        else // Game End イベント
        {
            //  Arduinoとのコネクトを切る
            BoardRequester.Instance.OutTheArduino();

            #if SHOW_DEBUG_MESSAGES
                Debug.Log("ゲーム終了");
#endif

            //  ゲーム終了
            //  Editorの場合、pause
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                //アプリの場合、ゲーム終了
                Application.Quit(); 
            #endif
        }
    }

    //  Fade Effect Check
    private void FadeEffectCheck(object SceneName)
    {
        //  特定の画面だけEffectを実行
        if (SceneManager.GetActiveScene().buildIndex == 10 ||
            SceneManager.GetActiveScene().buildIndex == 9 ||
            SceneManager.GetActiveScene().buildIndex == 8 ||
            SceneManager.GetActiveScene().buildIndex == 6 ||
            SceneManager.GetActiveScene().buildIndex == 7)
        {
            //  Effectが進行中ならreturn
            if (GameObject.Find("Canvas").transform.Find("Fade Effect").GetComponent<FadeEffect>().isFading)
                return;

            GameObject.Find("Canvas").transform.Find("Fade Effect").GetComponent<FadeEffect>().StartFade(1f);

            int[,] zeroXY = new int[,]
            {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            };

            //  座標をボードに転送
            BoardController.Instance.SendCoordinateToBoard(zeroXY);

            StartCoroutine(ChangeScene(SceneName, 1.3f));
        }
        else
        {
            StartCoroutine(ChangeScene(SceneName, 0f));
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Scene Change
    /// </summary>
    IEnumerator ChangeScene(object SceneName, float time)
    {
        //  delay time
        yield return new WaitForSeconds(time);

        //  parameterを使ってScene Change
        SceneManager.LoadScene(SceneName.ToString());
    }

    //-------------------------------------------------
    /// <summary>
    /// Objectが破壊された時、実行される
    /// </summary>
    private void OnDestroy() {
        //  Arduinoとのコネクトを切る
        BoardRequester.Instance.OutTheArduino();
    }
}
