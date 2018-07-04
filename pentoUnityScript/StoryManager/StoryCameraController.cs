using UnityEngine;
using System.Collections;

/// <summary>
/// Story Modeのカメラを制御
/// </summary>
public class StoryCameraController : MonoBehaviour
{
    //  カメラのスピード
    public float speed = 5f;

    //  目標にカメラを向けているかどうか
    private bool isLooking = false;

    //  Camera follow mode
    private bool isLerp = true;

    //-------------------------------------------------
    /// <summary>
    /// mainCharacter c# property
    /// </summary>
    public GameObject MC
    {
        get { return mainCharacter; }
        set
        {
            mainCharacter = value;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// isLooking c# property
    /// </summary>
    public bool IL
    {
        get { return isLooking; }
        set
        {
            isLooking = value;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// isLerp c# property
    /// </summary>
    public bool ILP
    {
        get { return isLerp; }
        set
        {
            isLerp = value;
        }
    }

    //------------------------------------------------
    //  対象のキャラクター
    private GameObject mainCharacter;

    //  カメラのモード
    public bool followCharacter = false;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryCameraController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryCameraController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryCameraControllerを実行");
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
    private void LateUpdate()
    {
        if (followCharacter)
        {
            //  Characterとの距離
            Vector3 distance = new Vector3(0f, 0.3f, -10f);

            //  角度を求める
            Quaternion CharacterRotateValue = Quaternion.Euler(0f, mainCharacter.transform.eulerAngles.y, 0f);

            //  角度と距離を使って目標座標を計算
            Vector3 targetPoint = CharacterRotateValue * distance;

            //  カメラを目標の座標に移動
            if (isLerp)
            {
                Camera.main.transform.position = 
                    Vector3.Lerp(Camera.main.transform.position,
                                    targetPoint + mainCharacter.transform.position, 
                                    Time.deltaTime * speed);
            }
            else
            {
                Camera.main.transform.position = (targetPoint + mainCharacter.transform.position);
            }
            
            //  カメラがCharacterを向けるように回転させる
            Camera.main.transform.LookAt(mainCharacter.transform.localPosition);
        }
    }

    //------------------------------------------------------
    /// <summary>
    /// LookAt Character Animation Activate
    /// </summary>
    /// <param name="targetCoordinate">目標の座標</param>
    public void StartLookAt(Vector3 targetCoordinate)
    {
        if (isLooking == false)
            StartCoroutine(LookAtAnimation(targetCoordinate));
    }

    //------------------------------------------------------
    /// <summary>
    /// LookAt Character Animation
    /// </summary>
    /// <param name="targetCoordinate">目標の座標</param>
    /// <returns></returns>
    IEnumerator LookAtAnimation(Vector3 targetCoordinate)
    {
        float time = 0f;

        //  LookAt Character
        while (time < 2f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetCoordinate - Camera.main.transform.position);

            // Smoothly rotate towards the target point.
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, targetRotation, 2f * Time.deltaTime);

            yield return new WaitForSeconds(0.01f);

            time += Time.deltaTime;
        }

        isLooking = true;
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
