using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------------
/// <summary>
/// Listenerたちにイベントを転送するためのsingletonイベントマネージャー
/// IListener と一緒に動作する
/// </summary>
public class EventManager : MonoBehaviour
{
    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static EventManager Instance
    {
        get
        {
            return instance;
        }
        set { }
    }
    //---------------------------------------------------------
    private static EventManager instance = null;
    //---------------------------------------------------------
    /// <summary>
    /// Listener Object Array(すべてのObjectがイベントの受信のために登録されている。)
    /// </summary>
    private Dictionary<EVENT_TYPE, List<IListener>> Listeners =
        new Dictionary<EVENT_TYPE, List<IListener>>();

    //---------------------------------------------------------
    // Use this for initialization
    void Awake()
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

    //---------------------------------------------------------
    /// <summary>
    /// Listener ArrayにListener Objectを追加
    /// </summary>
    /// <param name="Event_Type">イベントのタイプ</param>
    /// <param name="Listener">イベントを受信するObject</param>
    public void AddListener(EVENT_TYPE Event_Type, IListener Listener)
    {
        //  このイベントを受信するListenerのList
        List<IListener> ListenList = null;

        //  イベントが存在するかをチェック。存在する場合、これをListに追加
        if (Listeners.TryGetValue(Event_Type, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }

        //  新しいListを作る
        ListenList = new List<IListener>();
        ListenList.Add(Listener);
        //  内部のListener Listに追加する
        Listeners.Add(Event_Type, ListenList);
    }

    //---------------------------------------------------------
    /// <summary>
    /// イベントをListenerに伝達
    /// </summary>
    /// <param name="Event_Type">イベントのタイプ</param>
    /// <param name="Sender">イベントを呼ぶObject</param>
    /// <param name="Param">選択ができるParameter</param>
    public void PostNotification(
        EVENT_TYPE Event_Type, Component Sender, object Param = null, object Param2 = null)
    {
        //  すべてのListenerにイベントを知らせる
        //  このイベントを受信するListenerたちのList
        List<IListener> ListenList = null;

        //  イベントがないならreturn
        if (!Listeners.TryGetValue(Event_Type, out ListenList))
            return;

        //  イベントを適合したListenerに知らせる
        for (int i = 0; i < ListenList.Count; i++)
        {
            //  Objectがnullではない場合
            if (!ListenList[i].Equals(null))
            {
                ListenList[i].OnEvent(Event_Type, Sender, Param, Param2);
            }
        }
    }

    //---------------------------------------------------------
    /// <summary>
    /// イベントの種類とlistenerをDictionaryから消滅させる
    /// </summary>
    /// <param name="Event_Type">消滅させるイベント</param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        //  Dictionaryの項目を消滅させる
        Listeners.Remove(Event_Type);
    }

    //---------------------------------------------------------
    /// <summary>
    /// Dictionaryから使い道がない項目を消滅させる
    /// </summary>
    public void RemoveRedundancies()
    {
        //  新しいDictionaryを作る
        Dictionary<EVENT_TYPE, List<IListener>> TmpListeners =
            new Dictionary<EVENT_TYPE, List<IListener>>();

        // Dictionaryのすべての項目をチェック
        foreach (KeyValuePair<EVENT_TYPE, List<IListener>> Item in Listeners)
        {
            //  null Objectを削除する
            for (int i = Item.Value.Count-1; i >= 0; i--)
            {
                if (Item.Value[i].Equals(null))
                {
                    Item.Value.RemoveAt(i);
                }
            }

            //  Listに残った項目を臨時のDictionaryにセーブする
            if (Item.Value.Count > 0)
            {
                TmpListeners.Add(Item.Key, Item.Value);
            }
        }

        //  Dictionaryを交代する
        Listeners = TmpListeners;
    }

    //---------------------------------------------------------
    /// <summary>
    /// Sceneが変更する時実行される。RemoveRedundanciesを実行。
    /// </summary>
    private void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}
