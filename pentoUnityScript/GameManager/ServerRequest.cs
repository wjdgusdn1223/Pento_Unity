using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//------------------------------------------------
/// <summary>
/// サーバーに情報や機能の実行を要請
/// </summary>
public class ServerRequest : MonoBehaviour
{

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static ServerRequest Instance
    {
        get { return instance; }
        set { }
    }

    //------------------------------------------------
    private static ServerRequest instance = null;
    //------------------------------------------------
    // Use this for initialization
    private void Awake()
    {
        //  instanceが存在しない場合、このinstanceを唯一のobjectにする。
        if (instance == null)
        {
            instance = this;
            //  画面が変えても消滅しない
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //  存在する場合,消滅させる。
            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// POSTの方式を使ってサーバーに情報を転送
    /// </summary>
    /// <param name="baseURL">基本URL</param>
    /// <param name="url">ROUTE URL</param>
    /// <param name="post">POSTデータ</param>
    /// <returns></returns>
    public WWW POST(string baseURL, string url, Dictionary<string, string> post)
    {
        //  サーバーの例外処理の時、臨時にセーブ
        GameData.tmpBaseURL = baseURL;
        GameData.tmpURL = url;
        GameData.tmpPost = post;

        //  www form
        WWWForm form = new WWWForm();

        //  HeaderにTokenを追加
        form.headers.Add("csrf-token", "{{ csrf_token() }}");

        //  formにデータを追加
        foreach (KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
            Debug.Log("Postで転送するデータ - " + post_arg.Key + " : " + post_arg.Value);
        }

        Debug.Log("サーバーのURL : " + (baseURL + url));
        WWW www = new WWW(baseURL + url, form);

        StartCoroutine(WaitForRequest(baseURL, www, url));
        return www;
    }

    /// <summary>
    /// Responseを待つ
    /// </summary>
    /// <param name="baseURL">基本URL</param>
    /// <param name="www">www form</param>
    /// <param name="url">ROUTE URL</param>
    /// <returns></returns>
    private IEnumerator WaitForRequest(string baseURL, WWW www, string url)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            if (baseURL.Equals(GameData.baseURL))
            {
                // IMAGE_ARRIVED　イベントを転送
                EventManager.Instance.PostNotification(EVENT_TYPE.IMAGE_ARRIVED, this, url, www);
            }
            else
            {
                // SERVER_RESPONSE　イベントを転送
                EventManager.Instance.PostNotification(EVENT_TYPE.SERVER_RESPONSE, this, url, www);
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);

            if (www.error.Equals("Cannot resolve destination host"))
            {
                // Server Connecting Error　イベントを転送
                GameController.Instance.ISC = -1;;
            }
            else
            {
                // Server Error　イベントを転送
                EventManager.Instance.PostNotification(EVENT_TYPE.SERVER_ERROR, this, url, www);
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

        }
    }
}
