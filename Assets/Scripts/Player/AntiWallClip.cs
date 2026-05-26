using UnityEngine;

public class AntiWallClip : MonoBehaviour
{
    public LayerMask WallLayer;

    public float CheckRadius = 0.3f;

    private Vector3 lastPosition;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 direction = transform.position - lastPosition;
        float distance = direction.magnitude;


        if (distance > 0)
        {
            if (Physics.SphereCast(lastPosition, CheckRadius, direction.normalized, out RaycastHit hit, distance, WallLayer))
            {
                transform.position = hit.point + (hit.normal * CheckRadius);

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }

        lastPosition = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CheckRadius);
    }
}
