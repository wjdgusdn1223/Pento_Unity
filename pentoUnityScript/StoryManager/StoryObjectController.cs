using UnityEngine;
using System.Collections;

/// <summary>
/// Story Object Controller
/// </summary>
public class StoryObjectController : MonoBehaviour
{

    //  Map ObjectのParent Object
    public Transform Map;

    //  Character ObjectのParent Object
    public Transform Characters;

    //  CheckPoint ObjectのParent Object
    public Transform CPParent;

    //  CheckPoint Object
    public GameObject checkPoint;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryObjectController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryObjectController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryObjectControllerを実行");
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

    //---------------------------------------------------
    /// <summary>
    /// Mapを出力
    /// </summary>
    /// <param name="map">Map Prefab</param>
    /// <param name="point">Map Objectの座標</param>
    /// <param name="scale">Map Objectのサイズ</param>
    public void PrintMap(GameObject map, Vector3 point, Vector3 scale)
    {
        //  Map Prefab Object Create
        GameObject tmpObject = Instantiate(map) as GameObject;

        //  Parent Objectを設定
        tmpObject.transform.SetParent(Map);

        //  座標を設定
        tmpObject.transform.localPosition = point;

        //  サイズを設定
        tmpObject.transform.localScale = scale;
    }

    //---------------------------------------------------
    /// <summary>
    /// キャラクターを出力
    /// </summary>
    /// <param name="character">キャラクターObject</param>
    /// <param name="characterName">キャラクターObjectの名前</param>
    /// <param name="point">キャラクターObjectの座標</param>
    public void PrintCharacter(GameObject character, string characterName, Vector3 point)
    {
        //  Character Prefab Object Create 
        GameObject tmpObject = Instantiate(character) as GameObject;

        //  Parent Objectを設定
        tmpObject.transform.SetParent(Characters);

        //  座標を設定
        tmpObject.transform.localPosition = point;

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);

        //  名前を設定
        tmpObject.name = characterName;
    }

    //---------------------------------------------------
    /// <summary>
    /// CheckPointを出力
    /// </summary>
    /// <param name="position">目標の座標</param>
    public void PrintCheckPoint(Vector3 position)
    {
        //  CheckPoint Prefab Object Create 
        GameObject tmpObject = Instantiate(checkPoint) as GameObject;

        tmpObject.name = "CheckPoint";

        //  Parent Objectを設定
        tmpObject.transform.SetParent(CPParent);

        //  座標を設定
        tmpObject.transform.position = position;

        //  サイズを設定
        tmpObject.transform.localScale = new Vector3(1f, 1f, 1f);
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
