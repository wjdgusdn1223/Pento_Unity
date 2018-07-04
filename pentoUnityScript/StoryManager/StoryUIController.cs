using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Story Mode UI Controller
/// </summary>
public class StoryUIController : MonoBehaviour, IListener
{
    //  Textを出力しているかどうか
    private bool isTextPrinting = false;

    //  Ambassador　ウィンドウが活性化されているか
    private bool isAmbassadorPrinting = false;

    //　UIの出力を完了したかどうか
    private bool UIstepComplete = false;

    //  AmbassadorのStep
    private int AmbassadorStep = 0;

    //  Story UIのParent Object
    public Transform StoryCanvas;

    //  Story Title
    public GameObject StoryTitle;

    //  Story Narration
    public GameObject StoryNarration;

    //  Story Title Panel
    public GameObject StoryTitlePanel;

    //  Story Narration Panel
    public GameObject StoryNarrationPanel;

    //  Character Ambassador
    public GameObject CharacterAmbassador;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryUIController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryUIController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryUIControllerを実行");
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
        //  UI_STEP_COMPLETE　イベントを受信するためにListenerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.UI_STEP_COMPLETE, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// Story Title・Narrationを画面に出力するAnimationを実行
    /// </summary>
    /// <param name="type">Title・Narration</param>
    /// <param name="contents">内容</param>
    /// <param name="time">出力する時間</param>
    /// <param name="otherPosition">目標の座標</param>
    public void PrintText(string type, string contents, float time, Vector3 otherPosition)
    {
        if(!isTextPrinting)
            StartCoroutine(StartText(type, contents, time, otherPosition));
    }

    //---------------------------------------------------
    /// <summary>
    /// Story Title・Narrationを画面に出力
    /// </summary>
    /// <param name="type">Title・Narration</param>
    /// <param name="contents">内容</param>
    /// <param name="time">出力する時間</param>
    /// <param name="otherPosition">目標の座標</param>
    IEnumerator StartText(string type, string contents, float time, Vector3 otherPosition)
    {
        isTextPrinting = true;

        //  Text Object
        GameObject tmpObject = null;

        //  Panel Object
        GameObject tmpObjectPanel = null;

        //  タイプを分離
        switch (type)
        {
            case "title":
                //  Title Object
                tmpObject = StoryTitle;
                tmpObjectPanel = StoryTitlePanel;
                break;
            case "narration":
                //  Narration Object
                tmpObject = StoryNarration;
                tmpObjectPanel = StoryNarrationPanel;
                break;
        }

        //  座標を設定
        tmpObject.transform.position = 
            new Vector3(tmpObject.transform.position.x + otherPosition.x, 
                        tmpObject.transform.position.y + otherPosition.y, 
                        tmpObject.transform.position.z + otherPosition.z);
        tmpObjectPanel.transform.position =
            new Vector3(tmpObjectPanel.transform.position.x + otherPosition.x,
                        tmpObjectPanel.transform.position.y + otherPosition.y,
                        tmpObjectPanel.transform.position.z + otherPosition.z);

        //  内容を設定
        tmpObject.GetComponent<Text>().text = contents;

        //  Fade Out (Panel)
        tmpObjectPanel.GetComponent<FadeEffect>().StartFade(0.7f);
        yield return new WaitUntil(() => UIstepComplete == true);
        UIstepComplete = false;

        //  Fade Out (Text)
        tmpObject.GetComponent<FadeEffect>().StartFade(1f);
        yield return new WaitUntil(() => UIstepComplete == true);
        UIstepComplete = false;
        
        //  delay time
        yield return new WaitForSeconds(time);

        //  Fade In (Text)
        tmpObject.GetComponent<FadeEffect>().StartFade(1f);
        yield return new WaitUntil(() => UIstepComplete == true);
        UIstepComplete = false;

        //  Fade In (Panel)
        tmpObjectPanel.GetComponent<FadeEffect>().StartFade(0.7f);
        yield return new WaitUntil(() => UIstepComplete == true);
        UIstepComplete = false;

        isTextPrinting = false;

        //  STEP_COMPLETE イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// Ambassador Animationを実行
    /// </summary>
    /// <param name="contents">内容</param>
    /// <param name="time">delay time</param>
    /// <param name="otherPosition">目標の座標</param>
    public void PrintAmbassador(string contents, float time, Vector3 otherPosition)
    {
        if (!isAmbassadorPrinting)
            StartCoroutine(StartAmbassador(contents, time, otherPosition));
    }

    //---------------------------------------------------
    /// <summary>
    /// Ambassador Animation
    /// </summary>
    /// <param name="contents">内容</param>
    /// <param name="time">delay time</param>
    /// <param name="otherPosition">目標の座標</param>
    IEnumerator StartAmbassador(string contents, float time, Vector3 otherPosition)
    {
        isAmbassadorPrinting = true;

        //  Ambassador　Object
        GameObject tmpObject = CharacterAmbassador;

        tmpObject.name = "Character Ambassador";

        //  座標を設定
        tmpObject.transform.localPosition =
            new Vector3(265f + otherPosition.x,
                        155f + otherPosition.y,
                        0f + otherPosition.z);

        //  Ambassador Scale Change Animationを実行
        bool tmpBool = tmpObject.GetComponent<UIScaleAnimation>().StartScaleAnimation(time + (contents.Length * 0.1f));

        //  Textを出力
        string tmp = "";

        for (int i = 0; i < contents.Length; i++)
        {
            tmp += contents[i];

            tmpObject.transform.Find("Text").GetComponent<Text>().text = tmp;

            yield return new WaitForSeconds(0.05f);
        }

        isAmbassadorPrinting = false;
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
            case EVENT_TYPE.UI_STEP_COMPLETE:
                UIstepComplete = true;
                break;
        }
    }
}
