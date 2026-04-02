using UnityEngine;

public class Creature_SlimeHorse : MonoBehaviour
{
    private Creature BaseCreature;
    private Transform PlayerTransform;

    public enum SlimeHorseState { Idle, Explore, Stare, Capturable, Alert, Attack, Down, Retreat }
    [Header("현재 상태")]
    public SlimeHorseState CurrentState = SlimeHorseState.Idle;

    [Header("이동 및 거리")]
    public float DetectRadius = 15.0f;
    //원형 이동 속도
    public float CircleSpeed = 50.0f;
    //플레이어 주변을 도는 거리
    public float DistanceFromPlayer = 5.0f;
    //포획 실패시 도망가는 거리
    public float RetreatDistance = 10.0f;
    public float RetreatSpeed = 15.0f;
    public float TurnSpeed = 10.0f;

    private Vector3 TargetRetreatPosition;

    [Header("전투 및 공격 설정")]
    //최대 공격 횟수 -> 이후 다운
    public int MaxAttackCount = 5;
    //조준 시간
    public float AimTime = 1.0f;
    //다음 공격 쿨타임
    public float AttackCooldown = 2.0f;
    //다운 상태 유지 시간
    public float DownDuration = 10.0f;
    //슬라임 투사체 프리팹
    public GameObject SlimeProjectilePrefab;
    //발사 위치
    public Transform FirePoint;

    [Header("오카리나 포획설정")]
    //오카리나 반응 확률
    [Range(0, 100)] public int StareChance = 70;
    //포획 시도 횟수
    private int CaptureAttemptCount = 0;

    [Header("이펙트")]
    public GameObject HeartEffect;
    public GameObject BurstEffect;
    public Transform CheekTransform;
    private Vector3 OriginalCheekScale;

    private float StateTimer = 0f;
    private int CurrentAttackCount = 0;

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;

        if (HeartEffect != null) HeartEffect.SetActive(false);
        if (CheekTransform != null) OriginalCheekScale = CheekTransform.localScale;
    }

    void Update()
    {
        if (PlayerTransform == null) return;

        switch (CurrentState)
        {
            case SlimeHorseState.Idle:
                float distToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
                if (distToPlayer <= DetectRadius)
                {
                    Debug.Log($"[슬라임 말] 플레이어 인식");
                    ChangeState(SlimeHorseState.Explore);
                }
                break;
            case SlimeHorseState.Explore:
                Vector3 beforePos = transform.position;

                //플레이어 주변을 원형으로 달림
                transform.RotateAround(PlayerTransform.position, Vector3.up, CircleSpeed * Time.deltaTime);

                Vector3 moveDir = (transform.position - beforePos).normalized;
                moveDir.y = 0;

                if (moveDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);
                }

                break;

            case SlimeHorseState.Stare:
                //플레이어 응시
                transform.LookAt(PlayerTransform);
                break;

            case SlimeHorseState.Alert:
                StateTimer += Time.deltaTime;
                if (StateTimer >= AttackCooldown)
                {
                    ChangeState(SlimeHorseState.Attack);
                }
                break;

            case SlimeHorseState.Attack:
                transform.LookAt(PlayerTransform);

                if (CheekTransform != null)
                {
                    CheekTransform.localScale = Vector3.Lerp(OriginalCheekScale, OriginalCheekScale * 1.5f, StateTimer / AimTime);
                }

                StateTimer += Time.deltaTime;
                if (StateTimer >= AimTime)
                {
                    FireSlime();
                }
                break;

            case SlimeHorseState.Down:
                StateTimer += Time.deltaTime;
                if (StateTimer >= DownDuration)
                {
                    BaseCreature.Escape();
                }
                break;

            case SlimeHorseState.Retreat:
                Vector3 lookDir = (TargetRetreatPosition - transform.position).normalized;
                lookDir.y = 0;

                if (lookDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);
                }
                transform.position = Vector3.MoveTowards(transform.position, TargetRetreatPosition, RetreatSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, TargetRetreatPosition) <= 2.0f)
                {
                    ChangeState(SlimeHorseState.Explore);
                }
                break;
        }
    }

    public void OnOcarinaused()
    {
        /*
        float distance = Vector3.Distance(transform.position, PlayerTransform.position);
        if (distance > 10f) return;
        */

        if (CurrentState == SlimeHorseState.Explore)
        {
            int rand = Random.Range(0, 100);
            if (rand < StareChance)
            {
                Debug.Log($"[슬라임 말] 오카리나에 관심을 가집니다.");
                ChangeState(SlimeHorseState.Stare);
            }
        }
        else if (CurrentState == SlimeHorseState.Stare)
        {
            Debug.Log($"[슬라임 말] 포획 가능 상태가 되었습니다.");
            ChangeState(SlimeHorseState.Capturable);
        }
    }

    void OnMouseDown()
    {
        if (CurrentState == SlimeHorseState.Capturable ||
            CurrentState == SlimeHorseState.Down)
        {
            Debug.Log($"[슬라임 말] 포획 성공");
            BaseCreature.Capture();
        }
        else if (CurrentState == SlimeHorseState.Explore ||
                CurrentState == SlimeHorseState.Stare)
        {
            HandleCaptureFailure();
        }
    }

    private void HandleCaptureFailure()
    {
        CaptureAttemptCount++;
        Debug.Log($"[슬라임 말] 포획 실패, 시도 횟수 ({CaptureAttemptCount} / 3)");

        if (CaptureAttemptCount >= 3)
        {
            Debug.Log($"[슬라임 말] 경계 모드 돌입");
            ChangeState(SlimeHorseState.Attack);
        }
        else
        {
            //10m 밖으로 도망
            Vector3 retreatDir = (transform.position - PlayerTransform.position).normalized;
            retreatDir.y = 0;

            TargetRetreatPosition = PlayerTransform.position + (retreatDir.normalized * RetreatDistance);
            //transform.position = PlayerTransform.position + (retreatDir * RetreatDistance);
            ChangeState(SlimeHorseState.Retreat);
        }
    }

    private void FireSlime()
    {
        CurrentAttackCount++;
        Debug.Log($"[슬라임 말] 투사체 발사, ({CurrentAttackCount} / {MaxAttackCount})");

        if (SlimeProjectilePrefab != null && FirePoint != null)
        {
            GameObject proj = Instantiate(SlimeProjectilePrefab, FirePoint.position, FirePoint.rotation);
            //투사체에 플레이어 방향을 알려주는 초기화 함수 호출 (SlimeProjectile 스크립트 필요)
            //proj.GetComponent<SlimeProjectile>().Setup(PlayerTransform.position);
        }

        if (CheekTransform != null) CheekTransform.localScale = OriginalCheekScale;

        if (CurrentAttackCount >= MaxAttackCount)
        {
            ChangeState(SlimeHorseState.Down);
        }
        else
        {
            ChangeState(SlimeHorseState.Alert);
        }
    }

    private void ChangeState(SlimeHorseState state)
    {
        CurrentState = state;
        StateTimer = 0f;

        switch (state)
        {
            case SlimeHorseState.Capturable:
                if (HeartEffect != null) HeartEffect.SetActive(true);
                break;

            case SlimeHorseState.Alert:
                if (CurrentAttackCount == 0) StateTimer = AttackCooldown;
                break;

            case SlimeHorseState.Down:
                if (HeartEffect != null) HeartEffect.SetActive(false);
                if (BurstEffect != null) Instantiate(BurstEffect, transform.position, Quaternion.identity);
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);
    }
}
