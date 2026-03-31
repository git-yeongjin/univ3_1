using UnityEngine;

public class NightEventPlayer : MonoBehaviour
{
    Rigidbody rb;
    Transform cameraTransform;

    [Header("플레이어 이동")]
    public float MoveSpeed;
    public float RotationSpeed;

    private Vector3 MoveDirection;

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
}
