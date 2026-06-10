using UnityEngine;

public class BakePlayer : MonoBehaviour
{
    private Rigidbody rb;
    private Camera MainCamera;
    private BakeEventUI bakeUI;

    [Header("플레이어 회전")]
    public float MouseSpeed;
    float yRotation;
    float xRotation;

    [Header("플레이어 이동속도")]
    public float MoveSpeed;
    private Vector3 MoveDirection;
    float h;
    float v;

    [Header("사운드 설정")]
    public AudioClip[] FootstepSounds;
    public float FootstepInterval = 0.4f;
    private float FootstepTimer = 0f;

    private bool isCursorLocked = true;
    public Transform playerPositionSave;


    void Start()
    {
        bakeUI = FindAnyObjectByType<BakeEventUI>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        MainCamera = Camera.main;

        LockCursor(true);
    }

    void Update()
    {
        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            LockCursor(!isCursorLocked); // 현재 상태의 반대로 전환
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.position = playerPositionSave.position;
            rb.linearVelocity = Vector3.zero; // 순간이동 시 미끄러짐 방지
        }

        bool canMoveAndLook = true;
        if (bakeUI != null && bakeUI.isVideoPlaying)
        {
            canMoveAndLook = false; // 영상 재생 중이면 조작 불가
        }

        if (isCursorLocked && canMoveAndLook)
        {
            Rotate();
        }

        if (canMoveAndLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            //대각선 이동 속도 빨라짐 방지
            MoveDirection = (transform.forward * v + transform.right * h).normalized;
        }
        else
        {
            MoveDirection = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + MoveDirection * MoveSpeed * Time.fixedDeltaTime);
    }

    void Rotate()
    {
        float MouseX = Input.GetAxisRaw("Mouse X") * MouseSpeed * Time.deltaTime;
        float MouseY = Input.GetAxisRaw("Mouse Y") * MouseSpeed * Time.deltaTime;

        yRotation += MouseX;
        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation, -25f, 70f);

        if (MainCamera != null)
        {
            MainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void LockCursor(bool isLocked)
    {
        isCursorLocked = isLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // 마우스 자유롭게 풀기
            Cursor.visible = true; // 마우스 커서 보이게 하기
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
}
