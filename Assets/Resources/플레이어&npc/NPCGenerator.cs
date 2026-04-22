using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    [Header("--- 외형 파츠 배열 ---")]
    public GameObject[] frontHairs;
    public GameObject[] backHairs; 
    public GameObject[] sideHairs;
    public GameObject[] topOutfits; 
    public GameObject[] eyeParts;

    [Header("--- 바디 설정 ---")]
    public GameObject bodyObject; // BODY_all 또는 BODY_skirt_short 등

    [Header("--- 슬롯 번호 (오브젝트 내부) ---")]
    public int hair_ColorIdx = 0;     
    public int top_ColorIdx = 0;      
    public int eye_PupilIdx = 1;      

    [Header("--- 색상 범위 설정 ---")]
    [Range(0f, 1f)] public float hueMin = 0f;
    [Range(0f, 1f)] public float hueMax = 1f;
    [Range(0f, 1f)] public float saturation = 0.35f;
    [Range(0f, 1f)] public float brightness = 0.9f;

    [Header("--- 숏컷 규칙 ---")]
    public int specialFrontHairIndex = 0; 
    public int shortCutBackIndex = 0;

    [Header("--- 셰이더 변수명 ---")]
    public string colorPropertyName = "_BaseColor";
    public string shadePropertyName = "_1st_ShadeColor";

    private GameObject currentFront, currentBack, currentSide, currentTop, currentEye;

    void Start()
    {
        RandomizeAppearance();
        RandomizeColors();
    }

    [ContextMenu("1. 외형만 랜덤")]
    public void RandomizeAppearance()
    {
        int fIdx = Random.Range(0, frontHairs.Length);
        currentFront = frontHairs[fIdx];
        ActivateSpecificPart(frontHairs, fIdx);

        if (fIdx == specialFrontHairIndex)
        {
            currentBack = backHairs[shortCutBackIndex];
            currentSide = null;
            ActivateSpecificPart(backHairs, shortCutBackIndex);
            DisableAllParts(sideHairs);
        }
        else
        {
            int bIdx = Random.Range(0, backHairs.Length);
            int sIdx = Random.Range(0, sideHairs.Length);
            currentBack = backHairs[bIdx];
            currentSide = sideHairs[sIdx];
            ActivateSpecificPart(backHairs, bIdx);
            ActivateSpecificPart(sideHairs, sIdx);
        }

        currentTop = GetRandomAndActivate(topOutfits);
        currentEye = GetRandomAndActivate(eyeParts);
    }
    [ContextMenu("2. 색상만 랜덤")]
    public void RandomizeColors()
    {
        // --- 1. 부위별로 각각 랜덤 색상 생성 ---
        Color hairCol = GetPrettyColorFromRange();   // 머리카락용
        Color topCol = GetPrettyColorFromRange();    // 상의(원피스+팔)용
        Color bottomCol = GetPrettyColorFromRange(); // 하의+눈동자용 (이 둘은 세트로 묶는 게 자연스러워요)

        // --- 2. 머리카락 적용 ---
        ApplyColorToSlot(currentFront, hairCol, hair_ColorIdx);
        ApplyColorToSlot(currentBack, hairCol, hair_ColorIdx);
        ApplyColorToSlot(currentSide, hairCol, hair_ColorIdx);

        // --- 3. 상의(원피스 파츠 + 바디 내 상의/팔 슬롯) 적용 ---
        ApplyColorToSlot(currentTop, topCol, top_ColorIdx); // 원피스 오브젝트
        ApplyColorBySearchingName(bodyObject, topCol, "top"); // 바디 내 'top' 이름 들어간 슬롯
        ApplyColorBySearchingName(currentTop, topCol, "top"); // 바디 내 'top' 이름 들어간 슬롯

        // --- 4. 하의(바디 내 하의 슬롯) & 눈동자 적용 ---
        ApplyColorBySearchingName(currentTop, bottomCol, "bottom"); // 바디 내 'bottom' 슬롯
        ApplyColorToSlot(currentEye, bottomCol, eye_PupilIdx); // 눈동자
    }

    // [핵심] 렌더러의 슬롯을 돌며 이름을 검사하고 'PropertyBlock'을 입히는 함수
    private void ApplyColorBySearchingName(GameObject target, Color col, string searchKey)
    {
        if (target == null) return;
        Renderer rend = target.GetComponent<Renderer>();
        if (rend == null) return;

        // 원본 메테리얼 리스트(sharedMaterials)를 확인하여 슬롯 번호를 찾습니다.
        for (int i = 0; i < rend.sharedMaterials.Length; i++)
        {
            if (rend.sharedMaterials[i] != null && rend.sharedMaterials[i].name.ToLower().Contains(searchKey.ToLower()))
            {
                // 찾았다면 해당 슬롯(i)에만 개별 PropertyBlock을 입힙니다.
                ApplyColorToSlot(target, col, i);
            }
        }
    }

    private void ApplyColorToSlot(GameObject target, Color col, int matIndex)
    {
        if (target == null) return;
        Renderer rend = target.GetComponent<Renderer>();
        if (rend != null && matIndex >= 0 && matIndex < rend.sharedMaterials.Length)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            // GetPropertyBlock을 통해 해당 슬롯의 기존 값을 가져오고 수정한 뒤 다시 Set합니다.
            rend.GetPropertyBlock(propBlock, matIndex);
            propBlock.SetColor(colorPropertyName, col);
            propBlock.SetColor(shadePropertyName, col * 0.75f);
            rend.SetPropertyBlock(propBlock, matIndex);
        }
    }

    private Color GetPrettyColorFromRange()
    {
        float h = Random.Range(hueMin, hueMax);
        return Color.HSVToRGB(h, saturation, brightness);
    }

    private GameObject GetRandomAndActivate(GameObject[] parts)
    {
        if (parts == null || parts.Length == 0) return null;
        int idx = Random.Range(0, parts.Length);
        ActivateSpecificPart(parts, idx);
        return parts[idx];
    }

    private void ActivateSpecificPart(GameObject[] parts, int index)
    {
        for (int i = 0; i < parts.Length; i++)
            if (parts[i] != null) parts[i].SetActive(i == index);
    }

    private void DisableAllParts(GameObject[] parts)
    {
        foreach (var part in parts) if (part != null) part.SetActive(false);
    }
}