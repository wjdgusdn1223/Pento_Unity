using UnityEngine;
using System.Collections;

/// <summary>
/// LeapMotion Controller
/// </summary>
public class LeapMotionController : MonoBehaviour, IListener
{
    //  LeapMotion prefab
    public GameObject leap;

    //  ColorBox prefab
    public GameObject colorBox;

    //  dimensionCube prefab
    public GameObject dimensionCube;

    //  Gesture 認識度
    private float gestureSpeed = 0.25f;

    //  パズルの番号
    private int puzzleNum;

    //  ゲームのタイプ
    private string type;

    //  Paintinモードかどうか
    public bool isPainting = true;

    //　dimensionCubeを回しているかどうか
    private bool isRotating = false;

    //  カメラのモードを変えているかどうか
    private bool isDimensionChanging = false;

    //  SelectingBoxを移動しているかどうか
    private bool isColorSelecting = false;

    //  パズルが回っているかどうか
    private bool isPuzzleRotating = false;

    //  colorBox's child object prefab
    public GameObject red;
    public GameObject orange;
    public GameObject brown;
    public GameObject green;
    public GameObject blue;
    public GameObject skyBlue;
    public GameObject purple;
    public GameObject white;
    public GameObject black;

    //  LeapMotionで回す方向を判断するObject
    private GameObject handGestureBase;

    //  パズルを回す方向
    public Transform right;
    public Transform left;
    public Transform up;
    public Transform down;

    //  選べられた色
    private string selectedColor = "redCube";

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static LeapMotionController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static LeapMotionController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("LeapMotionControllerを実行");
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
        //  COLOR_SELECT イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.COLOR_SELECT, this);
        //  PAINT_COLOR イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.PAINT_COLOR, this);
        //  DIMENSION_CHANGE イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.DIMENSION_CHANGE, this);
        //  SAVE_RECORD イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SAVE_RECORD, this);
        //  SAVE_PENTO イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.SAVE_PENTO, this);
    }

    //-------------------------------------------------
    /// <summary>
    /// LeapMotionモードを実行
    /// </summary>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <param name="puzzleSize">パズルのサイズ</param>
    /// <param name="type">game type</param>
    public void LeapMotionActivate(int puzzleNum, int[] puzzleSize, string type)
    {
        //  ゲームのタイプをセーブ
        this.type = type;

        //  パズルの番号をセーブ
        this.puzzleNum = puzzleNum;

        //  ---------------------------------------LeapMotion Create
        //  LeapMotion prefab Create
        GameObject tmpObject = Instantiate(leap) as GameObject;

        tmpObject.name = "Leap";

        //  Parent Objectを設定
        tmpObject.transform.SetParent
            (transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform);

        //  角度を設定
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //  基準のサイズを求める
        int puzzleLength = puzzleSize[0] > puzzleSize[1] ? puzzleSize[0] : puzzleSize[1];

        //  サイズを計算
        float size = 14f + ((23.5f / 12f) * (puzzleLength - 3));

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(size, size, size);

        //  座標を計算
        float position = (4f + ((11f / 12f) * (puzzleLength - 3))) * -1f;

        //  座標を設定
        tmpObject.transform.localPosition = new Vector3(0f, position, -2f );
        //-----------------------------------------

        //  ---------------------------------------ColorBox Create
        //  ColorBox prefab Create
        tmpObject = Instantiate(colorBox) as GameObject;

        tmpObject.name = "ColorBox";

        //  Parent Objectを設定
        tmpObject.transform.SetParent
            (transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform);

        //  角度を設定
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //  サイズを計算
        size = 0.5f + ((1f / 12f) * (puzzleLength - 3));

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(size, size, size);

        //  座標を計算
        position = (3.5f + ((8f / 12f) * (puzzleLength - 3))) * -1f;

        //  座標を設定
        tmpObject.transform.localPosition = new Vector3(position, 0f, 0f);
        //-----------------------------------------

        //  ---------------------------------------dimensionCube Create
        //  dimensionCube prefab Create
        tmpObject = Instantiate(dimensionCube) as GameObject;

        tmpObject.name = "DimensionCube";

        //  Parent Objectを設定
        tmpObject.transform.SetParent
            (transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform);

        //  角度を設定
        tmpObject.transform.localRotation = Quaternion.Euler(25f, 50f, 25f);

        //  サイズを計算
        size = 0.7f + ((0.8f / 12f) * (puzzleLength - 3));

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(size, size, size);

        //  座標を計算
        float positionX = (4.5f + ((9f / 12f) * (puzzleLength - 3)));
        float positionY = (1.5f + ((3.5f / 12f) * (puzzleLength - 3))) * -1f;

        //  座標を設定
        tmpObject.transform.localPosition = new Vector3(positionX, positionY, 0f);
        //-----------------------------------------

        //  Hand Gestureの基準座標を設定
        handGestureBase = transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Leap").transform.Find("HandModels").transform.Find("RigidRoundHand_R").transform.Find("palm").gameObject;
    }

    //-------------------------------------------------
    /// <summary>
    /// 色を塗る
    /// </summary>
    /// <param name="Cube">対象のCube</param>
    private void paintColor(GameObject Cube)
    {
        //  Tutorialの場合
        if (type.Equals("Tutorial"))
        {
            //  TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 4);
        }

        //  選択した色のCube
        GameObject paintObject = null;

        //  選択した色をObjectにセーブ
        switch (selectedColor)
        {
            case "redCube":
                paintObject = red;
                break;
            case "orangeCube":
                paintObject = orange;
                break;
            case "brownCube":
                paintObject = brown;
                break;
            case "greenCube":
                paintObject = green;
                break;
            case "blueCube":
                paintObject = blue;
                break;
            case "skyBlueCube":
                paintObject = skyBlue;
                break;
            case "purpleCube":
                paintObject = purple;
                break;
            case "whiteCube":
                paintObject = white;
                break;
            case "blackCube":
                paintObject = black;
                break;
            default:
                return;
        }

        //  paintObject prefab Create
        GameObject tmpObject = Instantiate(paintObject) as GameObject;

        string[] tmpCoordinate = Cube.name.Split('_');

        tmpObject.name = "PentoBlock_" + tmpCoordinate[1] + "_" + tmpCoordinate[2] + "_" + puzzleNum;

        //  Parent Objectを設定
        tmpObject.transform.SetParent
            (transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("PentoPuzzle").transform);

        //  選択したCubeの角度を設定
        tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //  選択したCubeのサイズを設定
        tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);

        //  選択したCubeの座標に設定
        tmpObject.transform.localPosition = 
            new Vector3(Cube.transform.localPosition.x, Cube.transform.localPosition.y, Cube.transform.localPosition.z);

        //  選択したCubeを削除
        Destroy(Cube);
    }

    //-------------------------------------------------
    /// <summary>
    /// Color Select Animation
    /// </summary>
    /// <param name="targetPoint">目標の座標</param>
    IEnumerator ColorSelectAnimation(float targetPoint)
    {
        isColorSelecting = true;

        //  SelectBox Object  
        GameObject selectBox = 
            transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("ColorBox").transform.Find("SelectBox").transform.Find("selectBox").gameObject;

        //  目標の座標
        Vector3 target = new Vector3(0f, targetPoint, 0f);

        //  目標の座標に移動
        while (Mathf.Round(selectBox.transform.localPosition.y * 10) != Mathf.Round(target.y * 10))
        {
            selectBox.transform.localPosition = Vector3.Lerp(selectBox.transform.localPosition, target, Time.deltaTime * 3f);

            yield return new WaitForSeconds(0.01f);
        }

        //  確実に移動完了
        selectBox.transform.localPosition = target;

        Debug.Log("selectBox setting complete");

        isColorSelecting = false;
    }

    //-------------------------------------------------
    /// <summary>
    /// LeapMotionのモードを変える
    /// </summary>
    private void DimensionChange()
    {
        if (type.Equals("Tutorial"))
        {
            //  TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 5);
        }

        if (!isPuzzleRotating && !isDimensionChanging)
        {
            isDimensionChanging = true;

            if (isRotating)
            {
                isRotating = false;
            }

            //  モードを変える　色モード・回転モード
            if (isPainting)
            {
                //  ColorBox 非活性化
                transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("ColorBox").gameObject.SetActive(false);

                selectedColor = "null";

                isPainting = false;

                //  カメラのモードを３Ｄにする
                CameraOrthoController.Instance.StartMatrixBlender(0f);

                StartCoroutine(RotatePuzzle());
            }
            else
            {
                //  colorBox　活性化
                transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("ColorBox").gameObject.SetActive(true);

                selectedColor = "redCube";

                //  selectBoxをredCubeの座標まで移動
                StartCoroutine(ColorSelectAnimation(4.1f));

                GameObject puzzle =
                    transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("PentoPuzzle").gameObject;

                //  パズルをもとの角度に戻す
                StartCoroutine(PuzzleRotateAnimation("reset", puzzle));

                isPainting = true;

                //  カメラのモードを２Ｄにする
                CameraOrthoController.Instance.StartMatrixBlender(CameraController.Instance.tmpOrthSize);
            }

            //  dimensionCubeをモードに合わせて回転するかどうか設定
            StartCoroutine(RotateDimensionCube(isPainting));
        }
    }

    //----------------------------------------------------
    /// <summary>
    /// Rotate　Dimension　Cube
    /// </summary>
    /// <param name="isPainting">色を塗っているかどうか</param>
    /// <returns></returns>
    IEnumerator RotateDimensionCube(bool isPainting)
    {
        isRotating = true;

        //  Dimension　Cube
        GameObject cube =
            transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("DimensionCube").gameObject;

        int time = 0;

        //  回転モードの場合dimensionCubeを回転させる
        while (isRotating)
        {
            if (!isPainting)
            {
                cube.transform.Rotate(new Vector3(0f, 3f, 0f));

                if (time == 100)
                {
                    isDimensionChanging = false;
                }
            }
            else
            {
                cube.transform.localRotation = Quaternion.Euler(25f, 50f, 25f);

                isRotating = false;

                yield return new WaitForSeconds(1.5f);

                isDimensionChanging = false;
            }

            yield return new WaitForSeconds(0.01f);

            time += 1;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// パズルを回す
    /// </summary>
    /// <returns></returns>
    IEnumerator RotatePuzzle()
    {
        isPuzzleRotating = false;

        GameObject puzzle = 
            transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("PentoPuzzle").gameObject;

        //  Gesture x　基準座標
        float tmpX = handGestureBase.transform.localPosition.x;
        //float tmpY = handGestureBase.transform.localPosition.y;

        //  手が動く程度
        float horizontal = 0f;
        //float vertical = 0f;

        //  手が動いた方向
        string direction = string.Empty;

        bool rotateCheck = false;

        //  手の動きをチェック
        while (!isPainting)
        {
            rotateCheck = false;

            //  手が動いた程度を計算
            horizontal = tmpX - handGestureBase.transform.localPosition.x;
            //vertical = tmpY - handGestureBase.transform.localPosition.y;

            //  手が動いた方向を求める
            if (horizontal > gestureSpeed)
            {
                direction = "left";
                Debug.Log("left");

                rotateCheck = true;
            }
            else if (horizontal < gestureSpeed  * -1f)
            {
                direction = "right";
                Debug.Log("right");

                rotateCheck = true;
            }
            //else if (vertical < gestureSpeed  * -1f)
            //{
            //    direction = "up";
            //    Debug.Log("up");
            //
            //    rotateCheck = true;
            //}
            //else if (vertical > gestureSpeed * 1f)
            //{
            //    direction = "down";
            //    Debug.Log("down");
            //
            //    rotateCheck = true;
            //}

            if (rotateCheck && !isPuzzleRotating)
            {
                if (type.Equals("Tutorial"))
                {
                    //  TUTORIAL_STEP_COMPLETE イベントをListenerに転送
                    EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 6);
                }

                //  パズルを求めた方向に回す
                StartCoroutine(PuzzleRotateAnimation(direction, puzzle));
            }

            //  現在の座標に再設定  
            tmpX = handGestureBase.transform.localPosition.x;
            //tmpY = handGestureBase.transform.localPosition.y;
            
            //  delay time
            yield return new WaitForSeconds(0.4f);
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// Puzzle Rotate Animation
    /// </summary>
    /// <param name="direction">方向</param>
    /// <param name="puzzle">パズルのObject</param>
    IEnumerator PuzzleRotateAnimation(string direction, GameObject puzzle)
    {
        isPuzzleRotating = true;

        //  元のパズルの角度をセーブ
        Quaternion from = Quaternion.Euler(puzzle.transform.localEulerAngles.x, puzzle.transform.localEulerAngles.y, puzzle.transform.localEulerAngles.z);

        //  目標の角度を設定
        Quaternion to = from;

        //  方向による角度を設定
        switch (direction)
        {
            case "right":
                to *= right.localRotation;
                break;
            case "left":
                to *= left.localRotation;
                break;
            case "up":
                to *= up.localRotation;
                break;
            case "down":
                to *= down.localRotation;
                break;
            case "reset":
                to = Quaternion.Euler(0f, 0f, 0f);
                break;
        }

        //  角度の臨時変数
        float tmpX = 0f;
        float tmpY = 0f;
        float tmpZ = 0f;
        float firstValue = 0f;

        if (direction.Equals("right") || direction.Equals("left"))
        {
            firstValue = puzzle.transform.localEulerAngles.y;
        }
        else if (direction.Equals("up") || direction.Equals("down"))
        {
            firstValue = puzzle.transform.localEulerAngles.x;
        }

        //  パズルを回す
        while (true)
        {
            //  回す前の角度をセーブ
            //  이거 x y 바껴있었다
            tmpX = puzzle.transform.localEulerAngles.y;
            tmpY = puzzle.transform.localEulerAngles.x;
            tmpZ = puzzle.transform.localEulerAngles.z;

            //  目標の角度まで回す
            puzzle.transform.localRotation = Quaternion.Slerp(puzzle.transform.localRotation, to, Time.deltaTime * 3f);

            //  目標の角度まで回したかどうかをチェック
            if (((Mathf.Floor(tmpX) % 90 == 0 || Mathf.Ceil(tmpX) % 90 == 0) && 
                    (int)firstValue != (int)tmpX && (direction.Equals("right") || direction.Equals("left"))) ||
                ((Mathf.Floor(tmpY) % 90 == 0 || Mathf.Ceil(tmpY) % 90 == 0) &&
                    (int)firstValue != (int)tmpY && (direction.Equals("up") || direction.Equals("down"))) ||
                (direction.Equals("reset") && 
                    (Mathf.Floor(tmpX) == 0 || Mathf.Ceil(tmpX) == 0) &&
                    (Mathf.Floor(tmpY) == 0 || Mathf.Ceil(tmpY) == 0) &&
                    (Mathf.Floor(tmpZ) == 0 || Mathf.Ceil(tmpZ) == 0)))
            {
                // 確実に角度を設定
                puzzle.transform.localRotation = to;

                break;
            }

            //  delay time
            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("puzzle rotate complete");

        isPuzzleRotating = false;

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
            case EVENT_TYPE.COLOR_SELECT:
                if (!isColorSelecting)
                {
                    //  選択した色をparameterに変更
                    selectedColor = Param as string;

                    //  SelectBoxの目標座標
                    float targetPoint = 0f;

                    //  目標座標を設定
                    switch (selectedColor)
                    {
                        case "redCube":
                            targetPoint = 4.1f;
                            break;
                        case "orangeCube":
                            targetPoint = 2.9f;
                            break;
                        case "brownCube":
                            targetPoint = 1.7f;
                            break;
                        case "greenCube":
                            targetPoint = 0.5f;
                            break;
                        case "blueCube":
                            targetPoint = -0.7f;
                            break;
                        case "skyBlueCube":
                            targetPoint = -1.9f;
                            break;
                        case "purpleCube":
                            targetPoint = -3.1f;
                            break;
                        case "whiteCube":
                            targetPoint = -4.3f;
                            break;
                        case "blackCube":
                            targetPoint = -5.5f;
                            break;
                        default:
                            return;
                    }

                    //  Color　Select　Animationを実行
                    StartCoroutine(ColorSelectAnimation(targetPoint));
                }               
                break;
            case EVENT_TYPE.PAINT_COLOR:
                //  Blockの色を塗る
                paintColor(Param as GameObject);
                break;
            case EVENT_TYPE.DIMENSION_CHANGE:
                if (type.Equals("Tutorial"))
                {
                    //  TUTORIAL_STEP_COMPLETE　イベントをListenerに転送
                    EventManager.Instance.PostNotification(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this, 5);
                }

                //  LeapMotionのモードを変える
                DimensionChange();
                break;
            case EVENT_TYPE.SAVE_RECORD:
                if (type.Equals("Puzzle") || type.Equals("Story"))
                {
                    StopAllCoroutines();

                    //  モードをリセット
                    isPainting = true;

                    //  モードをリセット
                    isRotating = false;

                    //  モードをリセット
                    isColorSelecting = false;

                    //  モードをリセット
                    isPuzzleRotating = false;

                    //  LeapMotionを消滅させる
                    Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Leap").gameObject);

                    //  ColorBoxを消滅させる
                    Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("ColorBox").gameObject);

                    //  DimensionCubeを消滅させる
                    Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("DimensionCube").gameObject);
                }
                break;
            case EVENT_TYPE.SAVE_PENTO:
                StopAllCoroutines();

                //  モードをリセット
                isPainting = true;

                //  モードをリセット
                isRotating = false;

                //  モードをリセット
                isColorSelecting = false;

                //  モードをリセット
                isPuzzleRotating = false;

                //  LeapMotionを消滅させる
                Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("Leap").gameObject);

                //  ColorBoxを消滅させる
                Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("ColorBox").gameObject);

                //  DimensionCubeを消滅させる
                Destroy(transform.Find("Pento Object").transform.Find("Puzzle Object_" + puzzleNum).transform.Find("DimensionCube").gameObject);
                break;
        }
    }
}
