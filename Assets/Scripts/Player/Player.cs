using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Camera MainCamera;

    [Header("플레이어 회전")]
    public float MouseSpeed;
    float yRotation;
    float xRotation;

    [Header("플레이어 이동속도")]
    public float MoveSpeed;
    private Vector3 MoveDirection;
    float h;
    float v;

    private bool isCursorLocked = true;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            //GameManager.Instance.isBakingTime = false;
        }
        else
        {
            Debug.LogError($"[BakePlayer] GameManager.Instance를 찾을 수 없습니다.");
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        MainCamera = Camera.main;

        LockCursor(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            LockCursor(!isCursorLocked); // 현재 상태의 반대로 전환
        }

        if (isCursorLocked)
        {
            Rotate();
        }
        if (GameManager.Instance != null && !GameManager.Instance.isBakingTime)
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
}
