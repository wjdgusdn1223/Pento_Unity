using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼 시나리오
/// </summary>
public class TutorialScenario : MonoBehaviour, IListener
{

    //  다음 단계로 넘어갈지 여부
    private bool stepComplete = false;

    //  튜토리얼 단계
    private int tutorialStep = 0;



    //-----------------------------------------------------
    /// <summary>
    /// 싱글턴 인스턴스 요청,
    /// get 접근자만 가지는 읽기 전용 프로퍼티
    /// </summary>
    public static TutorialScenario Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static TutorialScenario instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("튜토리얼 시나리오 준비");
#endif

        //  씬에 이미 인스턴스가 존재하는지 검사한다
        //  존재하는 경우 이 인스턴스는 소멸시킨다
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        //  이 인스턴스를 유효한 유일 오브젝트로 만든다
        instance = this;
    }

    //---------------------------------------------------
    // Use this for initialization
    private void Start()
    {
        //  단계 완료 이벤트를 수신하기 위해 스스로를 리스너로 등록한다
        EventManager.Instance.AddListener(EVENT_TYPE.STEP_COMPLETE, this);
        //  단계 완료 이벤트를 수신하기 위해 스스로를 리스너로 등록한다
        EventManager.Instance.AddListener(EVENT_TYPE.TUTORIAL_STEP_COMPLETE, this);
    }

    private void Update()
    {

    }

    //---------------------------------------------------
    /// <summary>
    /// 시나리오 실행
    /// </summary>
    public void StartScenario()
    {
        //  시나리오 에니메이션 실행
        StartCoroutine(TutorialAnimation());
    }

    //---------------------------------------------------
    /// <summary>
    /// 시나리오 실행
    /// </summary>
    IEnumerator TutorialAnimation()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("튜토리얼 시나리오 시작");
#endif

        StoryUIController.Instance.PrintText("narration", "펜토미노 튜토리얼 시작합니다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        //  펜토미노 게임 컨트롤러 실행
        PentoGameController.Instance.startPentomino("Tutorial", -1, 0.2f, new Vector3(-3f, 4.8f, 4.2f), new Vector3(0f, 0f, 0f));
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        StoryUIController.Instance.PrintText("narration", "퍼즐 우측에는 타이머와 진행 상태창이 있습니다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        StoryUIController.Instance.PrintText("narration", "보드에 퍼즐모양의 불이 켜져있는 것을 볼 수 있습니다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        StoryUIController.Instance.PrintText("narration", "퍼즐 모양 안에 블록을 올려 놓으세요", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 1);

        StoryUIController.Instance.PrintText("narration", "화면 상에 블록이 표시되는 것을 볼 수 있습니다. ", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        StoryUIController.Instance.PrintText("narration", "퍼즐 모양 밖에 블록을 올려 놓으세요", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 2);

        StoryUIController.Instance.PrintText("narration", "틀린 경우 화면이 흔들립니다.", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        StoryUIController.Instance.PrintText("narration", "퍼즐을 완성시켜 주세요", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 3);

        StoryUIController.Instance.PrintText("narration", "손으로 왼쪽의 색을 선택해 퍼즐을 칠해 봅시다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 4);

        StoryUIController.Instance.PrintText("narration", "우측 하단의 큐브를 손으로 선택해봅시다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 5);

        StoryUIController.Instance.PrintText("narration", "손을 왼쪽이나 오른쪽으로 휘둘러 퍼즐을 돌려봅시다", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 6);

        yield return new WaitForSeconds(4f);

        StoryUIController.Instance.PrintText("narration", "여기까지 펜토미노 게임의 간략한 설명을 마치겠습니다.", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitForSeconds(1f);

        StoryUIController.Instance.PrintText("narration", "저장 버튼을 누르면 튜토리얼이 종료됩니다.", 2f, Vector3.zero);
        yield return new WaitUntil(() => stepComplete == true);
        stepComplete = false;

        yield return new WaitUntil(() => tutorialStep == 7);

        //  포커싱 모드 종료
        CameraController.Instance.StopFocusing();

        //  씬 이름을 사용해 씬 전환
        GameController.Instance.SN = "MainMenu";
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
            case EVENT_TYPE.TUTORIAL_STEP_COMPLETE:
                //  튜토리얼 박스 
                if (tutorialStep == ((int)Param - 1))
                {
                    tutorialStep = (int)Param;
                }
                break;
        }
    }
}