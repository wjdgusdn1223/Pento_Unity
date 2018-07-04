using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// block objectを制御
/// </summary>
public class BlockController : MonoBehaviour, IListener
{
    //  PuzzleGame ObjectのParent Object
    public Transform PentoObject;

    //  blankBlock prefab object
    public GameObject blankBlock;

    //  PentoBlock prefab object
    public GameObject pentoBlock;

    //  Puzzle ObjectのParent Object
    public GameObject PuzzleObject;

    //  Puzzleの基準座標
    float basedX;
    float basedY;
    float basedZ;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static BlockController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static BlockController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("BlockControllerを実行");
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
        //  PUZZLE_COMPLETE イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.PUZZLE_COMPLETE, this);
    }

    //--------------------------------------------------
    /// <summary>
    /// Puzzle Parent Objectを作る
    /// </summary>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <param name="puzzleCoordinate">パズルの座標</param>
    /// <param name="puzzleRotateValue">パズルのrotation</param>
    public void PuzzlePrefabCreate(int puzzleNum, Vector3 puzzleCoordinate, Vector3 puzzleRotateValue, float blockSize)
    {
        //  Parent Objectの座標を０，０，０に設定
        PentoObject.transform.localPosition = Vector3.zero;

        //  Create puzzle object 
        GameObject tmpObject = Instantiate(PuzzleObject) as GameObject;

        //  Parent Objectを設定
        tmpObject.transform.SetParent(PentoObject);

        //  Objectの座標を設定
        tmpObject.transform.localPosition = new Vector3(puzzleCoordinate.x, puzzleCoordinate.y, puzzleCoordinate.z);

        //  Objectの大きさを設定
        tmpObject.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

        //  Objectの名前を設定
        tmpObject.name = "Puzzle Object_" + puzzleNum;

        //  Objectのrotationを設定
        tmpObject.transform.rotation = Quaternion.Euler(puzzleRotateValue);
    }

    //-------------------------------------------------
    /// <summary>
    /// blankBlockを全部削除
    /// </summary>
    public void DestroyBlankCube()
    {
        //  画面にあるblankBlockを全部削除する
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("BlankBlock");

        foreach (GameObject item in tmp)
        {
            DestroyImmediate(item);
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// blankBlockを座標どおり配置
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    /// <param name="blockXY">パズルの座標</param>
    /// <param name="boardXY">ボードの座標</param>
    /// <param name="blockSize">blockの大きさ</param>
    /// <param name="puzzleNum">パズルの番号</param>
    public void PrintBlankCube(string type, int[,] blockXY, int[,] boardXY, int puzzleNum)
    {
        //  画面にあるblankBlockを全部削除する
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("BlankBlock");

        foreach (GameObject item in tmp)
        {
            DestroyImmediate(item);
        }

        //  基準座標を設定
        basedX = GameData.BoardSize / 2f * -1f + 0.5f;
        basedY = GameData.BoardSize / 2f - 1f;
        basedZ = 0;


        //  blankBlock　prefabをパズルの座標に出力
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (blockXY[i, j] == 1 && boardXY[i, j] == 0)
                {
                    //  Create blankBlock object 
                    GameObject tmpObject = Instantiate(blankBlock) as GameObject;

                    //  Parent Objectを設定
                    tmpObject.transform.SetParent
                        (PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.Find("BlankPuzzle").transform);

                    //  Objectの大きさを設定
                    tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);

                    //  Objectのrotationを設定
                    tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    //  Objectの座標を基準座標に合わせて設定
                    tmpObject.transform.localPosition = new Vector3(basedX + j, basedY - i, basedZ);
                }
            }
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// ボードの座標どおりPentoBlockを配置
    /// </summary>
    /// <param name="type">ゲームのタイプ</param>
    /// <param name="boardXY">ボードの座標</param>
    /// <param name="blockSize">blockの大きさ</param>
    /// <param name="puzzleNum">パズルの番号</param>
    public void PrintPuzzleCube(string type, int[,] boardXY, int puzzleNum)
    {
        //  画面にあるpentoBlockを全部削除する
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("PentoBlock");

        foreach (GameObject item in tmp)
        {
            DestroyImmediate(item);
        }

        //  ボードの座標どおりPentoBlockを配置
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (boardXY[i, j] == 1)
                {
                    //  Create pentoBlock object pentoBlock 
                    GameObject tmpObject = Instantiate(pentoBlock) as GameObject;

                    tmpObject.name = "pentoBlock_" + i + "_" + j + "_" + puzzleNum;

                    //  Parent Objectを設定
                    tmpObject.transform.SetParent
                        (PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.Find("PentoPuzzle").transform);

                    //  Objectの大きさを設定
                    tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);

                    //  Objectのrotationを設定
                    tmpObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    //  Objectの座標を基準座標に合わせて設定
                    tmpObject.transform.localPosition = new Vector3(basedX + j, basedY - i, basedZ);
                }
            }
        }

        //  タイプによる行動
        switch (type)
        {
            case "Story":
            case "Puzzle":
                break;
            case "Create":
                break;
            case "Tutorial":
                break;
        }
    }

    //----------------------------------------------
    /// <summary>
    /// 作ったパズルの座標を求める
    /// </summary>
    /// <param name="isColor">色の座標・パズルの座標</param>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <return>座標(String)</return>
    public string GetCreateCoordinate(bool isColor, int puzzleNum)
    {
        //  画面にあるpentoBlockを探す
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("PentoBlock");

        //  座標を臨時にセーブする変数
        string[] tmpValue = null;

        //  座標Array
        int[,] createCoordinate = 
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

        //  色の番号
        int colorValue = 1;

        //  objectの名前を使って座標を設定
        foreach (GameObject item in tmp)
        {
            tmpValue = item.name.Split('_');

            if(!tmpValue[3].Equals(puzzleNum.ToString()))
                continue;

            //  色の座標を求める場合、blockの色を数字に変えて座標Arrayにセーブ
            if (isColor)
            {
                switch (item.transform.GetChild(0).GetComponent<MeshRenderer>().material.name.Split(' ')[0])
                {
                    case "whiteCube":
                        colorValue = 1;
                        break;
                    case "blackCube":
                        colorValue = 2;
                        break;
                    case "redCube":
                        colorValue = 3;
                        break;
                    case "greenCube":
                        colorValue = 4;
                        break;
                    case "blueCube":
                        colorValue = 5;
                        break;
                    case "skyBlueCube":
                        colorValue = 6;
                        break;
                    case "purpleCube":
                        colorValue = 7;
                        break;
                    case "orangeCube":
                        colorValue = 8;
                        break;
                    case "brownCube":
                        colorValue = 9;
                        break;
                    default:
                        colorValue = 1;
                        break;
                }

                createCoordinate[int.Parse(tmpValue[1]), int.Parse(tmpValue[2])] = colorValue;
            }
            else
            {
                createCoordinate[int.Parse(tmpValue[1]), int.Parse(tmpValue[2])] = colorValue;
            }
        }

        //  パズルの大きさを測定
        int xs = -1;
        int xe = -1;
        int ys = -1;
        int ye = -1;

        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if (createCoordinate[i, j] != 0)
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

        int xTmp = (0 - xs) + (GameData.BoardSize - 1 - xe);

        if (xTmp !=0)
        {
            if (xTmp % 2 == 0)
            {
                xTmp /= 2;
            }
            else
            {
                xTmp = (int)Mathf.Floor(xTmp / 2f);
            }
        }

        int yTmp = (0 - ys) + (GameData.BoardSize - 1 - ye);

        if (yTmp != 0)
        {
            if (yTmp % 2 == 0)
            {
                yTmp /= 2;
            }
            else
            {
                yTmp = (int)Mathf.Floor(yTmp / 2f);
            }
        }

        int[,] tmpCoordinate =
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

        //  パズルを座標の中心に移動させる
        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                if ((i + yTmp) >= 0 && (i + yTmp) < GameData.BoardSize &&
                    (j + xTmp) >= 0 && (j + xTmp) < GameData.BoardSize)
                {
                    tmpCoordinate[i + yTmp, j + xTmp] = createCoordinate[i, j];
                }   
            }
        }

        createCoordinate = tmpCoordinate;

        string coordinateToString = "";

        for (int i = 0; i < GameData.BoardSize; i++)
        {
            for (int j = 0; j < GameData.BoardSize; j++)
            {
                coordinateToString += createCoordinate[i, j].ToString();
            }
        }

        return coordinateToString;
    }

    //-------------------------------------------------
    /// <summary>
    /// Effectを出力
    /// </summary>
    /// <param name="effectNum">Effectの番号</param>
    /// <param name="target">大きさを変更するターゲット</param>
    /// <param name="scale">Effectの大きさ</param>
    /// <param name="i">y 座標</param>
    /// <param name="j">x 座標</param>
    /// <param name="puzzleNum">パズルの番号</param>
    /// <param name="blockSize">Blockの大きさ</param>
    public void InputEffect(int effectNum, string target, float scale, int i, int j, int puzzleNum, float blockSize)
    {
        //  画面にあるpentoBlockを探す
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("PentoBlock");

        //  座標を臨時にセーブする変数
        string[] tmpValue = null;

        //  座標を計算してEffectを実行
        foreach (GameObject item in tmp)
        {
            tmpValue = item.name.Split('_');

            if(tmpValue[3].Equals(puzzleNum.ToString()) &&
                item.name.Equals("pentoBlock_" + i + "_" + j + "_" + puzzleNum))
            {
                Vector3 targetVector = new Vector3(0f,blockSize/2, blockSize * -1f);
                
                Quaternion targetRotation = 
                    Quaternion.Euler(item.transform.eulerAngles.x, item.transform.eulerAngles.y, item.transform.eulerAngles.z);

                Vector3 distance = targetRotation * targetVector;

                Vector3 effectPosition = item.transform.position + distance;

                EffectController.Instance.SpawnParticle(effectNum, target, scale, effectPosition);
            }
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
            case EVENT_TYPE.PUZZLE_COMPLETE:
                //  parameterを分離して別の変数にセーブ
                string[] tmp = Param2.ToString().Trim().Split('_');

                Debug.Log(Param2.ToString() + " : " + tmp[0] + ", " + tmp[1] + ", " + tmp[2]);

                int puzzleNum = Int32.Parse(tmp[0]);
                int puzzleSize = Int32.Parse(tmp[1]);
                float blockSize = float.Parse(tmp[2]);

                //  Effectの大きさを設定
                float scale = 3f + ((2f / 15) * puzzleSize);

                //  puzzle completeのEffectを実行
                StartCoroutine(EffectController.Instance.CompleteEffect
                    (6, "", scale * blockSize, PentoObject.transform.Find("Puzzle Object_" + puzzleNum).transform.position, puzzleSize, blockSize));
                break;
        }
    }
}
