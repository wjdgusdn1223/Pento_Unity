using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HanselAndGretel Story Scenario
/// </summary>
public class HanselAndGretelScenario : MonoBehaviour, IListener {

    //  Forest Map
    public GameObject Forest;

    //  SnackHouse Map
    public GameObject Room;

    //  Hansel Character
    public GameObject Hansel;

    //  Gretel Character
    public GameObject Gretel;

    //  Witch Character
    public GameObject Witch;

    //  Hansel Animator
    Animator hanselAnim;

    //  Gretel Animator
    Animator gretelAnim;

    //  Witch Animator
    Animator witchAnim;

    //  Step Complete Status
    private bool stepComplete = false;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static HanselAndGretelScenario Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static HanselAndGretelScenario instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("HanselAndGretelを実行");
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
        //  STEP_COMPLETE イベントをListenerに転送
        EventManager.Instance.AddListener(EVENT_TYPE.STEP_COMPLETE, this);
    }

    //---------------------------------------------------
    /// <summary>
    /// Start　Scenario
    /// </summary>
    public void StartScenario()
    {
        //  Start　Scenario
        StartCoroutine(ScenarioAnimation());
    }

    //---------------------------------------------------
    /// <summary>
    /// Start　Scenario
    /// </summary>
    IEnumerator ScenarioAnimation()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("Hansel And Gretel Scenario Start");
#endif
        //  カメラセット
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;

        //  ライトを非活性か
        GameObject.Find("Directional light").gameObject.SetActive(false);

        //  Sound Play
        SoundController.Instance.PlaySound("Forest");

        //  Create Map
        StoryObjectController.Instance.PrintMap(Forest, new Vector3(-60f, -2f, 35f), new Vector3(0.1f, 0.1f, 0.1f));

        //  キャラクターを生成
        StoryObjectController.Instance.PrintCharacter(Hansel, "Hansel", new Vector3(14f, 0f, 0f));
        StoryObjectController.Instance.PrintCharacter(Gretel, "Gretel", new Vector3(12f, 0f, 0f));

        //  キャラクターをセット
        GameObject objGretel = GameObject.Find("Character").transform.Find("Gretel").gameObject;
        GameObject objHansel = GameObject.Find("Character").transform.Find("Hansel").gameObject;

        //  Animator Setting
        gretelAnim = objGretel.GetComponent<Animator>();
        hanselAnim = objHansel.GetComponent<Animator>();

        //  カメラセット
        StoryCameraController.Instance.MC = objHansel;
        StoryCameraController.Instance.followCharacter = true;
        CameraOrthoController.Instance.StartMatrixBlender(5.5f);

        //  Change Animation
        AnimChange(objGretel, "RightIdle");
        AnimChange(objHansel, "RightIdle");

        //  delay time
        yield return new WaitForSeconds(2f);

        //  동화 제목 및 나레이션 출력
        StoryUIController.Instance.PrintText("title", "헨젤과 그레텔", 1.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "깊은  숲 속을 헤메는 헨젤과 그레텔", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  カメラセット
        StoryCameraController.Instance.ILP = false;

        //  キャラクターを移動させる
        StoryObjectController.Instance.PrintCheckPoint(new Vector3(22.7f, 0f, 0f));
        AnimChange(objHansel, "RightWalk");
        AnimChange(objGretel, "RightWalk");
        StoryCharacterController.Instance.CharacterMove(new GameObject[] {objHansel, objGretel}, Vector3.right, 4.2f, false);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {objHansel}, new Vector3(23f, objHansel.transform.localPosition.y, 0f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {objGretel}, new Vector3(22f, objGretel.transform.localPosition.y, 0f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objHansel, "RightSurprise");
        AnimChange(objGretel, "RightSurprise");

        StoryUIController.Instance.PrintAmbassador("과자로 만들어진... 집?", 0.2f, new Vector3(-190f, 0f, 0f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  カメラセット
        StoryCameraController.Instance.ILP = true;

        //  Change Animation
        AnimChange(objHansel, "RightIdle");
        AnimChange(objGretel, "RightTalk");

        //  Change Animation
        AnimChange(objGretel, "RightIdle");
        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("안에 사람이 있으면 도움을 요청해보자", 0.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightWalk");
        AnimChange(objHansel, "RightWalk");

        StoryObjectController.Instance.PrintCheckPoint(new Vector3(26.1f, 0.6f, 0f));
        StoryCharacterController.Instance.CharacterJump(new GameObject[] {objHansel, objGretel}, new Vector3(0.5f, 0.5f, 0f), 4.7f, 0.5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightIdle");
        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("문고리가 없어서 열수가 없어!", 0.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {objHansel}, new Vector3(26.5f, objHansel.transform.localPosition.y, 0f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objHansel, "RightIdle");

        StoryUIController.Instance.PrintText("narration", "문고리를 만들어 주세요!!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  ------------    펜토 게임 시작
        //  카메라 세팅 해제
        StoryCameraController.Instance.followCharacter = false;
        CameraOrthoController.Instance.StartMatrixBlender(0f);

        //  펜토미노 게임 컨트롤러 실행
        PentoGameController.Instance.startPentomino("Story", 51, 0.05f, new Vector3(27.857f, 0.536f, 0.179f), new Vector3(0f, 28f, 0f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  포커싱 모드 종료
        CameraController.Instance.StopFocusing();

        //  카메라 세팅
        CameraOrthoController.Instance.StartMatrixBlender(0f);
        
        //  부드럽게 캐릭터바라보기 에니메이션 실행
        //StoryCameraController.Instance.StartLookAt(objHansel.transform.position);
        //yield return new WaitUntil(() => StoryCameraController.Instance.IL == true);
        //StoryCameraController.Instance.IL = false;

        //  캐릭터 팔로우 카메라 활성화
        StoryCameraController.Instance.followCharacter = true;


        CameraOrthoController.Instance.StartMatrixBlender(5.5f);
        Camera.main.orthographicSize = 5.5f;
        //  ------------    펜토 게임 종료

        //  Change Animation
        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("문고리가 만들어졌어! 들어가보자!!", 0.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objHansel, "LeftIdle");

        //  delay time
        yield return new WaitForSeconds(0.5f);

        //  Create Map
        StoryObjectController.Instance.PrintMap(Room, new Vector3(0f, -100f, 0f), new Vector3(0.04f, 0.04f, 0.04f));

        //  Camera Setting
        CameraOrthoController.Instance.StartMatrixBlender(3f);

        SoundController.Instance.PlayEffectSound("Page");

        //  キャラクターを移動
        objHansel.transform.localPosition = new Vector3(-1.06f, -99f, 0f);
        objGretel.transform.localPosition = new Vector3(-1.82f, -99f, 1.08f);
        
        //  Change Animation
        AnimChange(objHansel, "RightIdle");
        AnimChange(objGretel, "RightIdle");

        StoryUIController.Instance.PrintText("narration", "과자 집 내부", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {objHansel, objGretel}, "left");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  delay time
        yield return new WaitForSeconds(0.5f);

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {objHansel, objGretel}, "right");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("우와 집 안에도 과자가 가득해!!", 0.2f, new Vector3(-400f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightSmile");
        AnimChange(objHansel, "LeftIdle");

        //  delay time
        yield return new WaitForSeconds(1f);

        //  Change Animation
        AnimChange(objHansel, "RightIdle");

        //  delay time
        yield return new WaitForSeconds(1f);

        //  Change Animation

        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("집주인은 안 계시는 건가?", 0.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Sound Play
        SoundController.Instance.PlaySound("Witch");

        SoundController.Instance.PlayEffectSound("Ws");

        //  Witch Effect
        EffectController.Instance.SpawnParticle(22, "", 1.2f, new Vector3(0.83f, -99.5f, -1.22f));

        //  キャラクターを生成
        StoryObjectController.Instance.PrintCharacter(Witch, "Witch", new Vector3(0.76f, -99f, -1.22f));

        //  キャラクターをセット
        GameObject objWitch = GameObject.Find("Character").transform.Find("Witch").gameObject;

        //  Animator Setting
        witchAnim = objWitch.GetComponent<Animator>();

        //  Change Animation
        AnimChange(objHansel, "RightSurprise");
        AnimChange(objGretel, "RightSurprise");
        AnimChange(objWitch, "LeftIdle");

        //  delay time
        yield return new WaitForSeconds(0.5f);

        //  Change Animation
        AnimChange(objWitch, "LeftTalk");

        StoryUIController.Instance.PrintAmbassador("이 녀석들! 누구 마음대로 들어온거야!!", 0.2f, new Vector3(100f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objWitch, "LeftSmile");

        StoryUIController.Instance.PrintAmbassador("대가로 너희를 잡아먹겠다!!!", 0.2f, new Vector3(100f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objWitch, "LeftIdle");

        StoryUIController.Instance.PrintAmbassador("꺄악! 어떻게 해?", 0.2f, new Vector3(-400f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("마녀를 막아야해!", 0.2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightIdle");
        AnimChange(objHansel, "RightIdle");

        StoryUIController.Instance.PrintText("narration", "마녀로부터 아이들을 지켜주세요!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  ------------    펜토 게임 시작
        //  카메라 세팅 해제
        StoryCameraController.Instance.followCharacter = false;
        CameraOrthoController.Instance.StartMatrixBlender(0f);

        //  펜토미노 게임 컨트롤러 실행
        PentoGameController.Instance.startPentomino("Story", 52, 0.16f, new Vector3(0.83f, -99.2f, -2.2f), new Vector3(0f, 0f, 0f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  포커싱 모드 종료
        CameraController.Instance.StopFocusing();

        //  카메라 세팅
        CameraOrthoController.Instance.StartMatrixBlender(0f);
        
        //  부드럽게 캐릭터바라보기 에니메이션 실행
        StoryCameraController.Instance.StartLookAt(objHansel.transform.position);
        yield return new WaitUntil(() => StoryCameraController.Instance.IL == true);
        StoryCameraController.Instance.IL = false;

        //  캐릭터 팔로우 카메라 활성화
        StoryCameraController.Instance.followCharacter = true;


        CameraOrthoController.Instance.StartMatrixBlender(3f);
        Camera.main.orthographicSize = 3f;
        //  ------------    펜토 게임 종료

        //  Change Animation
        AnimChange(objGretel, "RightSurprise");
        AnimChange(objHansel, "RightSurprise");
        AnimChange(objWitch, "LeftSurprise");

        //  delay time
        yield return new WaitForSeconds(0.5f);

        //  Sound Play
        SoundController.Instance.PlaySound("WitchDead");

        StoryUIController.Instance.PrintAmbassador("으아아악!!!!", 0.2f, new Vector3(100f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        SoundController.Instance.PlayEffectSound("Wd");

        //  Witch dead Effect
        EffectController.Instance.SpawnParticle(14, "", 1.2f, new Vector3(0.83f, -99.4f, -1.22f));

        //  Witch Destroy
        Destroy(objWitch);

        //  Change Animation
        AnimChange(objGretel, "RightTalk");
        AnimChange(objHansel, "LeftIdle");

        StoryUIController.Instance.PrintAmbassador("마녀가 과자로 변했어!", 1f, new Vector3(-400f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightSmile");
        AnimChange(objHansel, "RightSmile");

        //  delay time
        yield return new WaitForSeconds(0.5f);

        //  Change Animation
        AnimChange(objGretel, "RightTalk");
        AnimChange(objHansel, "RightTalk");

        StoryUIController.Instance.PrintAmbassador("우리를 도와줘서 고마워!!!", 0.2f, new Vector3(-200f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("덕분에 마녀를 물리칠 수 있었어", 0.2f, new Vector3(-200f, 0.03f, 0.03f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  Change Animation
        AnimChange(objGretel, "RightSmile");
        AnimChange(objHansel, "RightSmile");

        StoryUIController.Instance.PrintText("narration", "헨젤과 그레텔 끝", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  씬 이름을 사용해 씬 전환
        GameController.Instance.SN = "StoryScrollView";
    }

    //-------------------------------------------------
    /// <summary>
    /// Animation Change
    /// </summary>
    /// <param name="Character">Character</param>
    /// <param name="anim">target animation</param>
    private void AnimChange(GameObject Character, string anim)
    {
        Animator CA = null;
        float scale = 1f;

        switch (Character.name)
        {
            case "Hansel":
                CA = hanselAnim;
                break;
            case "Gretel":
                CA = gretelAnim;
                break;
            case "Witch":
                CA = witchAnim;
                scale = 1.4f;
                break;
        }

        switch (anim)
        {
            case "RightIdle":
                CA.SetInteger("Status", 0);
                Character.transform.localScale = new Vector3(scale, scale, scale);
                break;
            case "LeftIdle":
                CA.SetInteger("Status", 0);
                Character.transform.localScale = new Vector3(scale * -1f, scale, scale);
                break;
            case "LeftWalk":
                CA.SetInteger("Status", 1);
                Character.transform.localScale = new Vector3(scale * -1f, scale, scale);
                break;
            case "RightWalk":
                CA.SetInteger("Status", 1);
                Character.transform.localScale = new Vector3(scale, scale, scale);
                break;
            case "RightSmile":
                CA.SetInteger("Status", 2);
                Character.transform.localScale = new Vector3(scale, scale, scale);
                break;
            case "LeftSmile":
                CA.SetInteger("Status", 2);
                Character.transform.localScale = new Vector3(scale * -1f, scale, scale);
                break;
            case "RightSurprise":
                CA.SetInteger("Status", 3);
                Character.transform.localScale = new Vector3(scale, scale, scale);
                break;
            case "LeftSurprise":
                CA.SetInteger("Status", 3);
                Character.transform.localScale = new Vector3(scale * -1f, scale, scale);
                break;
            case "RightTalk":
                CA.SetInteger("Status", 4);
                Character.transform.localScale = new Vector3(scale, scale, scale);
                break;
            case "LeftTalk":
                CA.SetInteger("Status", 4);
                Character.transform.localScale = new Vector3(scale * -1f, scale, scale);
                break;
        }
    }

    //-------------------------------------------------
    /// <summary>
    /// 이벤트가 발생할 때 리스너에서 호출할 함수
    /// </summary>
    /// <param name="Event_Type">받는 이벤트</param>
    /// <param name="Sender">받을 컴포넌트</param>
    /// <param name="Param">매개 변수</param>
    /// <param name="Param2">매개 변수</param>
    public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null, object Param2 = null)
    {
        switch (Event_Type)
        {
            case EVENT_TYPE.STEP_COMPLETE:
                // 단계 넘기기
                stepComplete = true;
                break;
        }
    }
}
