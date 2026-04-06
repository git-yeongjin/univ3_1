using System.Collections.Generic;
using UnityEngine;

public class ObstacleFader : MonoBehaviour
{
    [Header("타겟 설정")]
    public Transform PlayerTransform;
    public Vector3 TargetOffset = new Vector3(0, 1f, 0);

    [Header("투명화(On/Off) 설정")]
    public LayerMask ObstacleLayer;

    // 숨겨진(Off) 오브젝트들을 기억해둘 리스트
    private List<Renderer> hiddenRenderers = new List<Renderer>();
    // 이번 프레임에 레이저에 맞은 오브젝트들
    private List<Renderer> currentHits = new List<Renderer>();

    void Update()
    {
        if (PlayerTransform == null) return;

        Vector3 targetPosition = PlayerTransform.position + TargetOffset;
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // 1. 카메라 -> 플레이어 방향으로 레이저 발사!
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, ObstacleLayer);

        currentHits.Clear();

        // 2. 레이저에 맞은 장애물 Off (렌더러 끄기)
        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                currentHits.Add(renderer);

                // 처음 맞는 장애물이라면 리스트에 넣고 그래픽을 끕니다.
                if (!hiddenRenderers.Contains(renderer))
                {
                    hiddenRenderers.Add(renderer);

                    // 💡 핵심: 그래픽만 끄고 충돌체(Collider)는 남겨서 통과되는 버그를 막습니다.
                    renderer.enabled = false;
                }
            }
        }

        // 3. 레이저에서 벗어난 장애물 다시 On (렌더러 켜기)
        List<Renderer> toRemove = new List<Renderer>();
        foreach (Renderer r in hiddenRenderers)
        {
            if (!currentHits.Contains(r))
            {
                if (r != null) r.enabled = true; // 💡 그래픽 다시 켜기
                toRemove.Add(r);
            }
        }

        // 복구가 끝난 녀석들은 숨김 리스트에서 제거
        foreach (Renderer r in toRemove)
        {
            hiddenRenderers.Remove(r);
        }
    }

    void OnDrawGizmos()
    {
        if (PlayerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, PlayerTransform.position + TargetOffset);
        }
    }
}