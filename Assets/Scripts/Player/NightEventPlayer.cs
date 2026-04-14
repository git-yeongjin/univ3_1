using UnityEngine;

public class NightEventPlayer : MonoBehaviour
{
    Rigidbody rb;
    Transform cameraTransform;

    [Header("플레이어 이동")]
    public float MoveSpeed;
    public float RotationSpeed;
    private Vector3 MoveDirection;

    [Header("상호작용 설정")]
    public float InteractRadius = 3.0f;
    public LayerMask CreatureLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError($"[NightEventPlayer] 씬에 MainCamera 태그가 달린 카메라가 없습니다.");
        }
    }

    void Update()
    {
        CalculateMovemenetDirection();

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInteractWithCreature();
        }
    }

    void FixedUpdate()
    {
        if (MoveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + MoveDirection * MoveSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void CalculateMovemenetDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            MoveDirection = (camForward * v + camRight * h).normalized;
        }
    }

    private void TryInteractWithCreature()
    {
        //CreatureLayer인 오브젝트만 InteractRadius범위만큼 감지하기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractRadius, CreatureLayer);

        float closestDistance = float.MaxValue;
        Collider closestCreature = null;

        //범위 안에 들어온 것들 중 가장 가까운 크리쳐 하나를 찾음
        foreach (Collider hit in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCreature = hit;
            }
        }

        //해당 크리쳐의 스크립트를 불러와 상호작용
        if (closestCreature != null)
        {
            Debug.Log($"[NightEventPlayer] {closestCreature.name}와(과) 상호작용");

            //인형패턴 상호작용
            Creature_Doll doll = closestCreature.GetComponent<Creature_Doll>();
            if (doll != null)
            {
                doll.OnPlayerInteract();
                return;
            }

            //버섯패턴 상호작용
            Creature_Mushroom mushroom = closestCreature.GetComponent<Creature_Mushroom>();
            if (mushroom != null)
            {
                mushroom.OnPlayerInteract();
                return;
            }

            //슬라임말 패턴 상호작용
            Creature_SlimeHorse slime = closestCreature.GetComponent<Creature_SlimeHorse>();
            if (slime != null)
            {
                slime.OnPlayerInteract();
                return;
            }

            // 다른 크리쳐들도 이런 식으로 추가
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }
}
