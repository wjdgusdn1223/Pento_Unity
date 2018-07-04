using UnityEngine;
using System.Collections;

/// <summary>
/// Story　Controller
/// </summary>
public class StoryController : MonoBehaviour
{
    //  Storyの名前
    private string storyName;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryControllerを実行");
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
    /// Story Scenario Start
    /// </summary>
    /// <param name="storyName">Storyの名前</param>
    public void StartStory(string storyName)
    {
        this.storyName = storyName;

        //  Story Scenario Start
        switch (storyName)
        {
            case "Tutorial":
                TutorialScenario.Instance.StartScenario();
                break;
            case "HanselAndGretel":
                HanselAndGretelScenario.Instance.StartScenario();
                break;
            case "Cindella":
                CindellaScenario.Instance.StartScenario();
                break;
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
