using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームUIを制御
/// </summary>
public class GameUIController : MonoBehaviour, IListener
{
    //  UI Parent Object
    public Transform Canvas;

    //  shake time
    public float shake = 0.15f;

    //  Shake degree
    public float shakeAmount = 0.05f;
    public float decreaseFactor = 1.0f;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static GameUIController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static GameUIController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("GameUIControllerを実行");
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
        //  WRONG_BLOCK イベントを受信するためにlistnerに登録
        EventManager.Instance.AddListener(EVENT_TYPE.WRONG_BLOCK, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// WRONG_BLOCK　Effect
    /// </summary>
    IEnumerator WrongBlockEffect()
    {
        //  'X' image GameObject
        GameObject WB = transform.Find("Pento Canvas").transform.Find("Wrong").gameObject;

        //  座標を設定
        WB.transform.localPosition = new Vector3(0f, 0f, 0f);

        //　サイズを設定
        WB.transform.localScale = new Vector3(0f, 0f, 0f);

        //　Shake Camera Animation
        StartCoroutine(ShakeCamera());

        //  'X' Scale Animation
        WB.GetComponent<UIScaleAnimation>().StartScaleAnimation(0.5f, false);

        yield return null;
    }

    //----------------------------------------------------------
    /// <summary>
    /// Shake Camera Animation
    /// </summary>
    IEnumerator ShakeCamera()
    {
        //  --------- Shake Camera Animation
        Vector3 originalPos = Camera.main.transform.localPosition;

        float shaketime = shake;

        while (shaketime > 0)
        {
            Camera.main.transform.localPosition = 
                originalPos + Random.insideUnitSphere * shakeAmount * PentoGameController.Instance.BS;

            shaketime -= Time.deltaTime * decreaseFactor;

            yield return new WaitForSeconds(0.05f);
        }

        Camera.main.transform.localPosition = originalPos;
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
            case EVENT_TYPE.WRONG_BLOCK:
                //  WRONG_BLOCK　Effect
                OnWrongBlock();
                break;
        }
    }

    //---------------------------------------------------
    /// <summary>
    /// WRONG_BLOCK　Effect Activate
    /// </summary>
    public void OnWrongBlock()
    {
        StartCoroutine(WrongBlockEffect());
    }
}
