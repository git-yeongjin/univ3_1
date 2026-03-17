using Unity.Mathematics;
using UnityEngine;

public class Creature_Frog : MonoBehaviour
{
    private Creature BaseCreature;
    private Transform PlayerTransform;

    public enum ForgState { Wandering, Fleeing, Lured, Eating }
    [Header("현재 상태")]
    public ForgState CurrentState = ForgState.Wandering;

    [Header("거리 및 시간 설정")]
    //플레이어와 거리
    public float FleeDistance = 5.0f;
    //먹이 인식 거리
    public float FoodDetectRadius = 5.0f;
    //점프 주기
    public float JumpInterval = 2.0f;
    //점프 대기 시간
    public float JumpPrepTime = 1.0f;
    //점프 시 이동거리
    public float JumpForceDistance = 4.0f;
    //먹이 먹는 시간
    public float EatDuration = 10.0f;
    //먹이로 가는 속도
    public float LureWalkSpeed = 1.5f;

    private float StateTimer = 0f;
    private bool isJumping = false;
    private Transform TargetFood;

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;
    }


    void Update()
    {
        if (PlayerTransform == null) return;

        if (CurrentState != ForgState.Eating)
        {
            FindNearestFood();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);

        switch (CurrentState)
        {
            case ForgState.Wandering:
                if (distanceToPlayer <= FleeDistance)
                {
                    Debug.Log($"[개구리] 플레이어 발견");
                    ChangeState(ForgState.Fleeing);
                }
                break;

            case ForgState.Fleeing:
                if (distanceToPlayer > FleeDistance + 2.0f)
                {
                    ChangeState(ForgState.Wandering);
                }
                else
                {
                    HandleFleeing();
                }
                break;

            case ForgState.Lured:
                if (TargetFood == null)
                {
                    ChangeState(ForgState.Wandering);
                    break;
                }
                MoveToFood();
                break;

            case ForgState.Eating:
                StateTimer += Time.deltaTime;
                if (StateTimer >= EatDuration)
                {
                    if (TargetFood != null) Destroy(TargetFood.gameObject);
                    ChangeState(ForgState.Wandering);
                }
                break;
        }
    }

    private void FindNearestFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, FoodDetectRadius);
        float closestDist = Mathf.Infinity;
        Transform bestFood = null;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Food"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestFood = hit.transform;
                }
            }
        }

        if (bestFood != null && CurrentState != ForgState.Lured)
        {
            TargetFood = bestFood;
            ChangeState(ForgState.Lured);
            Debug.Log($"[개구리] 개구리가 먹이로 가는 중");
        }
    }

    private void HandleFleeing()
    {
        StateTimer += Time.deltaTime;

        if (StateTimer < JumpPrepTime)
        {
            Vector3 fleeDir = (transform.position - PlayerTransform.position).normalized;
            fleeDir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fleeDir), Time.deltaTime * 10f);
        }
        else if (StateTimer >= JumpPrepTime && !isJumping)
        {
            JumpAway();
            isJumping = true;
        }

        if (StateTimer >= JumpInterval)
        {
            StateTimer = 0f;
            isJumping = false;
        }
    }

    private void JumpAway()
    {
        // 간단한 위치 이동 로직 (실제 프로젝트에서는 Rigidbody AddForce나 궤적 애니메이션 적용 권장)
        Vector3 fleeDir = transform.forward;
        transform.position += fleeDir * JumpForceDistance;
    }

    private void MoveToFood()
    {
        Vector3 direction = (TargetFood.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction * LureWalkSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, TargetFood.position) <= 1.0f)
        {
            ChangeState(ForgState.Eating);
        }
    }

    private void ChangeState(ForgState state)
    {
        CurrentState = state;
        StateTimer = 0f;
        isJumping = false;
    }

    void OnMouseDown()
    {
        bool isHoldingNet = true;

        if (!isHoldingNet) return;

        if (CurrentState == ForgState.Eating)
        {
            BaseCreature.Capture();
        }
        else
        {
            ChangeState(ForgState.Fleeing);
            StateTimer = JumpPrepTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FleeDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FoodDetectRadius);
    }
}
