using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイマーのController
/// </summary>
public class TimerController : MonoBehaviour,IListener {

    //  タイマーのサイズ
    private float size;

    //  Time
    private int time = 0;

    //  パズルゲームが終わったかどうか
    private int isPlaying = 0;

    //  Timer Prefab
    public GameObject timer;

    //  タイマーのParent Object
    public Transform PentoObject;

    //  Number Object Prefab
    public GameObject colon;
    public GameObject num0;
    public GameObject num1;
    public GameObject num2;
    public GameObject num3;
    public GameObject num4;
    public GameObject num5;
    public GameObject num6;
    public GameObject num7;
    public GameObject num8;
    public GameObject num9;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static TimerController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static TimerController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("TimerControllerを実行");
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
        //  PUZZLE_COMPLETE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.PUZZLE_COMPLETE, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// タイマーを実行
    /// </summary>
    /// <param name="puzzleSize">パズルのサイズ</param>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <param name="puzzleRotateValue">パズルの角度</param>
    public void StartTimer(int[] puzzleSize, int puzzleNum, Vector3 puzzleRotateValue)
    {
        //  Timer Prefab Object Create
        GameObject tmpObject = Instantiate(timer) as GameObject;

        //  名前を設定
        tmpObject.name = "Timer";

        //  Parent Objectを設定
        tmpObject.transform.SetParent
            (PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform);

        //  座標とサイズを設定
        int tmpLength = puzzleSize[0] > puzzleSize[1] ? puzzleSize[0] : puzzleSize[1];
        tmpObject.transform.localPosition = 
            new Vector3(tmpLength * (0.62f + (0.008f * (15 - tmpLength))) , tmpLength * (0.42f + (0.008f * (15 - tmpLength))), 0f);
        float tmpSize = 0.15f + ((tmpLength - 3) * 0.021f);
        tmpObject.transform.localScale = new Vector3(tmpSize, tmpSize, tmpSize);

        //  角度をリセット
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //  タイマーを実行
        StartCoroutine(Timer(puzzleNum));
    }

    //--------------------------------------------------
    /// <summary>
    /// タイマーのAnimation
    /// </summary>
    /// <param name="puzzleNum">パズルの番号</param>
    IEnumerator Timer(int puzzleNum)
    {
        isPlaying = 1;

        //  タイマーを実行
        while (isPlaying == 1)
        {
            //  画面上にあるタイマーを全部削除
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("Timer");

            foreach (GameObject item in tmp)
            {
                DestroyImmediate(item);
            }

            time += 1;

            //  時間を出力
            printTime("minute1", (int)Mathf.Floor(Mathf.Floor(time / 60) / 10), puzzleNum);
            printTime("minute2", (int)Mathf.Floor(time / 60) % 10, puzzleNum);
            printTime("second1", (int)Mathf.Floor(time % 60 / 10), puzzleNum);
            printTime("second2", time % 60 % 10, puzzleNum);

            //  ------ Colon Prefab Object Create
            GameObject tmpObject = Instantiate(colon) as GameObject;

            //  Parent Objectを設定
            tmpObject.transform.SetParent(PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Timer").transform);

            //  座標を設定
            tmpObject.transform.localPosition = new Vector3(6.5f, 0f, 0f);

            //  サイズを設定
            tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);

            //  角度をリセット
            tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //  -------

            //  delay time
            yield return new WaitForSeconds(1f);
        }
    }

    //---------------------------------------------
    /// <summary>
    /// Number Object Create
    /// </summary>
    /// <param name="type">番号のタイプ</param>
    /// <param name="num">出力する番号</param>
    /// <param name="puzzleNum">パズルの番号</param>
    private void printTime(string type, int num, int puzzleNum)
    {
        //　Number Objectを選択
        GameObject prefab = null;

        switch (num)
        {
            case 0: prefab = num0; break;
            case 1: prefab = num1; break;
            case 2: prefab = num2; break;
            case 3: prefab = num3; break;
            case 4: prefab = num4; break;
            case 5: prefab = num5; break;
            case 6: prefab = num6; break;
            case 7: prefab = num7; break;
            case 8: prefab = num8; break;
            case 9: prefab = num9; break;
            default: prefab = num0; break;
        }

        //  Number prefab Object Create
        GameObject tmpObject = Instantiate(prefab) as GameObject;

        //  Parent Objectを設定
        tmpObject.transform.SetParent(PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Timer").transform);

        //  座標を設定
        switch (type)
        {
            case "minute1":
                tmpObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                break;
            case "minute2":
                tmpObject.transform.localPosition = new Vector3(4, 0f, 0f);
                break;
            case "second1":
                tmpObject.transform.localPosition = new Vector3(9, 0f, 0f);
                break;
            case "second2":
                tmpObject.transform.localPosition = new Vector3(13, 0f, 0f);
                break;
        }

        //  サイズを1に設定
        tmpObject.transform.localScale = new Vector3(-1f, 1f, 1f);

        //  角度をリセット
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    //-------------------------------------------------
    /// <summary>
    /// 時間を求める
    /// </summary>
    /// <returns>記録時間</returns>
    public int getTime()
    {
        return time;
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
            case EVENT_TYPE.PUZZLE_COMPLETE:
                if (!Param.ToString().Equals("Create"))
                {
                    //  Puzzle Complete　イベントをListenerに転送
                    isPlaying = 0;
                }
                break;
        }
    }
}
