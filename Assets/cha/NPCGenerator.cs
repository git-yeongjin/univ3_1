using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("--- 의상 및 눈 ---")]
    public GameObject[] allOutfits;
    public GameObject[] eyeParts;

    [Header("--- 헤어 조합 (Hair) ---")]
    public GameObject[] frontHairs;
    public GameObject[] backHairs; // [0]번이나 특정 인덱스를 '숏컷'으로 가정
    public GameObject[] sideHairs;

    [Header("--- 예외 규칙 설정 ---")]
    [Tooltip("숏컷과만 조합되어야 하는 앞머리의 인덱스 번호")]
    public int specialFrontHairIndex = 0;
    [Tooltip("위 앞머리가 선택되었을 때 강제로 켜질 뒷머리(숏컷)의 인덱스")]
    public int shortCutBackIndex = 0;

    void Start()
    {
        GenerateRandomNPC();
    }

    [ContextMenu("Generate Random NPC")]
    public void GenerateRandomNPC()
    {
        // 1. 기본 파츠들 랜덤 결정
        SetRandomPart(allOutfits);
        SetRandomPart(eyeParts);

        // 2. 앞머리 먼저 결정 (기준점이 됨)
        int selectedFrontIndex = GetRandomIndex(frontHairs);
        ActivateSpecificPart(frontHairs, selectedFrontIndex);

        // 3. [예외 규칙 검사] 특정 앞머리가 선택되었는가?
        if (selectedFrontIndex == specialFrontHairIndex)
        {
            // 규칙: 특정 앞머리일 때는 무조건 지정된 숏컷 뒷머리만 켜고, 옆머리는 모두 끔
            ActivateSpecificPart(backHairs, shortCutBackIndex);
            DisableAllParts(sideHairs);
            Debug.Log("규칙 적용: 숏컷 전용 앞머리 조합 생성");
        }
        else
        {
            // 일반적인 경우: 뒷머리와 옆머리도 랜덤으로 결정
            SetRandomPart(backHairs);
            SetRandomPart(sideHairs);
        }
    }

    // 배열에서 랜덤 인덱스만 뽑아오는 함수
    private int GetRandomIndex(GameObject[] parts)
    {
        if (parts == null || parts.Length == 0) return -1;
        return Random.Range(0, parts.Length);
    }

    // 특정 인덱스만 켜고 나머지는 끄는 함수
    private void ActivateSpecificPart(GameObject[] parts, int index)
    {
        if (parts == null || index < 0 || index >= parts.Length) return;
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null) parts[i].SetActive(i == index);
        }
    }

    private void SetRandomPart(GameObject[] parts)
    {
        if (parts == null || parts.Length == 0) return;
        int randomIndex = Random.Range(0, parts.Length);
        ActivateSpecificPart(parts, randomIndex);
    }

    private void DisableAllParts(GameObject[] parts)
    {
        if (parts == null) return;
        foreach (var part in parts) if (part != null) part.SetActive(false);
    }
}
