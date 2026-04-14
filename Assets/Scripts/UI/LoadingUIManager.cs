using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingUIManager : MonoBehaviour
{
    public static LoadingUIManager Instance;

    [Header("UI 구성 요소")]
    public GameObject loadingScreen; // 로딩 화면 전체를 담은 패널
    public CanvasGroup canvasGroup;
    public Slider progressBar;       // 진행도 슬라이더
    public TMP_Text progressText;

    [Header("페이드 설정")]
    public float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (loadingScreen != null) loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        //초기화
        loadingScreen.SetActive(true);
        progressBar.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);

        progressBar.value = 0f;
        progressText.text = "0%";
        canvasGroup.alpha = 0f;

        float timer = 0.0f;
        while (timer < fadeDuration)
        {
            // Time.timeScale의 영향을 받지 않도록 unscaledDeltaTime 사용 (게임 일시정지 중 씬 넘어감 방지)
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            yield return null;
        }
        canvasGroup.alpha = 1f;

        progressBar.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);

        //비동기 씬 로드 시작 (백그라운드에서 다음 씬을 불러옴)
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        //씬 로딩이 90% 완료되었을 때 바로 넘어가지 않게하기
        operation.allowSceneActivation = false;

        timer = 0.0f;

        //로딩 진행도 업데이트
        while (!operation.isDone)
        {
            yield return null;
            timer += Time.deltaTime;

            // 유니티의 비동기 로딩 진행도(operation.progress)는 0 ~ 0.9까지만 오릅니다.
            // 나머지 0.1은 씬을 활성화(allowSceneActivation = true)할 때 채워집니다.
            if (operation.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, operation.progress, timer);
                if (progressBar.value >= operation.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                // 씬 로드는 끝났지만, 로딩바가 부드럽게 100%까지 차오르게 연출
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);

                // 로딩바가 100%에 도달하면 씬 활성화
                if (progressBar.value >= 1.0f)
                {
                    progressText.text = "100%";
                    operation.allowSceneActivation = true;

                    //씬 전환 끝날 때까지 대기
                    yield return new WaitUntil(() => operation.isDone);
                    break;
                }
            }

            if (progressBar.value < 1.0f)
            {
                progressText.text = Mathf.RoundToInt(progressBar.value * 100f) + "%";
            }
        }

        progressBar.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        // 투명도 0으로 만들고 진짜로 화면 끄기
        canvasGroup.alpha = 0f;
        loadingScreen.SetActive(false);
    }
}
