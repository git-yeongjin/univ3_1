using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public enum TraceState { Clean, NormalDirt, CreatureTrace }

[System.Serializable]
public class InspectionPoint
{
    public Transform PointTransform;
}

public class CleanEventNPC : MonoBehaviour
{
    [Header("이동 경로")]
    public InspectionPoint[] InspectionPath;
    public Transform FinalPoint;

    [Header("메인 대사 UI (시작/결과)")]
    public GameObject MainDialogueUI;   // 첫번째 캔버스
    public Image MainDialogueImage;     // 이미지를 바꿔치기 할 Image 컴포넌트
    public Sprite[] IntroSprites;       // 2개 (순차 재생)
    public Sprite[] ThinkingSprites;    // 3개 (순차 재생)
    public Sprite FinalSprite;          // 1개 (최종 발표)

    [Header("구역 확인 대사 UI (검사 중)")]
    public GameObject ZoneDialogueUI;   // 두번째 캔버스
    public Image ZoneDialogueImage;     // 이미지를 바꿔치기 할 Image 컴포넌트
    public Sprite[] CleanSprites;       // 4개 중 랜덤
    public Sprite[] DirtySprites;       // 2개 중 랜덤
    public Sprite CreatureTraceSprite;  // 1개 (게임오버)

    [Header("사운드")]
    public AudioClip SpawnSound;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isInspecting;

    private CleanEvent cleanEvent;
    private CleanDayUI cleanDayUI;
    private Camera mainCamera;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        mainCamera = Camera.main;

        HideAllDialogues();

        cleanEvent = FindAnyObjectByType<CleanEvent>();
        if (cleanEvent == null) Debug.LogError($"[CleanEventNPC] CleanEvent 스크립트를 찾을 수 없습니다.");
        cleanDayUI = FindAnyObjectByType<CleanDayUI>();
        if (cleanDayUI == null) Debug.LogError($"[CleanEventNPC] CleanEvent 스크립트를 찾을 수 없습니다.");

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // 메인 대사 UI가 켜져있다면 카메라 방향으로 회전
            if (MainDialogueUI != null && MainDialogueUI.activeSelf)
            {
                MainDialogueUI.transform.rotation = mainCamera.transform.rotation;
            }

            // 구역 확인 대사 UI가 켜져있다면 카메라 방향으로 회전
            if (ZoneDialogueUI != null && ZoneDialogueUI.activeSelf)
            {
                ZoneDialogueUI.transform.rotation = mainCamera.transform.rotation;
            }
        }
    }

    //120초 타이머 끝났을때 호출 할 함수
    public void AppearAndReady()
    {
        if (SoundManager.Instance != null && SpawnSound != null)
        {
            SoundManager.Instance.PlaySFX(SpawnSound);
        }

        gameObject.SetActive(true);
        Debug.Log("[CleanEventNPC] 위생검사원 등장. 마우스 클릭 대기 중");

        if (IntroSprites != null && IntroSprites.Length > 0)
        {
            ShowMainDialogue(IntroSprites[0]);
        }
    }

    public void StartInspection()
    {
        if (isInspecting) return;

        Debug.Log($"[CleanEventNPC] 곧 위생점검을 시작합니다.");

        if (cleanEvent != null)
        {
            cleanEvent.CleanDayEventFin = true;
        }

        StartCoroutine(InspectionRoutine());
    }

    private IEnumerator InspectionRoutine()
    {
        isInspecting = true;
        //플레이어 이동 막기

        //대사 넣기
        if (IntroSprites != null)
        {
            for (int i = 1; i < IntroSprites.Length; i++)
            {
                ShowMainDialogue(IntroSprites[i]);
                yield return new WaitForSeconds(2.0f);
            }
        }
        HideAllDialogues();
        yield return new WaitForSeconds(1.0f);
        //ShowMainDialogue(IntroSprite);
        //yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < InspectionPath.Length; i++)
        {
            InspectionPoint currentPoint = InspectionPath[i];

            agent.SetDestination(currentPoint.PointTransform.position);
            HideAllDialogues();

            //목적기 도착 까지 대기
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f);

            //Inspect파라미터 있어야 됨
            if (anim != null) anim.SetTrigger("Inspect");

            yield return new WaitForSeconds(3.0f);

            bool hasNormalDirt = false;
            bool hasCreatureTrace = false;

            Dirt[] dirtsHere = currentPoint.PointTransform.GetComponentsInChildren<Dirt>();
            foreach (Dirt dirt in dirtsHere)
            {
                if (dirt != null)
                {
                    if (dirt.isCreatureTrace)
                    {
                        hasCreatureTrace = true;
                    }
                    else
                    {
                        hasNormalDirt = true;
                    }
                }
            }

            TraceState currentState;
            if (hasCreatureTrace)
            {
                currentState = TraceState.CreatureTrace;
            }
            else if (hasNormalDirt)
            {
                currentState = TraceState.NormalDirt;
            }
            else
            {
                currentState = TraceState.Clean;
            }

            switch (currentState)
            {
                case TraceState.Clean:
                    //일반 대사
                    if (CleanSprites != null && CleanSprites.Length > 0)
                    {
                        Sprite randomClean = CleanSprites[Random.Range(0, CleanSprites.Length)];
                        ShowZoneDialogue(randomClean);
                    }
                    yield return new WaitForSeconds(2.0f);
                    break;
                case TraceState.NormalDirt:
                    if (DirtySprites.Length > 0)
                    {
                        Sprite randomDirty = DirtySprites[Random.Range(0, DirtySprites.Length)];
                        ShowZoneDialogue(randomDirty);
                    }
                    yield return new WaitForSeconds(2.0f);
                    break;
                case TraceState.CreatureTrace:
                    //게임 오버 텍스트
                    ShowZoneDialogue(CreatureTraceSprite);
                    yield return new WaitForSeconds(3.0f);

                    Debug.Log($"[CleanEventNPC] 크리쳐 흔적 발견 게임오버");
                    if (cleanEvent != null)
                    {
                        cleanEvent.CheckCreatureTrace(true);
                    }

                    HideAllDialogues();
                    yield break;
            }
        }

        //모든 검사 완료 후 최종 지점으로 이동
        agent.SetDestination(FinalPoint.position);
        HideAllDialogues();
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f);

        if (anim != null) anim.SetTrigger("Think");
        if (ThinkingSprites != null)
        {
            foreach (Sprite s in ThinkingSprites)
            {
                ShowMainDialogue(s);
                yield return new WaitForSeconds(2.0f);
            }
        }
        yield return new WaitForSeconds(2.0f);

        ShowMainDialogue(FinalSprite);
        yield return new WaitForSeconds(2.0f);
        HideAllDialogues();

        isInspecting = false;

        if (cleanEvent != null)
        {
            cleanEvent.CheckCreatureTrace(false);
            cleanEvent.CheckTimeOutCleanEvent();
        }
    }

    private void ShowMainDialogue(Sprite sprite)
    {
        HideAllDialogues();
        if (sprite == null) return;

        if (MainDialogueUI != null) MainDialogueUI.SetActive(true);
        if (MainDialogueImage != null) MainDialogueImage.sprite = sprite;
    }

    private void ShowZoneDialogue(Sprite sprite)
    {
        HideAllDialogues();
        if (sprite == null) return;

        if (ZoneDialogueUI != null) ZoneDialogueUI.SetActive(true);
        if (ZoneDialogueImage != null) ZoneDialogueImage.sprite = sprite;
    }

    private void HideAllDialogues()
    {
        if (MainDialogueUI != null) MainDialogueUI.SetActive(false);
        if (ZoneDialogueUI != null) ZoneDialogueUI.SetActive(false);
    }
}
