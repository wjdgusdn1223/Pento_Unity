using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Progress Controller
/// </summary>
public class ProgressController : MonoBehaviour, IListener {

    //  Pento GameObjectのParent Object
    public Transform PentoObject;

    //  Progress ObjectのParent Object
    public GameObject progress;

    //  Progress GameObject
    public GameObject progressDonut, progressStatus;

    //  Progressのサイズ
    private float size;

    //  パズルの座標
    private int[,] blockXY;

    //  Block Count
    private int blockNum = 0;

    //  Progress Transform
    Transform progressTrsfm = null;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static ProgressController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static ProgressController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("ProgressControllerを実行");
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

        //  BLOCK_CHANGED　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.BLOCK_CHANGED, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// Progressを実行
    /// </summary>
    /// <param name="puzzleSize">パズルのサイズ</param>
    /// <param name="blockXY">パズルの座標</param>
    /// <param name="blockSize">Blockのサイズ</param>
    /// <param name="puzzleNum">パズルの番号</param>
    public void StartProgress(int[] puzzleSize, int[,] blockXY, float blockSize, int puzzleNum)
    {
        //  パズルのっ座標
        this.blockXY = blockXY;

        //  Block Countをリセット
        blockNum = 0;

        //  Block Countを計算
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (blockXY[i, j] == 1)
                {
                    blockNum += 1;
                }
            }
        }

        //   Progress prefab object Create 
        GameObject tmpObject = Instantiate(progress) as GameObject;

        //  Objectの名前を設定
        tmpObject.name = "Progress";

        //  Parent Objectを設定
        tmpObject.transform.SetParent(PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform);
        
        //  座標とサイズを設定
        int tmpLength = puzzleSize[0] > puzzleSize[1] ? puzzleSize[0] : puzzleSize[1];
        tmpObject.transform.localPosition =
            new Vector3(tmpLength * (1f + (0.025f * (15 - tmpLength))), tmpLength * (0.35f + (0.007f * (15 - tmpLength))), 0f);
        float tmpSize = 0.3f + ((tmpLength - 3) * 0.05f);
        tmpObject.transform.localScale = new Vector3(tmpSize, tmpSize, tmpSize);

        //  角度をリセット
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //  ----------- Progress Status Object Create
        //  Progress Status Prefab Object Create
        tmpObject = Instantiate(progressStatus) as GameObject;

        //  Progress Object Transform
        progressTrsfm =  PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Progress").transform;
        
        //  Parent Objectを設定
        tmpObject.transform.SetParent(progressTrsfm);

        //  Objectの名前を設定
        tmpObject.name = "Progress Status";

        //  座標を設定
        tmpObject.transform.localPosition = new Vector3(-2f, -2f, 1f);

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        //  角度をリセット
        tmpObject.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f)); 

        //  ProgressDonut Prefab Object Create
        tmpObject = Instantiate(progressDonut) as GameObject;

        //  Parent Objectを設定
        tmpObject.transform.SetParent(progressTrsfm);

        //  座標を設定
        tmpObject.transform.localPosition = new Vector3(0f, 0f, 0f);

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(0.2f, 0.05f, 0.2f);

        //  角度をリセット
        tmpObject.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        //  -----------
    }

    //---------------------------------------------
    /// <summary>
    /// ProgressStatusを変更
    /// </summary>
    /// <param name="boardXY">ボードの座標</param>
    private void ProgressChange(int[,] boardXY)
    {
        //  Correct Block Count
        int correctBlockNum = 0;

        //  Correct Block Countを計算
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (blockXY[i, j] == 1 && boardXY[i, j] == 1)
                {
                    correctBlockNum += 1;
                }
            }
        }
        Debug.Log(blockNum + " : " + correctBlockNum);
        float size = 0.5f + ((3.1f / blockNum) * correctBlockNum);

        //  ProgressStatusのサイズを変更
        StartCoroutine(ProgressScaleChange(size));
    }

    //--------------------------------------------------
    /// <summary>
    /// Stop Progress Courutine
    /// </summary>
    public void StopProgress()
    {
        //  Stop All Courutine
        StopAllCoroutines();
    }

    //--------------------------------------------------
    /// <summary>
    /// Progress Scale Change Animation
    /// </summary>
    /// <param name="size">目標のサイズ</param>
    IEnumerator ProgressScaleChange(float size)
    {
        float progress = 0;

        Transform statusSize = progressTrsfm.Find("Progress Status").GetComponent<Transform>();

        //  ProgressStatusのサイズを変える
        while (progress <= 4)
        {
            statusSize.localScale = 
                Vector3.Lerp(statusSize.localScale, new Vector3(size, 0.5f, size), progress);
            progress += Time.deltaTime * 0.5f;
            yield return new WaitForSeconds(0.01f);
        }

        //  確実にサイズを設定
        statusSize.localScale = new Vector3(size, 0.5f, size);
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
                    //  Stop Progress Courutine
                    StopProgress();

                    //  Progress Complete
                    ProgressChange(blockXY);
                }
                break;
            case EVENT_TYPE.BLOCK_CHANGED:
                //  Stop Progress Courutine
                StopProgress();

                //  Progress Change (only PuzzleGame Playing)
                ProgressChange(Param as int[,]);
                break;
        }
    }
}
