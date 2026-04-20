using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlimeProjectile : MonoBehaviour
{

    [Header("투사체 설정")]
    public float Speed = 15f;
    public float Lifetime = 5f;

    [Header("이펙트 설정")]
    public GameObject SlimePuddlePrefab;

    [Header("사운드 설정")]
    public AudioClip HitSound;

    private Rigidbody rb;
    private Vector3 MoveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 targetPosition)
    {
        Vector3 aimPosition = targetPosition;
        aimPosition.y += 1.0f;

        MoveDirection = (aimPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(MoveDirection);

        rb.linearVelocity = MoveDirection * Speed;

        Destroy(gameObject, Lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        // 3. 웅덩이 장판 생성
        if (SlimePuddlePrefab != null)
        {
            // 노멀 벡터(표면 방향)를 바탕으로 바닥이나 벽에 찰싹 달라붙는 회전값 계산
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Quaternion prefabFixRotation = Quaternion.Euler(-90, 0, 0);

            Quaternion finalSapwnRatation = surfaceRotation * prefabFixRotation;

            if (SoundManager.Instance != null && HitSound != null)
            {
                SoundManager.Instance.PlaySFX(HitSound);
            }

            // 지정된 위치와 회전값으로 웅덩이 소환
            GameObject puddle = Instantiate(SlimePuddlePrefab, contact.point, finalSapwnRatation);

            // Z-Fighting(깜빡임) 방지를 위해 표면에서 아주 미세하게 띄워주기
            puddle.transform.position += contact.normal * 0.005f;

            // 10초 뒤 웅덩이 파괴 (메모리 관리)
            Destroy(puddle, 10f);
        }

        // 4. 부딪혔으니 투사체(본체)는 파괴
        Destroy(gameObject);
    }
}
