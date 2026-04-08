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
    public int CurrentRecipeSprite = 0;
    public RecipeDataBook recipeDataBook;

    [Header("UI 및 텍스트")]
    public GameObject BakeTutorialUI;
    public TMP_Text BakeTutorialText;

    public GameObject BakeFailUI;

    [Header("페이드 및 컷신 설정")]
    public CanvasGroup FadePanelGroup;
    public float FadeDuration = 1.0f;
    public GameObject BakeCutSceneUI;

    public GameObject[] CutSceneSprites_1st;
    public GameObject[] CutSceneSprites_2nd;
    public GameObject[] CutSceneSprites_3rd;

    private int CurrentCutScenePhase = 1;
    private int CurrentCutSceneIndex = 0;
    private int CurrentDialogueIndex = 0;
    [TextArea]
    public string[] TutorialDialogues =
    {
        "내일 오픈하기 전에 연습 해보자",
        "케이크면 괜찮겠지, 레시피를 확인해서 만들어 보자"
    };

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError($"GameManager를 찾을 수 없습니다.");

        if (RecipeImage != null && Sprites.Length > 0)
        {
            RecipeImage.sprite = Sprites[CurrentRecipeSprite];
        }

        if (GameManager.Instance.DayCount == 0)
        {
            BakeTutorialUI.SetActive(true);

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
        RecipeBookUI.SetActive(false);
        BakeFailUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BakeTutorialUI != null && BakeTutorialUI.activeSelf)
            {
                Debug.Log($"[BakeEventUI] 0일차 튜토리얼 이벤트 시작");

                if (BakeTutorialText == null) return;

                if (CurrentDialogueIndex < TutorialDialogues.Length)
                {
                    Debug.Log($"[BakeEventUI] 대사 출력중 {CurrentDialogueIndex} / {TutorialDialogues.Length}");

                    BakeTutorialText.text = TutorialDialogues[CurrentDialogueIndex];
                    CurrentDialogueIndex++;
                }
                else
                {
                    Debug.Log($"[BakeEventUI] 대사 종료");
                    BakeTutorialUI.SetActive(false);
                    CurrentDialogueIndex = 0;
                }
            }
            else if (BakeCutSceneUI != null && BakeCutSceneUI.activeSelf)
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
                        BakeCutSceneUI.SetActive(false);

                        CurrentCutScenePhase = 1;
                        CurrentCutSceneIndex = 0;
                        HideCutSceneImages(CutSceneSprites_3rd);

                        GM.ChangeDayNight();
                        SceneManager.LoadScene("NightEventScene");
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
            }
        }
    }

    public void OpenDayEventScene()
    {
        SceneManager.LoadScene("DayEventScene");
    }

    private IEnumerator HideBakeFailUI()
    {
        yield return new WaitForSeconds(3.0f);

        if (BakeFailUI != null) BakeFailUI.SetActive(false);
    }
}
