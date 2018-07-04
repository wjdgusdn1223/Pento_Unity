using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// パズルゲームのController
/// </summary>
public class PentoGameController : MonoBehaviour, IListener
{
    //  パズルの座標
    private int[,] blockXY = new int[15,15];

    //  ボードの座標
    private int[,] boardXY = new int[15, 15];

    //  座標の臨時変数
    private int[,] tmpXY = new int[GameData.BoardSize, GameData.BoardSize];

    //  パズルのサイズ
    private int[] puzzleSize;

    //  Blockのサイズ
    private float blockSize;

    //  パズルの番号
    private int puzzleNum;

    //  ゲームのタイプ
    private string type;

    //  パズルの座標を求めたかどうか
    private bool CoordinateArrived = false;

    //  カメラを目標の座標に移動したか
    private bool isCameraReady = false;

    //  ボードがらのresponse
    private bool boardResponsed = false;

    //  wrong block effectが終わったかどうか
    private bool wrongEffectOver = true;

    //  Title input field
    public InputField titleInput;

    //  Summarize input field
    public InputField summarizeInput;

    //  block input count
    private int blockInputCount = 1;

    //  セーブが終わったかどうか
    private bool isSaveComplete = false;

    //-------------------------------------------------
    /// <summary>
    /// blockInputCount c# property
    /// </summary>
    public int BIC
    {
        get { return blockInputCount; }
        set
        {
            blockInputCount = value;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// blockSize c# property
    /// </summary>
    public float BS
    {
        get { return blockSize; }
        set
        {
            blockSize = value;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// wrongEffectOver c# property
    /// </summary>
    public bool WEO
    {
        get { return wrongEffectOver; }
        set
        {
            wrongEffectOver = value;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// isComplete c# property
    /// </summary>
    public int IC
    {
        get { return isComplete; }
        set
        {
            isComplete = Mathf.Clamp(value, 0, 1);

            if (isComplete == 1)
            {
                //  パズルのサイズ
                int blockLength = 0;
                
                //  パズルのサイズを計算
                if (!type.Equals("Create"))
                {
                    foreach(int item in puzzleSize)
                    {
                        if (blockLength < item)
                            blockLength = item;
                    }
                }
                else
                {
                    blockLength = 15;
                }

                //  PUZZLE_COMPLETE　イベントをListenerに転送
                EventManager.Instance.PostNotification(
                    EVENT_TYPE.PUZZLE_COMPLETE, this, type, puzzleNum + "_" + blockLength + "_" + blockSize);
            }
        }
    }
    //-----------------------------------------------------
    //  パズルを完成させたかどうか
    private int isComplete = 0;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static PentoGameController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }
    public float a = 4;

    //-----------------------------------------------------
    private static PentoGameController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("PentoGameControllerを実行");
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
        //  PUZZLE_COMPLETE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.PUZZLE_COMPLETE, this);

        //  SAVE_RECORD イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SAVE_RECORD, this);

        //  SAVE_PENTO イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SAVE_PENTO, this);

        //  CREATE_COMPLETE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.CREATE_COMPLETE, this);

        //  CAMERA_SETTING_COMPLETE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.CAMERA_SETTING_COMPLETE, this);

        //  BOARD_RESPONSE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.BOARD_RESPONSE, this);

        //  SERVER_CONNECTED イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_CONNECTED, this);

        //  SERVER_RESPONSE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SERVER_RESPONSE, this);

        //  SPECIAL_COORDINATE_RESPONSE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SPECIAL_COORDINATE_RESPONSE, this);

        //  座標のArrayを０にリセット
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                tmpXY[i, j] = 0;
            }
        }
    }

    //---------------------------------------------------
    /// <summary>
    /// パズルゲームを実行
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <param name="blockSize">Blockのサイズ</param>
    /// <param name="puzzleCoordinate">パズルの座標</param>
    /// <param name="rotateValue">パズルの角度</param>
    public void startPentomino(string type, int puzzleNum, float blockSize, Vector3 puzzleCoordinate, Vector3 puzzleRotateValue)
    {
        //  Puzzle Complete　リセット
        IC = 0;

        //  Block Input Count リセット
        BIC = 1;

        //  ゲームのタイプをセーブ
        this.type = type;

        //  パズルの番号をセーブ
        this.puzzleNum = puzzleNum;

        //  Blockのサイズをセーブ
        this.blockSize = blockSize;

        //  パズルゲームを実行
        StartCoroutine(PentoMinoController(puzzleCoordinate, puzzleRotateValue));
    }

    //---------------------------------------------------
    /// <summary>
    /// パズルゲーム Thread
    /// </summary>
    /// <param name="puzzleCoordinate">パズルの座標</param>
    /// <param name="puzzleRotateValue">パズルの角度</param>
    IEnumerator PentoMinoController(Vector3 puzzleCoordinate, Vector3 puzzleRotateValue)
    {
        //  パズルの座標をサーバーに要請
        BoardController.Instance.GetCoordinateValue(puzzleNum);

        //  パズルの座標を待つ
        yield return new WaitUntil(() => CoordinateArrived == true);
        CoordinateArrived = false;

        //  パズルのParent Objectを設定
        BlockController.Instance.PuzzlePrefabCreate(puzzleNum, puzzleCoordinate, puzzleRotateValue, blockSize);

        //  カメラの座標を設定
        puzzleSize = CameraController.Instance.CameraSetter(type, blockXY, blockSize, puzzleCoordinate, puzzleRotateValue);

        //  タイマーを実行、Create Modeは例外
        if (!type.Equals("Create"))
        {
            TimerController.Instance.StartTimer(puzzleSize, puzzleNum, puzzleRotateValue);
        }

        //  Progressを実行、Create Modeは例外
        if (!type.Equals("Create"))
        {
            ProgressController.Instance.StartProgress(puzzleSize, blockXY, blockSize, puzzleNum);
        }

        if (BoardRequester.Instance.IsConnect())
        {
            //  座標をArduinoに転送
            BoardController.Instance.SendCoordinateToBoard(blockXY);
        }
        else
        {
            //  Board Connecting Error イベントをListenerに転送
            GameController.Instance.IBC = -1;

            //  Board Connecting trueを待つ
            yield return new WaitUntil(() => GameController.Instance.IBC == 1);

            //  座標をArduinoに転送
            BoardController.Instance.SendCoordinateToBoard(blockXY);
        }

        //  パズルの座標を宣言
        boardXY = new int[,]
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

        //  カメラの移動が終わるまでBlankBlockを出力
        BlockController.Instance.PrintBlankCube(type, blockXY, boardXY, puzzleNum);

        //  カメラの移動を待つ
        yield return new WaitUntil(() => isCameraReady == true);

        if (type.Equals("Tutorial"))
        {
            // STEP_COMPLETE　イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
        }
        isCameraReady = false;

        //  Create modeだけ
        if (type.Equals("Create"))
        {
            //  完了Buttonを活性化
            transform.Find("Pento Canvas").transform.Find("Complete_Button").gameObject.SetActive(true);
        }

        int i = 1;

        while (IC == 0)
        {
            //  ボードの座標を求める
            if (BoardRequester.Instance.IsConnect())
            {
                BoardController.Instance.GetBoardBlock();
            }
            else
            {
                //  Board Connecting Error イベントをListenerに転送
                GameController.Instance.IBC = -1;

                yield return new WaitUntil(() => GameController.Instance.IBC == 1);

                //  ボードの座標を求める
                BoardController.Instance.GetBoardBlock();
            }

            //  ボードのresponseを待つ
            yield return new WaitUntil(() => boardResponsed == true);
            boardResponsed = false;

            //  パズルの座標を使ってPentoBlockを出力
            BlockController.Instance.PrintBlankCube(type, blockXY, boardXY, puzzleNum);

            //  ボードの座標を使ってPentoBlockを出力
            BlockController.Instance.PrintPuzzleCube(type, boardXY, puzzleNum);

            //  Block Check, Create modeは例外
            if (!type.Equals("Create"))
            {
                //  パズルの完成・ミスをチェック
                PuzzleBlockChecker(boardXY);
            }

            //  wrong block Effectが終わるまで待つ
            yield return new WaitUntil(() => wrongEffectOver == true);
        }

        if (type.Equals("Create"))
        {
            //  すべてのBlankBlockを削除
            BlockController.Instance.DestroyBlankCube();
        }
    }

    //-------------------------------------------------------------
    /// <summary>
    /// パズルゲームの状態をチェック
    /// </summary>
    /// <param name="type">げーむのタイプ</param>
    /// <param name="boardXY">ボードの座標</param>
    private void PuzzleBlockChecker(int[,] boardXY)
    {
        //  Puzzle Complete Check
        bool equal = boardXY.Rank == blockXY.Rank &&
            Enumerable.Range(0, boardXY.Rank).All(dimension =>
            boardXY.GetLength(dimension) == blockXY.GetLength(dimension)) &&
            boardXY.Cast<int>().SequenceEqual(blockXY.Cast<int>());

        if (equal)
        {
#if SHOW_DEBUG_MESSAGES
            Debug.Log("Puzzle Complete!!");
#endif

            if (type.Equals("Tutorial"))
            {
                //  TUTORIAL_STEP_COMPLETE イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 3);
            }

            //  Puzzle Complete　イベントをListenerに転送
            IC = 1;
            return;
        }

        //  Block Check
        int checkStatus = 0;

        //  Input Block 座標
        int[,] inputXY = new int[GameData.BoardSize, GameData.BoardSize];

        //  以前の座標と比較して変わった座標を探す
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (tmpXY[i, j] == 0 && boardXY[i, j] == 1)
                {
                    inputXY[i, j] = 1;

                    checkStatus = 1;
                }
                else if (tmpXY[i, j] == 1 && boardXY[i, j] == 0)
                {
                    inputXY[i, j] = 0;

                    checkStatus = 1;
                }
                else
                {
                    inputXY[i, j] = 0;
                }
            }
        }

        //  変わった部分があったら、BLOCK_CHANGEDいべんとをListenerに転送
        if (checkStatus == 1)
        {
            EventManager.Instance.PostNotification(
                        EVENT_TYPE.BLOCK_CHANGED, this, boardXY);

            //  block input count ++
            BIC++;
        }

        //  変わった座標とパズルの座標を比較
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (inputXY[i, j] == 1 && blockXY[i, j] == 0)
                {
                    BlockController.Instance.InputEffect(1, "", 1f * blockSize, i, j, puzzleNum, blockSize);

                    //  wrong block
                    checkStatus = 2;
                }
                else if (inputXY[i, j] == 1 && blockXY[i, j] == 1)
                {   
                    //  right place Effect
                    BlockController.Instance.InputEffect(17, "", 1f * blockSize, i, j, puzzleNum, blockSize);
                }
            }
        }

        //  間違った時イベントを転送
        if (checkStatus == 2)
        {
#if SHOW_DEBUG_MESSAGES
            Debug.Log("Wrong Block !!");
#endif

            SoundController.Instance.PlayEffectSound("Wrong");

            //  WRONG_BLOCK　イベントをListenerに転送
            wrongEffectOver = false;
            EventManager.Instance.PostNotification(EVENT_TYPE.WRONG_BLOCK, this);

            if (type.Equals("Tutorial"))
            {
                //  TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 2);
            }
        }
        else if(checkStatus == 1)
        {
            SoundController.Instance.PlayEffectSound("Correct");

            if (type.Equals("Tutorial"))
            {
                //  TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 1);
            }
        }

        //  現在の座標を臨時の座標のArrayにセーブ
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                tmpXY[i, j] = boardXY[i, j];
            }
        }
    }

    //-------------------------------------------------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            xxx = "1";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            xxx = "2";
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            xxx = "3";
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            xxx = "4";
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            xxx = "5";
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            xxx = "6";
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            xxx = "7";
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            xxx = "8";
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            xxx = "9";
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            xxx = "q";
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            xxx = "w";
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            xxx = "e";
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            xxx = "r";
        }
    }

    //-------------------------------------------------------------
    /// <summary>
    /// セーブButtonを活性化
    /// </summary>
    private void SaveButtonActivate()
    {
        //  セーブButtonを活性化
        transform.Find("Pento Canvas").transform.Find("Save_Button").gameObject.SetActive(true);
    }

    //------------------------------------------------------------
    /// <summary>
    /// 記録をセーブ
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    private void SaveRecord(string type)
    {
        //  Tutorial / Create Modeではない場合
        if (!type.Equals("Create") && !type.Equals("Tutorial"))
        {
            //  Record Time 
            int timeRecord = TimerController.Instance.getTime();

#if SHOW_DEBUG_MESSAGES
            Debug.Log("Record Time : " + timeRecord + " second");
#endif

            //  色の座標
            string ColorCoordinate = BlockController.Instance.GetCreateCoordinate(true, puzzleNum);

            //  Block Input Count
            Debug.Log("Block Input Count : " + BIC);

            //  サーバーに会員番号、パズルの番号、RecordTime、色の座標、BlockInputCountを転送
            Dictionary<string, string> post = new Dictionary<string, string>();
            post.Add("location_color", ColorCoordinate);
            post.Add("user_no", GameData.userNum);
            post.Add("design_no", puzzleNum.ToString());
            post.Add("put_num", BIC.ToString());
            post.Add("clear_time", timeRecord.ToString());
            
            ServerRequest.Instance.POST(GameData.UnityURL, "SaveLocationColor", post);
        }

        switch (type)
        {
            case "Puzzle":
                //  Save Completeを待つ
                StartCoroutine(WaitUntilSave("Record"));
                break;
            case "Story":
                //  Save Completeを待つ
                StartCoroutine(WaitUntilSave("Story"));
                break;
            case "Create":
                //  Save Panelを活性化
                transform.Find("Pento Canvas").transform.Find("PentoSave").gameObject.SetActive(true);
                break;
            case "Tutorial":
                // TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 7);
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Create Mode Complete
    /// </summary>
    private void CreateComplete()
    {
        //  Puzzle Complete イベントを転送
        IC = 1;
    }

    //-------------------------------------------------
    /// <summary>
    /// パズルをセーブ
    /// </summary>
    private void SavePento()
    {
        Debug.Log(titleInput.text);
        Debug.Log(summarizeInput.text);

        //  パズルの座標を求める
        string PentoCoordinate = BlockController.Instance.GetCreateCoordinate(false, puzzleNum);
        Debug.Log(PentoCoordinate);
        
        //  色の座標を求める
        string ColorCoordinate = BlockController.Instance.GetCreateCoordinate(true, puzzleNum);
        Debug.Log(ColorCoordinate);

        //  会員番号、Title, Summarize, パズルの座標、色の座標をサーバーに転送
        Dictionary<string, string> post = new Dictionary<string, string>();
        post.Add("location_default", PentoCoordinate);
        post.Add("location_color", ColorCoordinate);
        post.Add("design_title", titleInput.text);
        post.Add("design_explain", summarizeInput.text);
        post.Add("user_no", GameData.userNum);
        
        ServerRequest.Instance.POST(GameData.UnityURL, "SaveLocationDefault", post);

        //  Save Completeを待つ
        StartCoroutine(WaitUntilSave("Puzzle"));
    }

    //-------------------------------------------------
    /// <summary>
    /// Save Completeを待つ
    /// </summary>
    /// <param name="saveType">セーブのタイプ</param>
    IEnumerator WaitUntilSave(string saveType)
    {
        //  Save Completeを待つ
        yield return new WaitUntil(() => isSaveComplete == true);
        isSaveComplete = false;

        //  セーブのタイプによるアルゴリズム
        switch (saveType)
        {
            case "Puzzle":
                //  FreeModeMenuに転換
                GameController.Instance.SN = "FreeModeMenu";
                break;
            case "Record":
                //  がめんによって違う画面に転換
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 7:
                        GameController.Instance.SN = "AllCollectionScrollView";
                        break;
                    case 8:
                        GameController.Instance.SN = "LevelScrollView";
                        break;
                    case 9:
                        GameController.Instance.SN = "MainMenu";
                        break;
                    default:
                        GameController.Instance.SN = "MainMenu";
                        break;
                } 
                break;
            case "Story":
                //  Progress Stop
                ProgressController.Instance.StopProgress();

                //  UI Object Destroy
                Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Timer").gameObject);
                Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Progress").gameObject);

                //  Save Buttonを非活性化
                transform.Find("Pento Canvas").transform.Find("Save_Button").gameObject.SetActive(false);

                // STEP_COMPLETE　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
                break;
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
                //  requestを再要請
                if (!GameData.tmpBaseURL.Equals(string.Empty))
                {
                    ServerRequest.Instance.POST(GameData.tmpBaseURL, GameData.tmpURL, GameData.tmpPost);
                }
                break;
            case EVENT_TYPE.PUZZLE_COMPLETE:
                SoundController.Instance.PlayEffectSound("Clap");

                //  LeapMotion Modeを実行
                if (type.Equals("Create"))
                {
                    LeapMotionController.Instance.LeapMotionActivate(puzzleNum, new int[] {15, 15}, type);
                }
                else
                {
                    LeapMotionController.Instance.LeapMotionActivate(puzzleNum, puzzleSize, type);
                }

                //  Save Buttonを活性化
                SaveButtonActivate();
                break;
            case EVENT_TYPE.SAVE_RECORD:
                //  記録をセーブ
                SaveRecord(type);
                break;
            case EVENT_TYPE.SAVE_PENTO:
                //  パズルをセーブ
                SavePento();
                break;
            case EVENT_TYPE.CREATE_COMPLETE:
                //  Puzzle Create Complete
                CreateComplete();
                break;
            case EVENT_TYPE.CAMERA_SETTING_COMPLETE:
                //  Camera Setting Complete
                isCameraReady = true;
                break;
            case EVENT_TYPE.BOARD_RESPONSE:
                //  ボードからの座標
                int[,] readXY = Param as int[,];

                string s = "";

                for (int i = 0; i < GameData.BoardSize; i++)
                    {
                        for (int j = 0; j < GameData.BoardSize; j++)
                        {
                            s += readXY[i,j];
                        }
                    }

                Debug.Log(s);

                if (xxx.Equals("1"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                        {0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                        {0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                        {0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if(xxx.Equals("2"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0 },
                        {0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
                        {0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 },
                        {0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("5"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("7"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("8"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("3"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("4"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("6"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("q"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("w"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("e"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("r"))
                {
                    readXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }
                else if (xxx.Equals("9"))
                {
                    
                }

                //  ボードの座標をArrayにセーブ
                for (int i = 0; i < GameData.BoardSize; i++)
                {
                    for (int j = 0; j < GameData.BoardSize; j++)
                    {
                        boardXY[i, j] = readXY[i,j];
                    }
                }
                boardResponsed = true;
                break;
            case EVENT_TYPE.SERVER_RESPONSE:
                if (((string)Param).Equals("GetCoordinate"))
                {
                    //  サーバーからのパズルの座標をセーブ
                    string tmpCoordinate = ((WWW)Param2).text.Trim();

                    for (int i = 0; i < GameData.BoardSize; i++)
                    {
                        for (int j = 0; j < GameData.BoardSize; j++)
                        {
                            blockXY[i,j] = (int)Char.GetNumericValue(tmpCoordinate[i*15 + j]);
                        }
                    }

                    CoordinateArrived = true;
                }
                else if (((string)Param).Equals("SaveLocationDefault") ||
                        ((string)Param).Equals("SaveLocationColor"))
                {
                    //  セーブの状態をチェック
                    if (((WWW)Param2).text.Trim().Equals("true"))
                    {
                        //  Save Complete
                        isSaveComplete = true;
                    }
                    else
                    {
                        //  SAVE_FAILED　イベントをListenerに転送
                        EventManager.Instance.PostNotification(EVENT_TYPE.SAVE_FAILED, this);
                    }
                }
                break;
            case EVENT_TYPE.SPECIAL_COORDINATE_RESPONSE:
                //  Create / Tutorial Modeのパズルの座標をセーブ
                if (((string)Param).Equals("Create"))
                {
                    blockXY = new int[,]
                    {
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    };
                }
                else if(((string)Param).Equals("Tutorial"))
                {
                    blockXY = new int[,]
                    {
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    };
                }

                CoordinateArrived = true;
                break;
        }
    }
    private string xxx = "0";
}
