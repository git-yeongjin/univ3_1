using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CleanDayUI : MonoBehaviour
{
    //위생 점검 제한시간 가져오기
    private CleanEvent CE;

    [Header("위생 관리 타이머")]
    //public TMP_Text CleanDayEvent_TimeLimitText;
    public Slider CleanDayEvent_TimeLimitSlider;

    [Header("30초 경고 알림")]
    public GameObject WarningUI;
    public TMP_Text WarningText;
    private bool isWarningShown = false;

    [Header("최종 검사 결과창")]
    public GameObject ResultUI;
    public Image ResultImage;
    [Tooltip("0: 매우 우수, 1: 우수, 2: 보통")]
    public Sprite[] ResultSprites;

    [Header("플레이어 시작 대사 UI")]
    public GameObject PlayerDialogueUI;
    public TMP_Text PlayerDialogueText;

    [TextArea]
    public string[] PlayerDialogues = {
        "오늘 위생관리원이 오는 날이야",
        "서둘러 가게를 청소하자",
    };

    private int currentDialogueIndex = 0;
    private bool isPlayerDialogueActive = false;

    [Header("크리쳐 흔적 엔딩 설정")]
    public GameObject CleanEnding;       // 최종 엔딩 일러스트 이미지
    public CanvasGroup EndingFadeGroup;     // 화면 까매지는 페이드 패널
    public GameObject EndingDialogueUI;     // 까만 화면 위에 띄울 대사창 UI
    public TMP_Text EndingDialogueText;     // 대사 텍스트

    [TextArea]
    public string[] CleanEndingDialogues = {
        "“검사 대상인 가게에서 정체불명의 재료를 사용하는 것을 발견했습니다.”",
        "그 남자가 그 말을 한 뒤로 모든 게 끝나버렸다.",
        "사랑하는 것을 먹는 게 뭐가 문제지?",
        "이제까지 먹지 않은 너네들이 바보잖아",
        "사람들은 너무 성가셔",
        "역시 사랑둥이로 변하는 게 훨씬 나아",
        "...이제 못하지만"
    };

    void Start()
    {
        CE = FindAnyObjectByType<CleanEvent>();
        if (CE == null)
        {
            Debug.LogError("DayEvent스크립트를 찾을 수 없음");
        }
        else if (CleanDayEvent_TimeLimitSlider != null)
        {
            CleanDayEvent_TimeLimitSlider.maxValue = CE.CleanDayEvent_TimeLimit;
            CleanDayEvent_TimeLimitSlider.value = CE.CleanDayEvent_TimeLimit;
        }

        if (CleanEnding != null) CleanEnding.SetActive(false);
        if (WarningUI != null) WarningUI.SetActive(false);
        if (ResultUI != null) ResultUI.SetActive(false);

        StartPlayerDialogue();
    }

    private void StartPlayerDialogue()
    {
        if (PlayerDialogueUI != null && PlayerDialogues.Length > 0)
        {
            isPlayerDialogueActive = true;
            PlayerDialogueUI.SetActive(true);
            PlayerDialogueText.text = PlayerDialogues[0];
            currentDialogueIndex = 1;
        }
        else
        {
            // UI 연결이 안 되어있거나 대사가 없으면 바로 청소 시작
            if (CE != null) CE.StartCleanEvent();
        }
    }


    void Update()
    {
        if (isPlayerDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (currentDialogueIndex < PlayerDialogues.Length)
                {
                    PlayerDialogueText.text = PlayerDialogues[currentDialogueIndex];
                    currentDialogueIndex++;
                }
                else
                {
                    // 대사가 모두 끝났을 때
                    isPlayerDialogueActive = false;
                    if (PlayerDialogueUI != null) PlayerDialogueUI.SetActive(false);

                    // 대사창을 닫고 청소 시작
                    if (CE != null) CE.StartCleanEvent();
                }
            }
            return;
        }

        if (CE != null && CleanDayEvent_TimeLimitSlider != null)
        {
            float currentTime = Mathf.Max(0f, CE.CleanDayEvent_TimeLimit);
            CleanDayEvent_TimeLimitSlider.value = currentTime;

            if (currentTime <= 30f && currentTime > 0f && !isWarningShown)
            {
                isWarningShown = true;
                ShowWarningMessage("위생 검사원이 곧 방문합니다!");
                //bgm빠르게 하기
            }
        }
    }

    private void ShowWarningMessage(string msg)
    {
        if (WarningUI != null)
        {
            WarningUI.SetActive(true);
            if (WarningText != null) WarningText.text = msg;
        }
    }

    //위생점검 결과창 호출
    public void ShowResultUI(int resultLevel)
    {
        if (ResultUI != null) ResultUI.SetActive(true);

        // 결과 등급에 맞게 스프라이트 교체
        if (ResultImage != null && ResultSprites != null && ResultSprites.Length > resultLevel)
        {
            ResultImage.sprite = ResultSprites[resultLevel];
        }
    }

    public void OpenNextDayScene()
    {
        if (GameManager.Instance.DayCount != 15)
        {
            Debug.Log("[CleanDayUI] 위생 점검 종료, 밤을 건너뛰고 다음 날로 넘어갑니다.");

            //9일차로 바꿈
            GameManager.Instance.DayCount++;

            GameManager.Instance.Day = true;
            GameManager.Instance.Night = false;

            LoadingUIManager.Instance.LoadScene("DayEventScene");
        }
    }

    public void TriggerCleanEnding()
    {
        StartCoroutine(CleanEndingRoutine());
    }

    private IEnumerator CleanEndingRoutine()
    {
        // 1. 화면 어두워지기 (페이드 인)
        if (EndingFadeGroup != null)
        {
            EndingFadeGroup.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                EndingFadeGroup.alpha = Mathf.Lerp(0f, 1f, timer / 1.5f);
                yield return null;
            }
            EndingFadeGroup.alpha = 1f;
        }

        // 2. 까만 화면 위에서 대사 출력 (스페이스바 or 클릭 대기)
        if (EndingDialogueUI != null)
        {
            EndingDialogueUI.SetActive(true);

            foreach (string dialogue in CleanEndingDialogues)
            {
                if (EndingDialogueText != null) EndingDialogueText.text = dialogue;

                yield return null;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            }

            // 대사가 모두 끝나면 텍스트창 닫기
            EndingDialogueUI.SetActive(false);
        }

        // 3. 엔딩 일러스트 켜기
        if (CleanEnding != null) CleanEnding.SetActive(true);

        // 4. 화면 밝아지기 (페이드 아웃)
        if (EndingFadeGroup != null)
        {
            float timer = 0f;
            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                EndingFadeGroup.alpha = Mathf.Lerp(1f, 0f, timer / 1.5f);
                yield return null;
            }
            EndingFadeGroup.alpha = 0f;
            EndingFadeGroup.gameObject.SetActive(false);
        }
    }
}
