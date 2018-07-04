using UnityEngine;
using System.Collections;

/// <summary>
/// Story Modeのキャラクタを制御
/// </summary>
public class StoryCharacterController : MonoBehaviour, IListener
{

    //  角度を変更しているか
    private bool isRotating = false;

    //  移動しているか
    private bool isMoving = false;

    //  チェックポイントに到着したか
    private bool isArrived = false;

    //  ジャンプしているか
    private bool isJumping = false;

    //  座標をセットしているか
    private bool isSetting = false;

    //  Main Character
    private GameObject[] mainCharacter;

    //  Rotate Direction
    public Transform right;
    public Transform left;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static StoryCharacterController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static StoryCharacterController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("StoryCharacterControllerを実行");
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
        //  CHARACTER_ARRIVED　イベントをListenerに転送
        EventManager.Instance.AddListener(EVENT_TYPE.CHARACTER_ARRIVED, this);
    }

    //----------------------------------------------------
    /// <summary>
    /// キャラクターを回す
    /// </summary>
    /// <param name="mainCharacter">カメラがFocusしているキャラクター</param>
    /// <param name="direction">回す方向</param>
    public void CharacterRotate(GameObject[] mainCharacter, string direction)
    {
        if (isRotating == false)
        {
            //  Animation Activate
            StartCoroutine(StartRotate(mainCharacter, direction));
        }
    }

    //----------------------------------------------------
    /// <summary>
    /// Character Rotate Animation
    /// </summary>
    /// <param name="mainCharacter">カメラがFocusしているキャラクター</param>
    /// <param name="direction">回す方向</param>
    /// <returns></returns>
    IEnumerator StartRotate(GameObject[] mainCharacter, string direction)
    {
        isRotating = true;

        //  もとの角度をセーブ
        Quaternion from = Quaternion.Euler(0f, mainCharacter[0].transform.eulerAngles.y, 0f);

        //  目標の角度
        Quaternion to = from;

        //  角度を設定
        switch (direction)
        {
            case "right":
                to *= right.rotation;
                break;   
            case "left":
                to *= left.rotation;
                break;
        }

        float tmp = 0f;

        //  キャラクターを回す
        while (true)
        {
            //  以前の角度をセーブ
            tmp = mainCharacter[0].transform.eulerAngles.y;

            //  目標の角度まで回す
            for (int i = 0; i < mainCharacter.Length; i++)
                mainCharacter[i].transform.rotation = Quaternion.Slerp(mainCharacter[i].transform.rotation, to,Time.deltaTime * 5f);

            //  目標の角度まで回したかをチェック
            if ((Mathf.Floor(tmp) % 90 == 0 || Mathf.Ceil(tmp) % 90 == 0) && 
                (int)from.eulerAngles.y != (int)tmp)
            {
                //  確実に角度を設定
                for (int i = 0; i < mainCharacter.Length; i++)
                    mainCharacter[i].transform.rotation = to;

                //  Animationを終了
                break;
            }

            //  delay time
            yield return new WaitForSeconds(0.01f);
        }
        
        isRotating = false;

        // STEP_COMPLETE　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
    }

    //----------------------------------------------------
    /// <summary>
    /// キャラクターを移動させる
    /// </summary>
    /// <param name="mainCharacter">移動させるキャラクター</param>
    /// <param name="direction">移動させる方向</param>
    /// <param name="speed">スピード</param>
    /// <param name="force">移動モード</param>
    public void CharacterMove(GameObject[] mainCharacter, Vector3 direction, float speed, bool force = true)
    {
        if (isMoving == false)
        {
            this.mainCharacter = mainCharacter;

            //  Animation Activate
            StartCoroutine(StartMove(mainCharacter, direction, speed, force));
        }
    }

    //----------------------------------------------------
    /// <summary>
    /// Character Move Animation
    /// </summary>
    /// <param name="mainCharacter">移動させるキャラクター</param>
    /// <param name="direction">移動させる方向</param>
    /// <param name="speed">スピード</param>
    /// <param name="force">移動モード</param>
    IEnumerator StartMove(GameObject[] mainCharacter, Vector3 direction, float speed, bool force = true)
    {
        isMoving = true;

        //  チェックポイントに到着するまで実行
        while (!isArrived)
        {
            if (force)
            {
                for (int i = 0; i < mainCharacter.Length; i++)
                    mainCharacter[i].GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Acceleration);
            }
            else
            {
                Vector3 targetPosition = gameObject.transform.Find("Check Points").transform.Find("CheckPoint").transform.position;
                
                for (int i = 0; i < mainCharacter.Length; i++)
                    mainCharacter[i].transform.position += direction * speed * Time.deltaTime; 
            }
            yield return new WaitForSeconds(0.01f);
        }

        isMoving = false;

        isArrived = false;

        //  STEP_COMPLETE　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
    }

    //-------------------------------------------------
    /// <summary>
    /// キャラクターをジャンプさせるAnimationを実行
    /// </summary>
    /// <param name="mainCharacter">ジャンプさせるキャラクター</param>
    /// <param name="direction">ジャンプの方向</param>
    /// <param name="power">ジャンプする程度</param>
    /// <param name="delay">delay time</param>
    public void CharacterJump(GameObject[] mainCharacter, Vector3 direction, float power, float delay)
    {
        if(isJumping == false)
            StartCoroutine(JumpStart(mainCharacter, direction, power, delay));
    }

    //-------------------------------------------------
    /// <summary>
    /// キャラクターをジャンプさせる
    /// </summary>
    /// <param name="mainCharacter">ジャンプさせるキャラクター</param>
    /// <param name="direction">ジャンプの方向</param>
    /// <param name="power">ジャンプする程度</param>
    /// <param name="delay">delay time</param>
    IEnumerator JumpStart(GameObject[] mainCharacter, Vector3 direction, float power, float delay)
    {
        isJumping = true;

        //  キャラクターをジャンプさせる
        while (!isArrived)
        {
            for (int i = 0; i < mainCharacter.Length; i++)
                mainCharacter[i].GetComponent<Rigidbody>().AddForce(direction * power, ForceMode.Impulse);

            yield return new WaitForSeconds(delay);
        }

        isArrived = false;

        isJumping = false;

        // STEP_COMPLETE　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
    }

    //-------------------------------------------------
    /// <summary>
    /// キャラクターの座標を設定するAnimationを実行
    /// </summary>
    /// <param name="mainCharacter">Main Character</param>
    /// <param name="targetPosition">目標の座標</param>
    /// <param name="speed">スピード</param>
    public void SetPosition(GameObject[] mainCharacter, Vector3 targetPosition, float speed)
    {
        if(isSetting == false)
            StartCoroutine(SetPositionStart(mainCharacter, targetPosition, speed));
    }

    //-------------------------------------------------
    /// <summary>
    /// キャラクターの座標を設定する
    /// </summary>
    /// <param name="mainCharacter">Main Character</param>
    /// <param name="targetPosition">目標の座標</param>
    /// <param name="speed">スピード</param>
    IEnumerator SetPositionStart(GameObject[] mainCharacter, Vector3 targetPosition, float speed)
    {
        isSetting = true;

        float time = 0f;

        //  キャラクターを目標の座標に移動
        while (time <= 0.5f)
        {
            for (int i = 0; i < mainCharacter.Length; i++)
                mainCharacter[i].transform.position = Vector3.Lerp(mainCharacter[i].transform.position, targetPosition, Time.deltaTime * speed);

            yield return new WaitForSeconds(0.01f);

            time += 0.01f;
        }

        isSetting = false;

        // STEP_COMPLETE　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_COMPLETE, this);
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
            case EVENT_TYPE.CHARACTER_ARRIVED:
                OnArrived(Param as string);
                break;
        }
    }

    //---------------------------------------------------
    /// <summary>
    /// チェックポイントに到着した時実行
    /// </summary>
    /// <param name="otherName">到着したObjectの名前</param>
    private void OnArrived(string otherName)
    {
        Debug.Log(otherName + " : " + mainCharacter[0].name);
        if (mainCharacter[0].name.Equals(otherName))
        {
            //  画面にあるチェックポイントを全部削除
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("CheckPoint");

            foreach (GameObject item in tmp)
            {
                Destroy(item);
            }

            isArrived = true;
        }
    }
}
