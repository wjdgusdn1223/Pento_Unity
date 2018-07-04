using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// MainMenu Scene Controller
/// </summary>
public class MainMenuController : MonoBehaviour, IListener
{
    //-------------------------------------------------
    //  ID Input Field
    public InputField idInput;

    //  Password Input Field
    public InputField pwInput;

    //-------------------------------------------------
    /// <summary>
    /// isSerialNumidentified c# property
    /// </summary>
    public string ISNI
    {
        get { return isSerialNumidentified; }
        set
        {
            isSerialNumidentified = value;

            //  SERIAL_NUM_CHECK　イベントをListenerに転送
            EventManager.Instance.PostNotification(
                EVENT_TYPE.SERIAL_NUM_CHECK, this, isSerialNumidentified);
        }
    }

    //-------------------------------------------------
    //  シリアル番号のチェック結果
    private string isSerialNumidentified = string.Empty;

    //  シリアル番号をサーバーからもらったかどうか
    private bool isSerialNumArrived = false;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static MainMenuController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static MainMenuController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("MainMenuControllerを実行");
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

    //-------------------------------------------------
    // Use this for initialization
    void Start()
    {
        //  SERVER_CONNECTED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTED, this);
        //  SERVER_RESPONSE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);
        //  SERIAL_NUM_CHECK　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERIAL_NUM_CHECK, this);
        //  SERIAL_NUM_ARRIVED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERIAL_NUM_ARRIVED, this);
        //  LOGIN　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.LOGIN, this);
            
        //  MainMenuを活性化する前の初期作業
        StartCoroutine(StartMainMenu());

        //  Sound Play
        SoundController.Instance.PlaySound("Title Screen");
    }

    //------------------------------------------------
    /// <summary>
    /// MainMenuを活性化する前の初期作業
    /// </summary>
    IEnumerator StartMainMenu()
    {
        //  ボードとのコネクトを待つ
        yield return new WaitUntil(() => GameController.Instance.IBC == 1);

        //  シリアル番号のチェックが終わっていない場合
        if (GameData.SerialNum.Equals(string.Empty))
        {
            //  Loading Object Activate
            GameController.Instance.LoadingSwitch(true);

            //  シリアル番号を要請・チェック
            RequestSerialNum();

            //  シリアル番号のチェックを待つ
            yield return new WaitUntil(() => isSerialNumArrived == true);

            //  シリアル番号チェックの例外処理
            if (GameData.SerialNum.Equals("false"))
            {
                ISNI = "false";
            }
            else
            {
                CheckSerialNum(GameData.SerialNum);
            }
        }
        else // シリアル番号のチェックが終わった場合
        {
            //  Login Check
            if (GameData.userNum.Equals(string.Empty))
            {
                //  Loading Object Deactivate
                GameController.Instance.LoadingSwitch(false);

                //  Login ウィンドウを活性化
                GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(true);
            }
            else
            {
                //  MainMenu　ウィンドウを活性化
                GameObject.Find("Canvas").transform.Find("MainMenu").gameObject.SetActive(true);
            }
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Arduinoにシリアル番号を要請
    /// </summary>
    private void RequestSerialNum()
    {
        //  Arduinoにシリアル番号を要請
        StartCoroutine(BoardRequester.Instance.GetSerial_Num());
    }

    //-------------------------------------------------
    /// <summary>
    /// シリアル番号をサーバーに転送
    /// </summary>
    /// <param name="serialNum">シリアル番号</param>
    private void CheckSerialNum(string serialNum)
    {
        //  シリアル番号をサーバーに転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("serial_no", serialNum);

        ServerRequest.Instance.POST(GameData.UnityURL, "CheckSerial", post);
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
                //  SERVER_CONNECTEDの時requestをサーバーに再転送
                if (!GameData.tmpBaseURL.Equals(string.Empty))
                {
                    ServerRequest.Instance.POST(GameData.tmpBaseURL, GameData.tmpURL, GameData.tmpPost);
                }
                break;
            case EVENT_TYPE.SERIAL_NUM_CHECK:
                OnSerialNumCheck(Param);
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("CheckSerial"))
                {
                    ISNI = ((WWW)Param2).text.Trim();
                }
                else if (((string)Param).Equals("UserNum"))
                {
                    OnLoginCheck(((WWW)Param2).text.Trim());
                }
                break;
            case EVENT_TYPE.SERIAL_NUM_ARRIVED:
                //  もらったシリアル番号をセーブ
                GameData.SerialNum = Param.ToString();

                Debug.Log(GameData.SerialNum);

                isSerialNumArrived = true;
                break;
            case EVENT_TYPE.LOGIN:
                //  Login Check
                OnLogin();
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// 로그인 체크
    /// </summary>
    private void OnLoginCheck(string responseValue)
    {
        //  로그인 성공/실패 여부 체크
        if (!responseValue.Equals("false"))
        {
            //  로그인 성공시 유저 번호 저장
            GameData.userNum = responseValue;

            //  로그인 창 비활성화
            GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(false);

            //  메인 메뉴 활성화
            GameObject.Find("Canvas").transform.Find("MainMenu").gameObject.SetActive(true);
        }
        else
        {
#if SHOW_DEBUG_MESSAGES
            Debug.LogError("로그인 실패");
#endif

            //  로그인 창 비활성화
            GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(false);

            //  경고창 활성화
            GameObject.Find("Game Manager").transform.Find("Warning Canvas").transform.Find("LoginWarning").gameObject.SetActive(true);
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// 로그인 요청
    /// </summary>
    private void OnLogin(){
        //  시리얼 번호 서버에 전송 및 결과 반환
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("user_id", idInput.text);
        post.Add("user_pw", pwInput.text);

        Debug.Log(idInput.text);
        Debug.Log(pwInput.text);

        ServerRequest.Instance.POST(GameData.UnityURL, "UserNum", post);
    }

    //-------------------------------------------------
    /// <summary>
    /// 시리얼 번호 체크 결과에 따른 이벤트 처리
    /// </summary>
    /// <param name="CheckResult">시리얼 번호 체크 결과 값</param>
    private void OnSerialNumCheck(object CheckResult)
    {
        //  시리얼 번호 체크 실패
        if (CheckResult.ToString().Equals("false"))
        {
#if SHOW_DEBUG_MESSAGES
            Debug.LogError("시리얼 번호 체크 실패");
#endif

            //  경고창 및 게임 종료
            GameObject.Find("Game Manager").transform.Find("Warning Canvas").transform.Find("SerialWarning").gameObject.SetActive(true);
        }
        else    //  시리얼 번호 체크 성공
        {
#if SHOW_DEBUG_MESSAGES
            Debug.Log("시리얼 번호 체크 성공");
#endif

            //  로딩 비활성화
            GameController.Instance.LoadingSwitch(false);

            //  로그인 창 활성화
            GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(true);
        }
    }
}
