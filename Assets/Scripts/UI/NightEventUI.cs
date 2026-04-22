using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NightEventUI : MonoBehaviour
{
    private NightEvent NE;
    private GameManager GM;

    [Header("사운드")]
    public AudioClip UIClickSound;

    [Header("밤 종료 UI")]
    public GameObject NightFinUI;
    public TMP_Text FinDollCount;
    public TMP_Text FinMushCount;
    public TMP_Text FinHorseCount;

    [Header("사냥 타이머 UI")]
    public Slider NightTimerSlider;

    [Header("페이드 및 컷신 설정")]
    public CanvasGroup FadePanelGroup;
    public float FadeDuration = 1.0f;
    public GameObject NightCutSceneUI;

    public GameObject[] EndingCutSceneSprites_1st;

    private int CurrentCutScenePhase = 1;
    private int CurrentCutSceneIndex = 0;
    private bool isPlayingEndingCutScene = false;

    void Start()
    {
        NE = FindAnyObjectByType<NightEvent>();
        GM = FindAnyObjectByType<GameManager>();

        if (NE != null && NightTimerSlider != null)
        {
            NightTimerSlider.maxValue = NE.MaxNightTime;
            NightTimerSlider.value = NE.MaxNightTime;
        }

        if (GameManager.Instance.DayCount == 0)
        {

        }

        NightFinUI.SetActive(false);
        if (NightCutSceneUI != null) NightCutSceneUI.SetActive(false);
    }

    void Update()
    {
        if (NE != null && NightTimerSlider != null)
        {
            NightTimerSlider.value = NE.CurrentNightTime;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowNightResult();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (NightCutSceneUI != null && NightCutSceneUI.activeSelf && isPlayingEndingCutScene)
            {
                if (CurrentCutScenePhase == 1)
                {
                    if (CurrentCutSceneIndex < EndingCutSceneSprites_1st.Length)
                    {
                        if (EndingCutSceneSprites_1st[CurrentCutSceneIndex] != null)
                        {
                            EndingCutSceneSprites_1st[CurrentCutSceneIndex].SetActive(true);
                        }
                        CurrentCutSceneIndex++;
                    }
                    else
                    {
                        Debug.Log($"[NightEventUI] 밤 엔딩 컷신 종료, 낮 씬으로 이동합니다.");

                        isPlayingEndingCutScene = false;
                        //NightCutSceneUI.SetActive(false);
                        CurrentCutScenePhase = 1;
                        CurrentCutSceneIndex = 0;

                        OpenDayEventScene();
                    }
                }
            }
        }
    }

    public void ShowNightResult()
    {
        FinDollCount.text = $"{GameManager.Instance.DollCakeCount}";
        FinMushCount.text = $"{GameManager.Instance.MushroomMuffinCount}";
        FinHorseCount.text = $"{GameManager.Instance.SlimePuddingCount}";
        if (NightFinUI != null)
        {
            NightFinUI.SetActive(true);
        }
    }

    public void StartNightEndingCutScene()
    {
        Debug.Log("[NightEventUI] 밤 엔딩 컷신을 시작합니다.");
        isPlayingEndingCutScene = true;
        StartCoroutine(FadeAndShowCutsceneRoutine());
    }

    private IEnumerator FadeAndShowCutsceneRoutine()
    {
        // 1. 페이드 인 (화면 어두워짐)
        if (FadePanelGroup != null)
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
        }

        // 2. 컷신 초기화 및 켜기
        if (NightCutSceneUI != null)
        {
            NightCutSceneUI.SetActive(true);

            HideCutSceneImages(EndingCutSceneSprites_1st);

            CurrentCutScenePhase = 1;
            CurrentCutSceneIndex = 0;

            if (EndingCutSceneSprites_1st.Length > 0 && EndingCutSceneSprites_1st[0] != null)
            {
                EndingCutSceneSprites_1st[0].SetActive(true);
                CurrentCutSceneIndex = 1;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // 3. 페이드 아웃 (화면 다시 밝아지며 첫 번째 컷신 이미지 보여줌)
        if (FadePanelGroup != null)
        {
            float timer = 0f;
            while (timer < FadeDuration)
            {
                timer += Time.deltaTime;
                FadePanelGroup.alpha = Mathf.Lerp(1f, 0f, timer / FadeDuration);
                yield return null;
            }
            FadePanelGroup.alpha = 0f;
            FadePanelGroup.gameObject.SetActive(false);
        }
    }
    private void HideCutSceneImages(GameObject[] imageArray)
    {
        if (imageArray == null) return;
        foreach (GameObject img in imageArray)
        {
            if (img != null) img.SetActive(false);
        }
    }

    public void OpenDayEventScene()
    {
        if (GM == null) return;

        if (SoundManager.Instance != null && UIClickSound != null)
        {
            SoundManager.Instance.PlaySFX(UIClickSound);
        }

        GM.ChangeDayNight();

        if (NE != null && NE.isCreatureUnlockedToday && GM.DayCount != 1)
        {
            Debug.Log("새로운 크리쳐 해금으로 빵 제작씬으로 이동");
            //SceneManager.LoadScene("BakeEventScene");
            LoadingUIManager.Instance.LoadScene("BakeEventScene");

            SoundManager.Instance.PlayDayBGM();
        }
        else
        {
            //SceneManager.LoadScene("DayEventScene");
            LoadingUIManager.Instance.LoadScene("DayEventScene");

            SoundManager.Instance.PlayDayBGM();
        }
    }
}
