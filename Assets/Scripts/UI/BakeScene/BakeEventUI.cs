using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BakeEventUI : MonoBehaviour
{
    private Coroutine HideBakeFailUICoroutine;

    [Header("UI 및 텍스트")]
    public GameObject BakeTutorialUI;
    public TMP_Text BakeTutorialText;
    public GameObject BakeFailUI;

    [Header("조작법 UI")]
    public GameObject ControlsTutorialUI;
    private bool isShowingControls = false;

    [Header("버튼 설정")]
    public GameObject BakeFinishButton;

    [Header("사운드 효과음")]
    public AudioClip UIClickSound;


    [Header("페이드 및 컷신 설정")]
    public CanvasGroup FadePanelGroup;
    public float FadeDuration = 1.0f;

    //아래 스프라이트들에 부조 오브젝트
    public GameObject BakeCutSceneUI;

    //BakeCutSceneUI 자식들
    //맨처음 컷신들
    public GameObject[] StartCutSceneSprites_1st;
    public GameObject[] StartCutSceneSprites_2nd;
    //진열 튜토리얼까지 완료하면 나오는 컷신들
    public GameObject[] CutSceneSprites_1st;
    public GameObject[] CutSceneSprites_2nd;
    public GameObject[] CutSceneSprites_3rd;

    private int CurrentCutScenePhase = 1;
    private int CurrentCutSceneIndex = 0;
    private int CurrentDialogueIndex = 0;
    private bool isPlayingStartCutScene = false;

    private readonly int MISSION_OPEN_BOOK = 4;
    private readonly int MISSION_MIX_DOUGH = 6;
    private readonly int MISSION_OVEN_BAKE = 9;

    //미션을 미리 깻는지 확인하는 변수
    private bool isBookOpened = false;
    private bool isDoughMixed = false;
    private bool isOvenBaked = false;

    [TextArea]
    public string[] TutorialDialogues =
    {
        "내일은 드디어 가게 오픈 날!",
        "오랫동안 기대했던 날이라 두근거려",
        "오픈하기 전에 빵 만드는 걸 연습해볼까…",
        "무슨 빵을 만들지?",
        "레시피북을 한번 확인해보자", //4

        "오늘은 롤케이크를 만들어야지",
        "레시피와 맞는 재료를 반죽에 넣어보자", //6

        "반죽 완성!",
        "이제 오븐에 구워야겠어",
        "오븐으로 가자", //9

        "롤케이크 완성!"
    };

    void Start()
    {
        //반죽 섞기 실패시 UI끄기
        if (BakeFailUI != null) BakeFailUI.SetActive(false);

        //조작법UI && 반죽완료 버튼 끄기
        if (ControlsTutorialUI != null) ControlsTutorialUI.SetActive(false);
        if (BakeFinishButton != null) BakeFinishButton.SetActive(false);

        //0일차 튜토리얼 시작
        if (GameManager.Instance.DayCount == 0)
        {
            //플레이어 텍스트창
            BakeTutorialUI.SetActive(false);

            //StartCutSceneSprites나오게 하는거
            isPlayingStartCutScene = true;
            StartCoroutine(FadeAndShowStartCutsceneRoutine());

            //대사 시작
            if (BakeTutorialText != null && TutorialDialogues.Length > 0)
            {
                BakeTutorialText.text = TutorialDialogues[0];
                CurrentDialogueIndex = 1;
            }
            else
            {
                BakeTutorialUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        //조작법UI 켜져 있을 때
        if (isShowingControls)
        {
            if (Input.anyKeyDown)
            {
                CloseControlsAndStartDialogue();
            }
            return;
        }

        //스페이스바 대사 진행 및 컷신
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //대사 창이 켜져 있을 때
            if (BakeTutorialUI != null && BakeTutorialUI.activeSelf)
            {
                Debug.Log($"[BakeEventUI] 0일차 튜토리얼 이벤트 시작");
                if (BakeTutorialText == null) return;

                int displayingIndex = CurrentDialogueIndex - 1;

                if (displayingIndex == MISSION_OPEN_BOOK && !isBookOpened)
                {
                    Debug.Log($"[튜토리얼 대기] 미션을 수행해야 다음 대사로 넘어갑니다. (현재 대사 번호: {displayingIndex})");
                    return;
                }
                if (displayingIndex == MISSION_MIX_DOUGH && !isDoughMixed)
                {
                    Debug.Log($"[튜토리얼 대기] 반죽을 섞어야 다음 대사로 넘어갑니다.");
                    return;
                }
                if (displayingIndex == MISSION_OVEN_BAKE && !isOvenBaked)
                {
                    Debug.Log($"[튜토리얼 대기] 오븐을 구워야 다음 대사로 넘어갑니다.");
                    return;
                }

                //다음 대사 출력
                if (CurrentDialogueIndex < TutorialDialogues.Length)
                {
                    Debug.Log($"[BakeEventUI] 대사 출력중 {CurrentDialogueIndex} / {TutorialDialogues.Length}");

                    BakeTutorialText.text = TutorialDialogues[CurrentDialogueIndex];
                    CurrentDialogueIndex++;
                }
                else
                {
                    Debug.Log($"[BakeEventUI] 대사 종료 밤 씬으로 이동합니다");
                    BakeTutorialUI.SetActive(false);
                    CurrentDialogueIndex = 0;

                    if (GameManager.Instance.DayCount == 0)
                    {
                        //최종 컷신 시작
                        OpenCutSceneUI();
                    }
                    else
                    {
                        GameManager.Instance.ChangeDayNight();
                        LoadingUIManager.Instance.LoadScene("NightEventScene");
                    }
                }
            }
            else if (BakeCutSceneUI != null && BakeCutSceneUI.activeSelf)
            {
                if (isPlayingStartCutScene)
                {
                    if (CurrentCutScenePhase == 1) AdvanceCutScene(StartCutSceneSprites_1st, StartCutSceneSprites_2nd, null);
                    else if (CurrentCutScenePhase == 2) AdvanceCutScene(StartCutSceneSprites_2nd, null, EndStartCutScene);
                }
                else
                {
                    if (CurrentCutScenePhase == 1) AdvanceCutScene(CutSceneSprites_1st, CutSceneSprites_2nd, null);
                    else if (CurrentCutScenePhase == 2) AdvanceCutScene(CutSceneSprites_2nd, CutSceneSprites_3rd, null);
                    else if (CurrentCutScenePhase == 3) AdvanceCutScene(CutSceneSprites_3rd, null, EndFinalCutScene);
                }
            }
            else
            {
                Debug.Log($"[BakeEventUI] 오븐 컷신 종료");
                BakeCutSceneUI.SetActive(false);
                CurrentCutSceneIndex = 0;
                HideCutSceneImages(CutSceneSprites_1st);
            }
        }
    }

    /// <summary>
    /// 컷신 배열을 받아서 이미지를 켜주고 배열이 끝나면 다음 페이즈로 넘기는 함수
    /// </summary>
    /// <param name="currentSprites">현재 재생 중인 이미지 배열</param>
    /// <param name="nextSprites">다음에 재생할 이지미 배열 없으면 null</param>
    /// <param name="onCutsceneEnd">컷신이 완전히 끝났을 때 실행할 함수</param>
    private void AdvanceCutScene(GameObject[] currentSprites, GameObject[] nextSprites, Action onCutsceneEnd)
    {
        //현재 컷신모음에서 남은 컷신이 있으면 CurrentCutSceneIndex번째 이미지 보이게 하기
        if (CurrentCutSceneIndex < currentSprites.Length)
        {
            if (currentSprites[CurrentCutSceneIndex] != null)
                currentSprites[CurrentCutSceneIndex].SetActive(true);
            CurrentCutSceneIndex++;
        }
        //남은 컷신없으면 다음 컷신모음으로 넘기기
        else
        {
            CurrentCutScenePhase++;
            CurrentCutSceneIndex = 0;
            HideCutSceneImages(currentSprites);

            if (nextSprites != null && nextSprites.Length > 0)
            {
                if (nextSprites[0] != null)
                    nextSprites[0].SetActive(true);
                CurrentCutSceneIndex = 1;
            }
            else
            {
                onCutsceneEnd?.Invoke();
            }
        }
    }

    /// <summary>
    /// 튜토리얼 시작 컷신이 모두 끝났을 때 호출되는 함수
    /// 조작법 UI띄우기
    /// </summary>
    private void EndStartCutScene()
    {
        Debug.Log($"[BakeEventUI] 시작 컷신 종료, 조작법 UI활성화");
        BakeCutSceneUI.SetActive(false);
        isPlayingStartCutScene = false;
        CurrentCutScenePhase = 1;
        CurrentCutSceneIndex = 0;

        if (ControlsTutorialUI != null)
        {
            ControlsTutorialUI.SetActive(true);
            StartCoroutine(WaitAndEnableControlsUI());
        }
        else
        {
            StartTutorialDialogue();
        }
    }

    /// <summary>
    /// 빵 굽기(오븐) 이후 나오는 최종 컷신이 모두 끝났을 때 호출되는 함수
    /// 씬을 밤으로 넘깁니다.
    /// </summary>
    private void EndFinalCutScene()
    {
        Debug.Log($"[BakeEventUI] 컷신이 종료 되었습니다.");
        CurrentCutScenePhase = 1;
        CurrentCutSceneIndex = 0;

        GameManager.Instance.ChangeDayNight();
        LoadingUIManager.Instance.LoadScene("NightEventScene");
    }

    /// <summary>
    /// 조작법 UI가 켜질 때, 스페이스바 연타로 인해 바로 닫히는 것을 막기 위한 보호 쿨타임(0.5초)
    /// </summary>
    private IEnumerator WaitAndEnableControlsUI()
    {
        //0.5초 대기 후 클릭가능
        yield return new WaitForSeconds(0.5f);
        isShowingControls = true;
    }

    /// <summary>
    /// 조작법 UI를 닫고 본격적인 플레이어 튜토리얼 대사를 시작하는 함수
    /// </summary>
    private void CloseControlsAndStartDialogue()
    {
        isShowingControls = false;
        if (ControlsTutorialUI != null) ControlsTutorialUI.SetActive(false);
        StartTutorialDialogue();
    }

    /// <summary>
    /// 튜토리얼 대사창을 켜고 첫 번째 대사를 출력하는 함수
    /// </summary>
    private void StartTutorialDialogue()
    {
        BakeTutorialUI.SetActive(true);
        if (BakeTutorialText != null && TutorialDialogues.Length > 0)
        {
            BakeTutorialText.text = TutorialDialogues[0];
            CurrentDialogueIndex = 1;
        }
    }

    /// <summary>
    /// 씬 시작 시 오프닝 컷신을 세팅하고 0.5초 대기하는 코루틴
    /// </summary>
    private IEnumerator FadeAndShowStartCutsceneRoutine()
    {
        // 시작 컷신 세팅
        if (BakeCutSceneUI != null)
        {
            BakeCutSceneUI.SetActive(true);

            HideCutSceneImages(StartCutSceneSprites_1st);
            HideCutSceneImages(StartCutSceneSprites_2nd);
            //혹시 켜져있을 기존 컷신도 숨김
            HideCutSceneImages(CutSceneSprites_1st);

            CurrentCutScenePhase = 1;
            CurrentCutSceneIndex = 0;

            if (StartCutSceneSprites_1st.Length > 0 && StartCutSceneSprites_1st[0] != null)
            {
                StartCutSceneSprites_1st[0].SetActive(true);
                CurrentCutSceneIndex = 1;
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 화면을 페이드 인(검게 변함) 한 뒤 최종 컷신을 세팅하고, 다시 페이드 아웃(밝아짐) 하는 연출 코루틴
    /// </summary>
    private IEnumerator FadeAndShowCutsceneRoutine()
    {
        FadePanelGroup.gameObject.SetActive(true);

        float timer = 0f;

        while (timer < FadeDuration)
        {
            timer += Time.deltaTime;

            FadePanelGroup.alpha = Mathf.Lerp(0f, 1f, timer / FadeDuration);
            yield return null;
        }

        FadePanelGroup.alpha = 1f;

        //컷신 세팅하기
        if (BakeCutSceneUI != null)
        {
            BakeCutSceneUI.SetActive(true);

            HideCutSceneImages(CutSceneSprites_1st);
            HideCutSceneImages(CutSceneSprites_2nd);
            HideCutSceneImages(CutSceneSprites_3rd);

            CurrentCutScenePhase = 1;
            CurrentCutSceneIndex = 0;

            if (CutSceneSprites_1st.Length > 0 && CutSceneSprites_1st[0] != null)
            {
                CutSceneSprites_1st[0].SetActive(true);
                CurrentCutSceneIndex = 1;
            }
        }

        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        while (timer < FadeDuration)
        {
            timer += Time.deltaTime;
            FadePanelGroup.alpha = Mathf.Lerp(1f, 0f, timer / FadeDuration);
            yield return null;
        }
        FadePanelGroup.alpha = 0f;

        FadePanelGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// 전달받은 이미지 배열의 모든 게임 오브젝트를 한 번에 비활성화 시키는 함수
    /// </summary>
    private void HideCutSceneImages(GameObject[] imageArray)
    {
        if (imageArray == null) return;
        foreach (GameObject img in imageArray)
        {
            if (img != null) img.SetActive(false);
        }
    }

    /// <summary>
    /// 빵 굽기 실패 UI를 띄우고 3초 뒤에 다시 꺼주는 코루틴
    /// </summary>
    private IEnumerator HideBakeFailUI()
    {
        yield return new WaitForSeconds(3.0f);

        if (BakeFailUI != null) BakeFailUI.SetActive(false);
    }


    /// <summary>
    /// '반죽 완료' 버튼을 눌렀을 때 실행되는 함수. 올바른 레시피인지 검사
    /// </summary>
    public void OnClickBakeFinish()
    {
        if (SoundManager.Instance != null && UIClickSound != null)
        {
            SoundManager.Instance.PlaySFX(UIClickSound);
        }

        Dough currentDough = FindAnyObjectByType<Dough>();
        if (currentDough != null)
        {
            currentDough.FindRecipe();
            if (currentDough.recipe == null)
            {
                Debug.Log("[BakeEventUI] 재료가 맞지 않아 반죽을 섞지 못했습니다, 반죽을 초기화 합니다.");
                currentDough.ClearBreadMaterial();
                BakeFailUI.SetActive(true);
                if (HideBakeFailUICoroutine != null) StopCoroutine(HideBakeFailUICoroutine);

                HideBakeFailUICoroutine = StartCoroutine(HideBakeFailUI());
            }
            else
            {
                Debug.Log($"반죽을 섞었습니다. 현재 레시피 : {currentDough.recipe.BreadName}");
                GameManager.Instance.isBakingTime = false;

                if (BakeFinishButton != null) BakeFinishButton.SetActive(false);

                //반죽 섞기 확인
                if (GameManager.Instance.DayCount == 0 && (CurrentDialogueIndex - 1) == 6)
                    AdvanceTutorialDialogue();
            }
        }
    }

    /// <summary>
    /// 빵 굽기가 완료된 후 최종 컷신을 켜라고 지시하는 퍼블릭 함수
    /// </summary>
    public void OpenCutSceneUI()
    {
        Debug.Log($"[BakeEventUI] 컷신이 실행 됩니다.");
        StartCoroutine(FadeAndShowCutsceneRoutine());
    }

    /// <summary>
    /// 외부 스크립트에서 오븐 굽기 타이머가 끝났을 때 호출하여 튜토리얼(9번 대사)을 진행시키는 함수
    /// </summary>
    public void OnOvenBakeFinishedInTutorial()
    {
        CheckTutorialMission(MISSION_OVEN_BAKE);
    }

    /// <summary>
    /// 낮 씬(매장 장사)으로 넘어가는 버튼용 함수
    /// </summary>
    public void OpenDayEventScene()
    {
        if (SoundManager.Instance != null && UIClickSound != null)
        {
            SoundManager.Instance.PlaySFX(UIClickSound);
        }
        LoadingUIManager.Instance.LoadScene("DayEventScene");
    }

    /// <summary>
    /// 튜토리얼 중 유저가 특정 미션(행동)을 완수했을 때, 다음 대사로 넘어가도록 강제 호출하는 함수
    /// </summary>
    public void AdvanceTutorialDialogue()
    {
        if (BakeTutorialUI != null && CurrentDialogueIndex < TutorialDialogues.Length)
        {
            BakeTutorialText.text = TutorialDialogues[CurrentDialogueIndex];
            CurrentDialogueIndex++;
            Debug.Log($"[튜토리얼 진행] 미션 성공! 다음 대사로 넘어갑니다.");
        }
    }

    /// <summary>
    /// 외부 스크립트에서 현재 플레이어가 해당 미션을 깰 차례인지 확인하고 대사를 넘겨주는 함수
    /// </summary>
    /// <param name="targetIndex">완수해야 할 목표 대사 인덱스 번호</param>
    public void CheckTutorialMission(int targetIndex)
    {
        if (targetIndex == MISSION_OPEN_BOOK) isBookOpened = true;
        if (targetIndex == MISSION_MIX_DOUGH) isDoughMixed = true;
        if (targetIndex == MISSION_OVEN_BAKE) isOvenBaked = true;

        if (GameManager.Instance.DayCount == 0 && (CurrentDialogueIndex - 1) == targetIndex)
        {
            AdvanceTutorialDialogue();
        }
    }
}
