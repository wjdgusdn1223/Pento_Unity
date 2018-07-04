using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームカメラのController
/// </summary>
public class CameraController : MonoBehaviour, IListener
{
    //  パズルの大きさ
    private int horizontalSize;
    private int verticalSize;

    //  カメラを拡大する程度
    public float tmpOrthSize;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static CameraController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static CameraController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("CameraControllerを実行");
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

    }

    //-------------------------------------------------------------
    /// <summary>
    /// パズルを見れる座標にカメラを移動
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    /// <param name="blockXY">パズルの座標</param>
    /// <param name="blockSize">Blockの大きさ</param>
    /// <param name="puzzleCoordinate">パズルの座標</param>
    /// <param name="puzzleRotateValue">パズルのRotation</param>
    /// <returns>パズルの大きさ</returns>
    public int[] CameraSetter(string type, int[,] blockXY, float blockSize, Vector3 puzzleCoordinate, Vector3 puzzleRotateValue)
    {
        //  The near clipping plane distance
        Camera.main.nearClipPlane = 0.001f;

        //  パズルとカメラ間の距離
        Vector3 distance = Vector3.zero;

        //  カメラを拡大する程度（臨時セーブ）
        tmpOrthSize = 0f;

        //  ゲームのタイプによるアルゴリズム
        switch (type)
        {
            case "Tutorial":
            case "Story":
            case "Puzzle":
                //  パズルの大きさを求める
                PuzzleSize(type, blockXY, blockSize);

                //  パズルの大きさを使って拡大の程度を求める
                if (horizontalSize < verticalSize)
                {
                    tmpOrthSize = (Mathf.Ceil(verticalSize / 2f) + 2f) * blockSize;

                    //  パズルとの距離を設定
                    distance = new Vector3(0f, 0f, ((verticalSize * (1.5f - ((verticalSize - 3) * 0.033f))) * -1f) * blockSize);
                }
                else
                {
                    tmpOrthSize = (Mathf.Ceil(horizontalSize / 2f) + 1f) * blockSize;

                    //  パズルとの距離を設定
                    distance = new Vector3(0f, 0f, ((horizontalSize * (1.5f - ((horizontalSize - 3) * 0.033f))) * -1f) * blockSize);
                }
                break;
            case "Create":
                tmpOrthSize = 9f * blockSize;

                //  パズルとの距離を設定
                distance = new Vector3(0f, 0f, -16.5f * blockSize);
                break;
        }

        //  パズルの角度を求める
        Quaternion CharacterRotateValue = Quaternion.Euler(puzzleRotateValue.x, puzzleRotateValue.y, puzzleRotateValue.z);

        //  目標の角度
        Vector3 targetPoint = CharacterRotateValue * distance;

        //  Camera Focusing Animation
        StartCoroutine(CameraFocusAnimation(targetPoint, puzzleCoordinate, puzzleRotateValue, type));

        //  パズルの大きさ
        return new int[] {horizontalSize, verticalSize};
    }

    //-------------------------------------------------------------
    /// <summary>
    /// パズルの大きさを求める
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    /// <param name="blockXY">パズルの座標</param>
    /// <param name="blockSize">Blockの大きさ</param>
    private void PuzzleSize(string type, int[,] blockXY, float blockSize)
    {
        switch (type)
        {
            case "Tutorial":
            case "Story":
            case "Puzzle":
                //  パズルのサイズを計算する

                int xs = -1;
                int xe = -1;
                int ys = -1;
                int ye = -1;

                for (int i = 0; i < GameData.BoardSize; i++)
                {
                    for (int j = 0; j < GameData.BoardSize; j++)
                    {
                        if (blockXY[i, j] == 1)
                        {
                            if (xs == -1) { xs = j; }
                            else if (xs > j) { xs = j; }

                            if (ys == -1) { ys = i; }
                            else if (ys > i) { ys = i; }

                            if (xe == -1) { xe = j; }
                            else if (xe < j) { xe = j; }

                            if (ye == -1) { ye = i; }
                            else if (ye < i) { ye = i; }
                        }
                    }
                }
                horizontalSize = (xe - xs + 1) % 2 == 0 ? xe - xs + 2 : xe - xs + 1;
                verticalSize = (ye - ys + 1) % 2 == 0 ? ye - ys + 2 : ye - ys + 1;
#if SHOW_DEBUG_MESSAGES
                Debug.Log(horizontalSize + " : " + verticalSize);
#endif
                break;
            case "Create":
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Camera Focusing Animation
    /// </summary>
    /// <param name="targetPoint">目標の座標</param>
    /// <param name="puzzleCoordinate">パズルの座標</param>
    /// <param name="puzzleRotateValue">パズルの角度</param>
    /// <param name="type">ゲームのタイプ</param>
    /// <returns></returns>
    IEnumerator CameraFocusAnimation(Vector3 targetPoint ,Vector3 puzzleCoordinate, Vector3 puzzleRotateValue, string type)
    {
        //  ゲームのタイプによるアルゴリズム
        switch (type)
        {
            case "Story":
                //  カメラの角度がパズルを向けるようにする
                StoryCameraController.Instance.StartLookAt(puzzleCoordinate);
                yield return new WaitUntil(() => StoryCameraController.Instance.IL == true);
                StoryCameraController.Instance.IL = false;
                break;
            case "Puzzle":
                break;
            case "Create":
                break;
            case "Tutorial":
                break;
        }

        //  臨時の座標
        Vector3 tmp = Vector3.zero;

        //  目標の座標
        Vector3 CameraDest = targetPoint + puzzleCoordinate;

        //  delay time
        int time = 0;

        while (tmp.x != Camera.main.transform.position.x ||
               tmp.y != Camera.main.transform.position.y ||
               tmp.z != Camera.main.transform.position.z)
        {
            //  変更する前の座標
            tmp = Camera.main.transform.position;

            //  カメラを目標の座標に移動
            Camera.main.transform.position =
                Vector3.Lerp(Camera.main.transform.position,
                                CameraDest,
                                Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(puzzleCoordinate - Camera.main.transform.position);

            // Smoothly rotate towards the target point.
            Camera.main.transform.rotation = 
                Quaternion.Slerp(Camera.main.transform.rotation, 
                                Quaternion.Euler(targetRotation.eulerAngles.x, 
                                                targetRotation.eulerAngles.y, 
                                                puzzleRotateValue.z), 
                                3f * Time.deltaTime);

            yield return new WaitForSeconds(0.01f);

            //  Block認識開始
            if ((time += 1) == 250)
            {
                //  カメラのモードを変える
                CameraOrthoController.Instance.StartMatrixBlender(tmpOrthSize);

                // //  CAMERA_SETTING_COMPLETE イベントを転送
                EventManager.Instance.PostNotification(EVENT_TYPE.CAMERA_SETTING_COMPLETE, this);
            }
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Camera Stop Focusing
    /// </summary>
    public void StopFocusing()
    {
        StopAllCoroutines();
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
