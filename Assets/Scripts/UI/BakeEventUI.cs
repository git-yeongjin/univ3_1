using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BakeEventUI : MonoBehaviour
{
    private GameManager GM;
    private Coroutine HideBakeFailUICoroutine;

    [Header("레시피 북 설정")]
    public bool isOpenRecipeBook = false;
    public GameObject RecipeBookUI;
    public Sprite[] Sprites;
    public Image RecipeImage;
    public Image RecipeImage_Tuto;
    public int CurrentRecipeSprite = 0;
    public RecipeDataBook recipeDataBook;

    [Header("UI 및 텍스트")]
    public GameObject BakeTutorialUI;
    public TMP_Text BakeTutorialText;
    public GameObject BakeFailUI;

    [Header("조작법 UI")]
    public GameObject ControlsTutorialUI;
    private bool isShowingControls = false;

    [Header("버튼 설정")]
    public GameObject BakeFinishButton;


    [Header("페이드 및 컷신 설정")]
    public CanvasGroup FadePanelGroup;
    public float FadeDuration = 1.0f;
    public GameObject BakeCutSceneUI;

    public GameObject[] StartCutSceneSprites_1st;
    public GameObject[] StartCutSceneSprites_2nd;

    public GameObject[] CutSceneSprites_1st;
    public GameObject[] CutSceneSprites_2nd;
    public GameObject[] CutSceneSprites_3rd;

    private int CurrentCutScenePhase = 1;
    private int CurrentCutSceneIndex = 0;
    private int CurrentDialogueIndex = 0;

    private bool isPlayingStartCutScene = false;
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
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError($"GameManager를 찾을 수 없습니다.");

        if (RecipeImage != null && Sprites.Length > 0)
        {
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }

        RecipeBookUI.SetActive(false);
        BakeFailUI.SetActive(false);

        if (ControlsTutorialUI != null) ControlsTutorialUI.SetActive(false);
        if (BakeFinishButton != null) BakeFinishButton.SetActive(true);

        if (GameManager.Instance.DayCount == 0)
        {
            if (RecipeImage != null) RecipeImage.gameObject.SetActive(false);
            if (RecipeImage_Tuto != null) RecipeImage_Tuto.gameObject.SetActive(true);

            BakeTutorialUI.SetActive(false);
            isPlayingStartCutScene = true;
            StartCoroutine(FadeAndShowStartCutsceneRoutine());

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
        else
        {
            //튜토레시피 비활성화, 일반 레시피 활성화
            if (RecipeImage != null) RecipeImage.gameObject.SetActive(true);
            if (RecipeImage_Tuto != null) RecipeImage_Tuto.gameObject.SetActive(false);
        }
    }

    void Update()
    {

        if (isShowingControls)
        {
            if (Input.anyKeyDown)
            {
                CloseControlsAndStartDialogue();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpenRecipeBook) CloseRecipeBook();
            else OpenRecipeBook();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BakeTutorialUI != null && BakeTutorialUI.activeSelf)
            {
                Debug.Log($"[BakeEventUI] 0일차 튜토리얼 이벤트 시작");

                if (BakeTutorialText == null) return;

                int displayingIndex = CurrentDialogueIndex - 1;
                if (displayingIndex == 4 || displayingIndex == 6 || displayingIndex == 9)
                {
                    Debug.Log($"[튜토리얼 대기] 미션을 수행해야 다음 대사로 넘어갑니다. (현재 대사 번호: {displayingIndex})");
                    return;
                }

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
                        OpenCutSceneUI(); // 최종 컷신 시작
                    }
                    else
                    {
                        GM.ChangeDayNight();
                        LoadingUIManager.Instance.LoadScene("NightEventScene");
                    }
                }
            }

            else if (BakeCutSceneUI != null && BakeCutSceneUI.activeSelf)
            {
                if (isPlayingStartCutScene)
                {
                    if (CurrentCutScenePhase == 1)
                    {
                        if (CurrentCutSceneIndex < StartCutSceneSprites_1st.Length)
                        {
                            if (StartCutSceneSprites_1st[CurrentCutSceneIndex] != null)
                                StartCutSceneSprites_1st[CurrentCutSceneIndex].SetActive(true);
                            CurrentCutSceneIndex++;
                        }
                        else
                        {
                            CurrentCutScenePhase = 2;
                            CurrentCutSceneIndex = 0;
                            HideCutSceneImages(StartCutSceneSprites_1st);

                            if (StartCutSceneSprites_2nd.Length > 0 && StartCutSceneSprites_2nd[CurrentCutSceneIndex] != null)
                            {
                                StartCutSceneSprites_2nd[CurrentCutSceneIndex].SetActive(true);
                                CurrentCutSceneIndex = 1;
                            }
                        }
                    }
                    else if (CurrentCutScenePhase == 2)
                    {
                        if (CurrentCutSceneIndex < StartCutSceneSprites_2nd.Length)
                        {
                            if (StartCutSceneSprites_2nd[CurrentCutSceneIndex] != null)
                                StartCutSceneSprites_2nd[CurrentCutSceneIndex].SetActive(true);
                            CurrentCutSceneIndex++;
                        }
                        else
                        {
                            Debug.Log($"[BakeEventUI] 시작 컷신 종료, 조작법 UI활성화");
                            BakeCutSceneUI.SetActive(false);
                            isPlayingStartCutScene = false;
                            CurrentCutScenePhase = 1;
                            CurrentCutSceneIndex = 0;
                            HideCutSceneImages(StartCutSceneSprites_2nd);

                            if (ControlsTutorialUI != null)
                            {
                                ControlsTutorialUI.SetActive(true);
                                // 컷신 스페이스바 연타로 바로 닫히는 걸 막기 위해 보호 시간 적용
                                StartCoroutine(WaitAndEnableControlsUI());
                            }
                            else
                            {
                                // 조작법 UI가 안 들어있으면 바로 대사 시작
                                StartTutorialDialogue();
                            }
                        }
                    }
                }
                else
                {
                    if (CurrentCutScenePhase == 1)
                    {
                        if (CurrentCutSceneIndex < CutSceneSprites_1st.Length)
                        {
                            if (CutSceneSprites_1st[CurrentCutSceneIndex] != null)
                            {
                                CutSceneSprites_1st[CurrentCutSceneIndex].SetActive(true);
                            }
                            CurrentCutSceneIndex++;
                        }
                        else
                        {
                            CurrentCutScenePhase = 2;
                            CurrentCutSceneIndex = 0;
                            HideCutSceneImages(CutSceneSprites_1st);

                            if (CutSceneSprites_2nd.Length > 0 && CutSceneSprites_2nd[CurrentCutSceneIndex] != null)
                            {
                                CutSceneSprites_2nd[CurrentCutSceneIndex].SetActive(true);
                                CurrentCutSceneIndex = 1;
                            }
                        }
                    }
                    else if (CurrentCutScenePhase == 2)
                    {
                        if (CurrentCutSceneIndex < CutSceneSprites_2nd.Length)
                        {
                            if (CutSceneSprites_2nd[CurrentCutSceneIndex] != null)
                            {
                                CutSceneSprites_2nd[CurrentCutSceneIndex].SetActive(true);
                                CurrentCutSceneIndex++;
                            }
                        }
                        else
                        {
                            CurrentCutScenePhase = 3;
                            CurrentCutSceneIndex = 0;
                            HideCutSceneImages(CutSceneSprites_3rd);

                            if (CutSceneSprites_3rd.Length > 0 && CutSceneSprites_3rd[CurrentCutSceneIndex] != null)
                            {
                                CutSceneSprites_3rd[CurrentCutSceneIndex].SetActive(true);
                                CurrentCutSceneIndex = 1;
                            }
                        }
                    }
                    else if (CurrentCutScenePhase == 3)
                    {
                        if (CurrentCutSceneIndex < CutSceneSprites_3rd.Length)
                        {
                            if (CutSceneSprites_3rd[CurrentCutSceneIndex] != null)
                            {
                                CutSceneSprites_3rd[CurrentCutSceneIndex].SetActive(true);
                                CurrentCutSceneIndex++;
                            }
                        }
                        else
                        {
                            Debug.Log($"[BakeEventUI] 컷신이 종료 되었습니다.");
                            //BakeCutSceneUI.SetActive(false);

                            CurrentCutScenePhase = 1;
                            CurrentCutSceneIndex = 0;
                            //HideCutSceneImages(CutSceneSprites_3rd);

                            GM.ChangeDayNight();
                            //SceneManager.LoadScene("NightEventScene");
                            LoadingUIManager.Instance.LoadScene("NightEventScene");
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"[BakeEventUI] 오븐 컷신 종료");
                BakeCutSceneUI.SetActive(false);
                CurrentCutSceneIndex = 0;

                foreach (GameObject img in CutSceneSprites_1st)
                {
                    if (img != null) img.SetActive(false);
                }
            }
        }
    }

    private IEnumerator WaitAndEnableControlsUI()
    {
        //0.5초 대기 후 클릭가능
        yield return new WaitForSeconds(0.5f);
        isShowingControls = true;
    }

    private void CloseControlsAndStartDialogue()
    {
        isShowingControls = false;
        if (ControlsTutorialUI != null) ControlsTutorialUI.SetActive(false);
        StartTutorialDialogue();
    }

    private void StartTutorialDialogue()
    {
        BakeTutorialUI.SetActive(true);
        if (BakeTutorialText != null && TutorialDialogues.Length > 0)
        {
            BakeTutorialText.text = TutorialDialogues[0];
            CurrentDialogueIndex = 1;
        }
    }

    public void AdvanceTutorialDialogue()
    {
        if (BakeTutorialUI != null && CurrentDialogueIndex < TutorialDialogues.Length)
        {
            BakeTutorialText.text = TutorialDialogues[CurrentDialogueIndex];
            CurrentDialogueIndex++;
            Debug.Log($"[튜토리얼 진행] 미션 성공! 다음 대사로 넘어갑니다.");
        }
    }

    private IEnumerator FadeAndShowStartCutsceneRoutine()
    {
        //FadePanelGroup.gameObject.SetActive(true);

        // 시작 컷신 세팅
        if (BakeCutSceneUI != null)
        {
            BakeCutSceneUI.SetActive(true);

            HideCutSceneImages(StartCutSceneSprites_1st);
            HideCutSceneImages(StartCutSceneSprites_2nd);
            HideCutSceneImages(CutSceneSprites_1st); // 혹시 켜져있을 기존 컷신도 숨김

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

    public void OpenCutSceneUI()
    {
        Debug.Log($"[BakeEventUI] 컷신이 실행 됩니다.");

        StartCoroutine(FadeAndShowCutsceneRoutine());
    }

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

    private void HideCutSceneImages(GameObject[] imageArray)
    {
        if (imageArray == null) return;
        foreach (GameObject img in imageArray)
        {
            if (img != null) img.SetActive(false);
        }
    }

    public void OpenRecipeBook()
    {
        if (!isOpenRecipeBook)
        {
            RecipeBookUI.SetActive(true);
            isOpenRecipeBook = true;

            //레시피 북 열기 확인
            if (GameManager.Instance.DayCount == 0 && (CurrentDialogueIndex - 1) == 4)
                AdvanceTutorialDialogue();
        }
    }

    public void CloseRecipeBook()
    {
        if (isOpenRecipeBook)
        {
            RecipeBookUI.SetActive(false);
            isOpenRecipeBook = false;
        }
    }

    public void OnClickRight()
    {
        if (GameManager.Instance.DayCount == 0) return;

        if (CurrentRecipeSprite < Sprites.Length - 1)
        {
            CurrentRecipeSprite++;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
        else
        {
            CurrentRecipeSprite = 0;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
    }

    public void OnClickLeft()
    {
        if (GameManager.Instance.DayCount == 0) return;

        if (CurrentRecipeSprite > 0)
        {
            CurrentRecipeSprite--;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
        else
        {
            CurrentRecipeSprite = Sprites.Length - 1;
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }
    }

    public void OnClickBakeFinish()
    {
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
                GM.isBakingTime = false;

                if (BakeFinishButton != null) BakeFinishButton.SetActive(false);

                //반죽 섞기 확인
                if (GameManager.Instance.DayCount == 0 && (CurrentDialogueIndex - 1) == 6)
                    AdvanceTutorialDialogue();
            }
        }
    }

    //오븐에 넣기 확인
    public void OnOvenBakeFinishedInTutorial()
    {
        if (GameManager.Instance.DayCount == 0 && (CurrentDialogueIndex - 1) == 9)
            AdvanceTutorialDialogue();
    }

    public void OpenDayEventScene()
    {
        //SceneManager.LoadScene("DayEventScene");
        LoadingUIManager.Instance.LoadScene("DayEventScene");
    }

    private IEnumerator HideBakeFailUI()
    {
        yield return new WaitForSeconds(3.0f);

        if (BakeFailUI != null) BakeFailUI.SetActive(false);
    }
}
