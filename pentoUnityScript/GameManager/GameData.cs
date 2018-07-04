using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------
/// <summary>
/// ゲームの重要なデータを管理
/// </summary>
public class GameData : MonoBehaviour {

    //  シリアル番号
    public static string SerialNum = string.Empty;

    //  会員番号
    public static string userNum = string.Empty;

    //  会員のレベル番号
    public static int LevelNum = 0;

    //  動画の番号
    public static int StoryNum = 0;

    //  コレクションの番号
    public static int CollectionNum = 0;

    //  パズルの番号
    public static int PuzzleNum = 0;

    //  動画の情報Array
    public static string[,] storyInfo;

    //  コレクションの情報Array
    public static string[,] CollectionInfo;

    //  パズルの情報Array
    //public static string[,] PuzzleInfo;

    //  パズルの情報Array
    public static string[,] LevelInfo;

    //  パズルの大きさ
    public static int BoardSize = 15;

    //  Sound Name
    public static string soundName = string.Empty;

    //  動画の名前Array
    public static string[] storyNames = {"HanselAndGretel", "Cindella"};

    //  基本 url
    public static string baseURL = "http://ec2-13-125-219-201.ap-northeast-2.compute.amazonaws.com";

    //  Unity url
    public static string UnityURL = "http://ec2-13-125-219-201.ap-northeast-2.compute.amazonaws.com/Load_Unity/";

    //  サーバーの例外処理の時、臨時にセーブしておく変数
    public static Dictionary<string, string> tmpPost = null;
    public static string tmpBaseURL = string.Empty;
    public static string tmpURL = string.Empty;

    //-----------------------------------------------------
    private static GameData instance = null;
    //-----------------------------------------------------
    //  Use this for initialization
    private void Awake()
    {
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
}
