using UnityEngine;
using UnityEngine.Rendering;

public class SporeScreenEffect : MonoBehaviour
{
    public static SporeScreenEffect Instance;

    [Header("카메라 효과 설정")]
    public Volume SporeVolume;

    [Header("효과 설정")]
    public float MaxWeight = 1.0f;
    public float FadeSpeed = 3.0f;

    private float currentWeight = 0f;
    private bool isPlayerInSpore = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (SporeVolume != null)
        {
            SporeVolume.weight = 0f;
        }
    }

    public void ApplyFog()
    {
        isPlayerInSpore = true;
    }

    void LateUpdate()
    {
        if (SporeVolume == null) return;

        // 포자 안에 있으면 MaxWeight까지, 아니면 0까지 서서히 변함
        float targetWeight = isPlayerInSpore ? MaxWeight : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * FadeSpeed);

        // UI 알파값 대신 Volume의 Weight(가중치)를 조절합니다.
        SporeVolume.weight = currentWeight;

        // 다음 프레임을 위해 false로 초기화
        isPlayerInSpore = false;
    }
}
