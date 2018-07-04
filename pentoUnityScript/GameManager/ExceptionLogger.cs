using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------
/// <summary>
/// Log Fileをセーブ
/// </summary>
public class ExceptionLogger : MonoBehaviour {

    //  StreanWriter Object
    private System.IO.StreamWriter SW;

    //  Log File Name
    private string LogFileName = string.Format("log_{0:yyyyMMdd_Hmmss}.txt",System.DateTime.Now);

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static ExceptionLogger Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static ExceptionLogger instance = null;
    //-------------------------------------------------------------
    // Use this for initialization
    void Awake () {
        //  sceneにinstanceが存在するかを検査
        //  存在する場合,消滅させる。
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        //  このinstanceを唯一のobjectにする。
        instance = this;

        //  stringの記録Objectを作る
        SW = new System.IO.StreamWriter(Application.persistentDataPath
                                        + "/" + LogFileName);

        #if SHOW_DEBUG_MESSAGES
                Debug.Log(Application.persistentDataPath + "/" + LogFileName);
#endif

        //  画面が変えても消滅しない
        DontDestroyOnLoad(gameObject);
    }

    //-------------------------------------------------------------
    /// <summary>
    /// 例外を受けて記録できるように登録する
    /// </summary>
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    //-------------------------------------------------------------
    /// <summary>
    /// 例外の受信の登録を解除する
    /// </summary>
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    //-------------------------------------------------------------
    /// <summary>
    /// 例外をtxt Fileに記録する
    /// </summary>
    /// <param name="logString">Logの内容</param>
    /// <param name="stackTrace">stackTraceの内容</param>
    /// <param name="type">Logのタイプ</param>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        //  Errorや警告の場合、記録する
        if(type == LogType.Exception || type == LogType.Error)
        {
            SW.WriteLine(
                string.Format("Logged at : {0} - Log Desc : {1}" + 
                              " - Trace : {2} - Type : {3}",
                              System.DateTime.Now.ToString(),
                              logString, stackTrace, type)
            );
        }
    }

    //-------------------------------------------------------------
    /// <summary>
    /// Objectが破壊される時、実行される
    /// </summary>
    private void OnDestroy()
    {
        SW.Close();
    }
}
