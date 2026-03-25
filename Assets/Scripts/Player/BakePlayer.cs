using UnityEngine;

public class BakePlayer : MonoBehaviour
{
    private GameManager GM;

    Rigidbody rb;
    Camera MainCamera;
    [Header("플레이어 회전")]
    public float MouseSpeed;
    float yRotation;
    float xRotation;

    [Header("플레이어 이동속도")]
    public float MoveSpeed;
    float h;
    float v;

    private bool isCursorLocked = true;

    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null) Debug.LogError($"GameManager를 찾을 수 없습니다.");
        GM.isBakingTime = true;

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
        if (!GM.isBakingTime)
        {
            Move();
        }
    }

    void Rotate()
    {
        float MouseX = Input.GetAxisRaw("Mouse X") * MouseSpeed * Time.deltaTime;
        float MouseY = Input.GetAxisRaw("Mouse Y") * MouseSpeed * Time.deltaTime;

        yRotation += MouseX;
        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        MainCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector3 MoveVector = transform.forward * v + transform.right * h;

        transform.position += MoveVector.normalized * MoveSpeed * Time.deltaTime;
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
