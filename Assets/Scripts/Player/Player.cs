using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        MainCamera = Camera.main;
    }


    void Update()
    {
        Rotate();
        Move();
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
}
