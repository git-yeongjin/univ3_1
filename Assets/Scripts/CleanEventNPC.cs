using TMPro;
using UnityEngine;
using UnityEngine.AI;
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

    [Header("대사UI")]
    public GameObject InspectorDialogueUI;
    public TMP_Text InspectorDialogueText;

    /*
    [Header("대사 목록")]
    public string IntroDialogue = "위생 점검 나왔습니다. 확인 좀 하겠습니다.";
    public string CheckingDialogue = "여긴 확인했고...";
    public string[] DirtyDialogues = { "이런 곳에서 빵을 만든다니...", "청소 상태가 불량하군요.", "먼지가 너무 많습니다." };
    public string CreatureDialogue = "아닛?! 이건 대체 무슨 흔적이죠?!";
    public string FinalDialogue = "음... 전반적인 위생 상태를 확인했습니다.";
    */

    private NavMeshAgent agent;
    private Animator anim;
    private bool isInspecting;

    private CleanEvent cleanEvent;
    private CleanDayUI cleanDayUI;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (InspectorDialogueUI != null) InspectorDialogueUI.SetActive(false);

        cleanEvent = FindAnyObjectByType<CleanEvent>();
        if (cleanEvent == null) Debug.LogError($"[CleanEventNPC] CleanEvent 스크립트를 찾을 수 없습니다.");
        cleanDayUI = FindAnyObjectByType<CleanDayUI>();
        if (cleanDayUI == null) Debug.LogError($"[CleanEventNPC] CleanEvent 스크립트를 찾을 수 없습니다.");
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
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < InspectionPath.Length; i++)
        {
            InspectionPoint currentPoint = InspectionPath[i];

            agent.SetDestination(currentPoint.PointTransform.position);
            HideDialogue();

            //목적기 도착 까지 대기
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f);

            //Inspect파라미터 있어야 됨
            if (anim != null) anim.SetTrigger("Inspect");

            ShowDialogue($"{i + 1}구역 확인 중...");
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
                    yield return new WaitForSeconds(2.0f);
                    break;
                case TraceState.NormalDirt:
                    //대사
                    yield return new WaitForSeconds(2.0f);
                    break;
                case TraceState.CreatureTrace:
                    //게임 오버 텍스트
                    yield return new WaitForSeconds(3.0f);

                    Debug.Log($"[CleanEventNPC] 크리쳐 흔적 발견 게임오버");
                    if (cleanEvent != null)
                    {
                        cleanEvent.CheckCreatureTrace(true);
                    }
                    InspectorDialogueUI.SetActive(false);
                    yield break;
            }
        }

        agent.SetDestination(FinalPoint.position);
        HideDialogue();
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f);

        if (anim != null) anim.SetTrigger("Think");
        ShowDialogue("검사 결과를 정리 중입니다...");
        yield return new WaitForSeconds(2.0f);

        ShowDialogue("완료 대사");
        yield return new WaitForSeconds(3.0f);
        HideDialogue();

        isInspecting = false;

        if (cleanEvent != null)
        {
            cleanEvent.CheckCreatureTrace(false);
            cleanEvent.CheckTimeOutCleanEvent();
        }
    }

    // 대사창 켜기 도우미 함수
    private void ShowDialogue(string text)
    {
        if (InspectorDialogueUI != null) InspectorDialogueUI.SetActive(true);
        if (InspectorDialogueText != null) InspectorDialogueText.text = text;
    }

    // 대사창 끄기 도우미 함수
    private void HideDialogue()
    {
        if (InspectorDialogueUI != null) InspectorDialogueUI.SetActive(false);
    }
}
