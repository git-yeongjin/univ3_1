using Unity.Mathematics;
using UnityEngine;
using Unity.Cinemachine;

public class NightEventPlayer : MonoBehaviour
{
    Rigidbody rb;
    Transform cameraTransform;

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
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        //Rotate();
        Move();
    }
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 MoveVector = (camForward * v + camRight * h).normalized;

            if (MoveVector != Vector3.zero)
            {
                transform.position += MoveVector * MoveSpeed * Time.deltaTime;

                Quaternion Rotation = Quaternion.LookRotation(MoveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, Time.deltaTime * 15f);
            }
        }
    }
}
