using UnityEngine;

public class Creature_Mammoth : MonoBehaviour
{
    private Creature BaseCreature;
    private Transform PlayerTransform;

    public enum MammothState { Patrol, Warning, Prep, Dashing, FakeStop, Stunned, Stuck, Defeated }
    [Header("현재 상태")]
    public MammothState CurrentState = MammothState.Patrol;

    [Header("기본 수치 및 거리")]
    //플레이어 인식 범위
    public float DetectRadius = 15.0f;
    //배회 속도
    public float PatrolSpeed = 2.0f;
    //기본 돌진 속도
    public float BaseDashSpeed = 15.0f;
    private float CurrentDashSpeed;

    [Header("시간 설정")]
    //경고 시간
    public float WarningDuration = 2.0f;
    //돌진 준비 시간
    public float PrepDuration = 2.0f;
    public float FakeStopDuration = 1.0f;
    //기절 시간
    public float StunDuration = 3.0f;
    //뿔 충돌 시간
    public float StuckDuration = 6.0f;

    [Header("돌진 및 충돌 페이즈 관리")]
    public int CrashCount = 0;
    public int MaxDashCount = 2;
    private int CurrentDashAttempt = 0;
    private Vector3 DashDirection;

    [Header("시각 효과 및 예고선")]
    public LineRenderer WarningLine;
    public GameObject DustEffect;
    public GameObject BreathEffect;

    public Renderer HornRenderer;
    public Material[] HornDamageMaterials;

    private float StateTimer = 0f;

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;

        CurrentDashSpeed = BaseDashSpeed;

        if (WarningLine != null) WarningLine.enabled = false;
    }

    void Update()
    {
        if (PlayerTransform == null || CurrentState == MammothState.Defeated) return;

        StateTimer += Time.deltaTime;

        switch (CurrentState)
        {
            case MammothState.Patrol:
                transform.Rotate(0, 10f * Time.deltaTime, 0);

                if (Vector3.Distance(transform.position, PlayerTransform.position) <= DetectRadius)
                {
                    Debug.Log($"[맘모스] 플레이어 발견");
                    ChangeState(MammothState.Warning);
                }
                break;
            case MammothState.Warning:
                if (StateTimer >= WarningDuration)
                {
                    ChangeState(MammothState.Prep);
                }
                break;
            case MammothState.Prep:
                AimDash();
                if (StateTimer >= PrepDuration)
                {
                    ChangeState(MammothState.Dashing);
                }
                break;
            case MammothState.Dashing:
                transform.position += DashDirection * CurrentDashSpeed * Time.deltaTime;
                break;
            case MammothState.FakeStop:
                if (StateTimer <= FakeStopDuration)
                {
                    ChangeState(MammothState.Prep);
                }
                break;
            case MammothState.Stunned:
                if (StateTimer >= StunDuration) EndDisableState();
                break;
            case MammothState.Stuck:
                if (StateTimer >= StuckDuration) EndDisableState();
                break;
        }
    }

    private void ChangeState(MammothState state)
    {
        CurrentState = state;
        StateTimer = 0f;

        if (WarningLine != null) WarningLine.enabled = (state == MammothState.Prep);
        if (DustEffect != null) DustEffect.SetActive(state == MammothState.Prep);
        if (BreathEffect != null) BreathEffect.SetActive(state == MammothState.Prep);

        if (state == MammothState.Dashing)
        {
            CurrentDashAttempt++;
            Debug.Log($"[맘모스] 돌진 공격 시작");
        }
    }

    private void AimDash()
    {
        Vector3 dirToPlayer = (PlayerTransform.position - transform.position).normalized;
        dirToPlayer.y = 0;
        DashDirection = dirToPlayer;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(DashDirection), Time.deltaTime * 5f);

        if (WarningLine != null)
        {
            WarningLine.SetPosition(0, transform.position);
            WarningLine.SetPosition(1, transform.position + (DashDirection * 30f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == MammothState.Dashing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            //플레이어 기절 함수

            HandleDashEnd();
        }
        //벽이나 장애물
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            HandleWallCrash();
        }
    }

    private void HandleDashEnd()
    {
        if (CurrentDashAttempt < MaxDashCount)
        {
            Debug.Log($"[맘모스] 2차 돌진 준비");
            ChangeState(MammothState.FakeStop);
        }
        else
        {
            CurrentDashAttempt = 0;
            ChangeState(MammothState.Warning);
        }
    }

    private void HandleWallCrash()
    {
        CrashCount++;
        CurrentDashAttempt = 0;
        //뿔 효과 넣을 곳

        Debug.Log($"[맘모스] 누적 충돌 ({CrashCount}회)");

        switch (CrashCount)
        {
            case 1:
                if (Random.value > 0.5f) ChangeState(MammothState.Stunned);
                else ChangeState(MammothState.Stuck);
                break;
            case 2:
                Debug.Log($"[맘모스] 돌진 속도 증가");
                CurrentDashSpeed = BaseDashSpeed * 1.5f;
                ChangeState(MammothState.Stunned);
                break;
            case 3:
                Debug.Log($"[맘모스] 돌진 횟수 증가");
                MaxDashCount = 3;
                ChangeState(MammothState.Stunned);
                break;
            case 4:
                Debug.Log($"[맘모스] 중심을 잃고 자가 전복");
                ChangeState(MammothState.Defeated);
                break;
        }
    }

    private void UpdateHornVisual()
    {

    }

    private void EndDisableState()
    {
        ChangeState(MammothState.Warning);
    }

    private void CameraShake()
    {
        //카메라 흔들림 연출
    }
}
