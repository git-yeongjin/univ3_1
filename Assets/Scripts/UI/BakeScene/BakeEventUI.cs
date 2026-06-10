using System.Collections;
using UnityEngine;

public class BakeEventUI : MonoBehaviour
{
    private Coroutine HideBakeFailUICoroutine;

    [Header("UI 및 텍스트")]
    public GameObject BakeFailUI;
    public GameObject OpenUI;
    public GameObject BakeFinishButton;

    [Header("사운드 효과음")]
    public AudioClip UIClickSound;

    [Header("영상 팝업 설정")]
    public GameObject BakeVideoPopupUI;
    public bool isVideoPlaying = false;

    void Start()
    {
        if (OpenUI != null) OpenUI.SetActive(false);
        //반죽 섞기 실패시 UI끄기
        if (BakeFailUI != null) BakeFailUI.SetActive(false);
        //반죽완료 버튼 끄기
        if (BakeFinishButton != null) BakeFinishButton.SetActive(false);
        if (BakeVideoPopupUI != null) BakeVideoPopupUI.SetActive(false);

        GameManager.Instance.isBakingTime = true;

        //0일차 튜토리얼 시작
        if (GameManager.Instance.DayCount == 0)
        {
            Debug.Log("[BakeEventUI] 튜토리얼 영상을 실행 합니다.");
            if (BakeVideoPopupUI != null) BakeVideoPopupUI.SetActive(true);
            isVideoPlaying = true;
        }
        else
        {
            isVideoPlaying = false;
        }
    }

    void Update()
    {
        if (isVideoPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (BakeVideoPopupUI != null) BakeVideoPopupUI.SetActive(false);
            isVideoPlaying = false;

            Debug.Log("[BakeEventUI] 영상 종료 조작이 활성화됩니다.");
        }

        if (BakeFinishButton != null && BakeFinishButton.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BakeFinish();
            }
        }

        if (OpenUI != null && OpenUI.activeSelf && GameManager.Instance.DayCount >= 1 && !isVideoPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OpenDayEventScene();
            }
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
    public void BakeFinish()
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

                DragDrop dragDrop = FindAnyObjectByType<DragDrop>();
                if (dragDrop != null)
                {
                    dragDrop.ActiveBreadMaterials();
                }
            }
            else
            {
                Debug.Log($"반죽을 섞었습니다. 현재 레시피 : {currentDough.recipe.BreadName}");

                currentDough.DoughBall.SetActive(true);
                if (BakeFinishButton != null) BakeFinishButton.SetActive(false);

                GameManager.Instance.isBakingTime = false;
            }
        }
    }

    /// <summary>
    /// Oven스크립트에서 호출할 함수
    /// </summary>
    public void ShowOpenUI(bool isShow)
    {
        OpenUI.SetActive(isShow);
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

    public void OpenNightEventScene()
    {
        if (SoundManager.Instance != null && UIClickSound != null)
        {
            SoundManager.Instance.PlaySFX(UIClickSound);
        }
        LoadingUIManager.Instance.LoadScene("NightEventScene");
    }
}
