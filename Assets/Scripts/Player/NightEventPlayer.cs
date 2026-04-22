using UnityEngine;
using Unity.Cinemachine;

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

    [Header("사운드 설정")]
    public AudioClip[] FootstepSounds;
    public float FootstepInterval = 0.4f;
    private float FootstepTimer = 0f;

    [Header("카메라 설정")]
    public Transform virtualCameraViewPoint;
    private CinemachineCamera virtualCamera;
    private CinemachineInputAxisController inputController;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        virtualCamera = FindFirstObjectByType<CinemachineCamera>();

        if (virtualCamera != null)
        {
            if (virtualCameraViewPoint != null)
            {
                virtualCamera.Follow = virtualCameraViewPoint;
                virtualCamera.LookAt = virtualCameraViewPoint;
            }
            else
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }


            inputController = virtualCamera.GetComponent<CinemachineInputAxisController>();
        }
        else
        {
            Debug.LogWarning("[NightEventPlayer] 씬에 CinemachineCamera가 존재하지 않습니다.");
        }
    }

    void Update()
    {
        CalculateMovemenetDirection();
        HandleFootsteps();

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

    /// <summary>
    /// 다른 UI 스크립트에서 호출하면 카메라 조작을 끄고 마우스를 보이게 하는 함수
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetCameraControl(bool isEnable)
    {
        if (inputController != null)
        {
            inputController.enabled = isEnable;
        }

        if (isEnable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // 메뉴 조작을 위해 커서 제한을 풀고 보이게 함
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleFootsteps()
    {
        // 플레이어가 이동 중일 때 (방향 벡터가 0이 아닐 때)
        if (MoveDirection != Vector3.zero)
        {
            FootstepTimer += Time.deltaTime;

            // 지정한 간격(FootstepInterval)마다 한 번씩 소리 재생
            if (FootstepTimer >= FootstepInterval)
            {
                FootstepTimer = 0f; // 타이머 초기화

                if (FootstepSounds != null && FootstepSounds.Length > 0 && SoundManager.Instance != null)
                {
                    // 4개의 소리 중 하나를 랜덤으로 뽑아서 재생
                    AudioClip randomStep = FootstepSounds[Random.Range(0, FootstepSounds.Length)];
                    SoundManager.Instance.PlaySFX(randomStep, 0.5f); // 볼륨은 0.5로 약간 줄임
                }
            }
        }
        else
        {
            // 가만히 멈춰있을 때는 타이머를 리셋해서, 다음에 움직일 때 즉시 소리가 나도록 함
            FootstepTimer = 0f;
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
