using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cindella Story Scenario
/// </summary>
public class CindellaScenario : MonoBehaviour, IListener {

    //  Cindella Map
    public GameObject cindellaMap;

    //  Cindella Character
    public GameObject cindella;

    //  Cindella Animator
    Animator cinderellaAnim;

    //  Step Complete Status
    private bool stepComplete = false;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static CindellaScenario Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static CindellaScenario instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("CindellaScenarioを実行");
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
        Debug.Log("신데렐라 시나리오 시작");
#endif

        //  맵 출력
        StoryObjectController.Instance.PrintMap(cindellaMap, Vector3.zero, new Vector3(1f, 1f, 1f));

        //  캐릭터 출력
        StoryObjectController.Instance.PrintCharacter(cindella, "Cinderella", new Vector3(0f, 0.5f, 0f));

        //  메인 캐릭터 저장
        GameObject mainCharacter = GameObject.Find("Character").transform.Find("Cinderella").gameObject;

        //  신데렐라 에니메이터
        cinderellaAnim = mainCharacter.GetComponent<Animator>();

        //  카메라 세팅
        StoryCameraController.Instance.MC = mainCharacter;
        StoryCameraController.Instance.followCharacter = true;
        CameraOrthoController.Instance.StartMatrixBlender(3f);

        AnimChange(mainCharacter, "LeftIdle");

        //  지연시간
        yield return new WaitForSeconds(2f);

        //  동화 제목 및 나레이션 출력
        StoryUIController.Instance.PrintText("title", "데모 시나리오", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "chapter 0 동화모드 소개", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("안녕하세요 동화 모드의 설명을 시작할게요", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("동화를 보면서 펜토미노 게임을 즐길 수 있는 모드에요!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("다른 곳으로 이동 해볼게요!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "chapter 1 캐릭터의 움직임", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  캐릭터 대화창 출력

        //  캐릭터 움직임

        StoryObjectController.Instance.PrintCheckPoint(new Vector3(-0.9f, 0.5f, 0f));
        AnimChange(mainCharacter, "LeftWalk");
        StoryCharacterController.Instance.CharacterMove(new GameObject[] {mainCharacter},Vector3.left, 6f);
        yield return new WaitUntil(() => stepComplete == true);
        AnimChange(mainCharacter, "RightIdle");
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {mainCharacter}, new Vector3(-0.75f, 0.5f, 0f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {mainCharacter}, "right");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryObjectController.Instance.PrintCheckPoint(new Vector3(-0.8f, 3.8f, 4f));
        AnimChange(mainCharacter, "RightWalk");
        StoryCharacterController.Instance.CharacterJump(new GameObject[] {mainCharacter}, new Vector3(0f, 1f, 0.6f), 3.5f, 0.4f);
        yield return new WaitUntil(() => stepComplete == true);
        AnimChange(mainCharacter, "RightIdle");
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {mainCharacter}, new Vector3(-0.8f, 3.675f, 4f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {mainCharacter}, "right");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryObjectController.Instance.PrintCheckPoint(new Vector3(-2.4f, 3.5f, 4f));
        AnimChange(mainCharacter, "RightWalk");
        StoryCharacterController.Instance.CharacterMove(new GameObject[] {mainCharacter}, Vector3.left, 6f);
        yield return new WaitUntil(() => stepComplete == true);
        AnimChange(mainCharacter, "RightIdle");
        stepComplete = false;

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {mainCharacter}, "right");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryCharacterController.Instance.SetPosition(new GameObject[] {mainCharacter}, new Vector3(-2.35f, 3.675f, 4f), 5f);
        yield return new WaitUntil(() => stepComplete == true);
        AnimChange(mainCharacter, "LeftIdle");
        stepComplete = false;

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {mainCharacter}, "left");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "chapter 2 펜토미노 게임", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("이벤트 발생!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("규칙은 펜토미노 게임 규칙과 같아요", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("함께 풀어봐요!!!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);
        
        //  ------------    펜토 게임 시작
        //  카메라 세팅 해제
        StoryCameraController.Instance.followCharacter = false;
        CameraOrthoController.Instance.StartMatrixBlender(0f);

        //  펜토미노 게임 컨트롤러 실행
        PentoGameController.Instance.startPentomino("Story", 2, 0.07f, new Vector3(-1.531f, 2.9f, 6.42f), new Vector3(0f, 90f, 0f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        //  포커싱 모드 종료
        CameraController.Instance.StopFocusing();

        //  카메라 세팅
        CameraOrthoController.Instance.StartMatrixBlender(0f);
        
        //  부드럽게 캐릭터바라보기 에니메이션 실행
        StoryCameraController.Instance.StartLookAt(mainCharacter.transform.position);
        yield return new WaitUntil(() => StoryCameraController.Instance.IL == true);
        StoryCameraController.Instance.IL = false;

        //  캐릭터 팔로우 카메라 활성화
        StoryCameraController.Instance.followCharacter = true;


        CameraOrthoController.Instance.StartMatrixBlender(3f);
        Camera.main.orthographicSize = 3f;
        //  ------------    펜토 게임 종료

        StoryCharacterController.Instance.CharacterRotate(new GameObject[] {mainCharacter}, "right");
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("짝짝짝! 잘 푸셨어요", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("퍼즐은 클리어한 뒤에도 계속 남아있어요", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "chapter 3 스토리 재개", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        AnimChange(mainCharacter, "RightIdle");
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("퍼즐을 클리어 하면 스토리가 다시 시작되요", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintAmbassador("여기까지 동화모드의 설명입니다 감사합니다!", 1f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(2f);

        //StoryObjectController.Instance.PrintCheckPoint(new Vector3(-2.35f, 5.38f, 1.63f));
        //AnimChange(mainCharacter, "RightWalk");
        //StoryCharacterController.Instance.CharacterJump(GameObject.Find("Character").transform.Find("Cinderella").gameObject, new Vector3(0f, 1f, -0.6f), 2f, 0.3f);
        //yield return new WaitUntil(() => stepComplete == true);
        //AnimChange(mainCharacter, "LeftIdle");
        //stepComplete = false;

        //StoryCharacterController.Instance.SetPosition(GameObject.Find("Character").transform.Find("Cinderella").gameObject, new Vector3(-2.35f, 5.275f, 1.66f), 5f);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryCharacterController.Instance.CharacterRotate(GameObject.Find("Character").transform.Find("Cinderella").gameObject, "left");
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryObjectController.Instance.PrintCheckPoint(new Vector3(-1.5f, 5.3f, 1.7f));
        //AnimChange(mainCharacter, "LeftWalk");
        //StoryCharacterController.Instance.CharacterMove(GameObject.Find("Character").transform.Find("Cinderella").gameObject, Vector3.right, 6f);
        //yield return new WaitUntil(() => stepComplete == true);
        //AnimChange(mainCharacter, "LeftIdle");
        //stepComplete = false;

        //StoryCharacterController.Instance.SetPosition(GameObject.Find("Character").transform.Find("Cinderella").gameObject, new Vector3(-1.65f, 5.275f, 1.66f), 5f);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryCharacterController.Instance.CharacterRotate(GameObject.Find("Character").transform.Find("Cinderella").gameObject, "right");
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryObjectController.Instance.PrintCheckPoint(new Vector3(-1.65f, 5.275f, 4.65f));
        //AnimChange(mainCharacter, "LeftWalk");
        //StoryCharacterController.Instance.CharacterMove(GameObject.Find("Character").transform.Find("Cinderella").gameObject, Vector3.forward, 6f);
        //yield return new WaitUntil(() => stepComplete == true);
        //AnimChange(mainCharacter, "LeftIdle");
        //stepComplete = false;

        //StoryCharacterController.Instance.SetPosition(GameObject.Find("Character").transform.Find("Cinderella").gameObject, new Vector3(-1.65f, 5.275f, 4.8f), 5f);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("동화 도중 대화창이나 카메라 / 캐릭터의 이동은 주인공에 한하지 않습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("다른 캐릭터를 이동시키거나 대화창을 띄우고 카메라를 비추는 것이 가능합니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("또한 동화 진행중 카메라는 2D모드를 유지하며 게임 진행 시에는 3D모드를 함께 사용합니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("두가지 카메라 모드의 차이점을 보겠습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintText("narration", "카메라 3D모드로 전환", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //CameraOrthoController.Instance.StartMatrixBlender(0f);

        //yield return new WaitForSeconds(1f);

        //StoryUIController.Instance.PrintText("narration", "카메라 2D모드로 전환", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //CameraOrthoController.Instance.StartMatrixBlender(3f);

        //yield return new WaitForSeconds(1f);

        //StoryUIController.Instance.PrintAmbassador("다시 스토리를 재개하겠습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintText("narration", "스토리 재개", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryCharacterController.Instance.CharacterRotate(GameObject.Find("Character").transform.Find("Cinderella").gameObject, "left");
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryObjectController.Instance.PrintCheckPoint(new Vector3(1.5f, 5.275f, 4.8f));
        //AnimChange(mainCharacter, "LeftWalk");
        //StoryCharacterController.Instance.CharacterMove(GameObject.Find("Character").transform.Find("Cinderella").gameObject, Vector3.right, 6f);
        //yield return new WaitUntil(() => stepComplete == true);
        //AnimChange(mainCharacter, "LeftIdle");
        //stepComplete = false;

        //StoryCharacterController.Instance.CharacterRotate(GameObject.Find("Character").transform.Find("Cinderella").gameObject, "left");
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryCharacterController.Instance.SetPosition(GameObject.Find("Character").transform.Find("Cinderella").gameObject, new Vector3(1.65f, 5.275f, 4.8f), 5f);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("펜토미노 게임의 퍼즐은 어떤 각도 어떤 크기에서도 작동합니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintText("narration", "두번째 펜토미노 이벤트 진행", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        ////  ------------    펜토 게임 시작
        ////  카메라 세팅 해제
        //StoryCameraController.Instance.followCharacter = false;
        //CameraOrthoController.Instance.StartMatrixBlender(0f);

        ////  펜토미노 게임 컨트롤러 실행
        //PentoGameController.Instance.startPentomino("Story", 3, 0.03f, new Vector3(2.4f, 5.342f, 4.77f), new Vector3(30f, -82.7f, 60f));
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        ////  포커싱 모드 종료
        //CameraController.Instance.StopFocusing();

        ////  카메라 세팅
        //CameraOrthoController.Instance.StartMatrixBlender(0f);

        ////  부드럽게 캐릭터바라보기 에니메이션 실행
        //StoryCameraController.Instance.StartLookAt(mainCharacter.transform.position);
        //yield return new WaitUntil(() => StoryCameraController.Instance.IL == true);
        //StoryCameraController.Instance.IL = false;

        ////  캐릭터 팔로우 카메라 활성화
        //StoryCameraController.Instance.followCharacter = true;


        //CameraOrthoController.Instance.StartMatrixBlender(3f);
        //Camera.main.orthographicSize = 3f;
        ////  ------------    펜토 게임 종료

        //StoryUIController.Instance.PrintAmbassador("퍼즐을 모두 푸셨습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("두번째 퍼즐도 해당 위치에 그대로 남아있음을 볼 수 있습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintAmbassador("이렇게 동화모드의 간략한 데모 시나리오를 마치겠습니다.", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //StoryUIController.Instance.PrintText("narration", "스토리 종료", 1f, Vector3.zero);
        //yield return new WaitUntil(() => stepComplete == true);
        //stepComplete = false;

        //  씬 이름을 사용해 씬 전환
        GameController.Instance.SN = "StoryScrollView";

        //  목표 지점 도달 시 카메라 변환

        //  캐릭터 움직임

        //  목표 지점 도달 시 대화창

        //  나레이션 출력

        //  게임 실행
    }

    //-------------------------------------------------
    /// <summary>
    /// 에니메이션 변경
    /// </summary>
    /// <param name="Character">캐릭터</param>
    /// <param name="anim">변경할 에니메이션</param>
    private void AnimChange(GameObject Character, string anim)
    {
        Animator CA = null;
        switch (Character.name)
        {
            case "Cinderella":
                CA = cinderellaAnim;
                break;
        }

        switch (anim)
        {
            case "RightIdle":
                CA.SetBool("walk", false);
                Character.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case "LeftIdle":
                CA.SetBool("walk", false);
                Character.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
            case "LeftWalk":
                CA.SetBool("walk", true);
                Character.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
            case "RightWalk":
                CA.SetBool("walk", true);
                Character.transform.localScale = new Vector3(1f, 1f, 1f);
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
