using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DollItemType
{
    Button,
    Ribbon
}

public class Creature_Doll : MonoBehaviour
{
    private Creature BaseCreature;
    private GameManager GM;
    private Camera MainCamera;

    [Header("인형")]
    //현재 호감도
    public float CurrentAffection;
    //목표 호감도
    public float MaxAffection;
    //현재 불안도
    public float CurrentAnxiety;
    //최대 불안도 -> 넘기면 포획 실패
    public float MaxAnxiety = 100f;

    [Header("미니게임 상태")]
    public bool isGameStarted = false;
    public bool isCapturable = false;
    public float TimeLimit = 180f;
    [SerializeField] private float CurrentTime;
    private bool isHintShown = false;

    [Header("물건 설정")]
    public int RequiredItemCount = 0;
    public int CurrentDeliverd = 0;

    //현재 요구 아이템
    public DollItemType RequiredItem;
    //버튼
    public GameObject ButtonPrefab;
    //리본
    public GameObject RibbonPrefab;

    public LayerMask GroundLayer = ~0;
    public float ItemYOffset = 0.1f;

    [SerializeField]
    //랜덤 스폰 위치
    private Transform[] SpawnLocations;
    //스폰된 아이템 추적
    public List<GameObject> SpawnedItems = new List<GameObject>();

    [Header("UI 및 이펙트 프리팹")]
    public GameObject ExclamationMark;
    public GameObject HeartEffect;

    public GameObject PopupUI;
    public GameObject SpeechBubble;
    public Sprite[] SpeechBubbleSprites;

    [Header("힌트 설정")]
    public GameObject HintArrowPrefab;
    private List<GameObject> SpawnedArrows = new List<GameObject>();

    public RectTransform UIArrowPrefab;
    public Transform UICanvasTransform;
    public float EdgePadding = 50f;
    public float ArrowRotationOffset = -90f;

    private Dictionary<GameObject, RectTransform> OffScreenUIArrows = new Dictionary<GameObject, RectTransform>();

    void Start()
    {
        BaseCreature = GetComponent<Creature>();
        GM = FindAnyObjectByType<GameManager>();

        MainCamera = Camera.main;

        if (ExclamationMark != null) ExclamationMark.SetActive(false);
        //if (SpeechBubble != null) SpeechBubble.SetActive(false);
        if (HeartEffect != null) HeartEffect.SetActive(false);

        if (UICanvasTransform == null)
        {
            Canvas mainCanvas = FindAnyObjectByType<Canvas>();
            if (mainCanvas != null)
            {
                UICanvasTransform = mainCanvas.transform;
            }
            else
            {
                Debug.LogWarning("[인형 패턴] 씬에 Canvas가 없어서 화면 밖 화살표 UI를 띄울 수 없습니다");
            }
        }
    }

    public void SetItemSpawnLocations(Transform[] points)
    {
        SpawnLocations = points;
    }

    void Update()
    {
        if (isGameStarted)
        {
            PlayMiniGameTimer();
        }

        if (SpeechBubble != null && SpeechBubble.activeSelf)
        {
            SpeechBubble.transform.rotation = MainCamera.transform.rotation;
        }

        foreach (GameObject arrow in SpawnedArrows)
        {
            if (arrow != null)
            {
                arrow.transform.rotation = MainCamera.transform.rotation;
                arrow.transform.Rotate(0, 0, 90f, Space.Self);
            }
        }

        if (isHintShown)
        {
            UpdateOffScreenUI();
        }
    }

    public void OnPlayerInteract()
    {
        if (isCapturable)
        {
            Debug.Log($"[인형 패턴] 포획 성공!");
            if (HeartEffect != null) HeartEffect.SetActive(false);
            BaseCreature.Capture();
            return;
        }

        if (!isGameStarted)
        {
            StartMiniGame();
        }
        else
        {
            TryDeliverItem();
        }
    }

    private void StartMiniGame()
    {
        isGameStarted = true;
        CurrentTime = TimeLimit;
        isHintShown = false;
        CurrentDeliverd = 0;

        if (ExclamationMark != null) ExclamationMark.SetActive(true);

        RequiredItemCount = 3;
        if (SpawnLocations != null && SpawnLocations.Length > 0)
        {
            RequiredItemCount = Mathf.Min(RequiredItemCount, SpawnLocations.Length);
        }


        RequiredItem = (Random.Range(0, 2) == 0) ? DollItemType.Button : DollItemType.Ribbon;
        string itemName = RequiredItem == DollItemType.Button ? "단추" : "리본";
        Debug.Log($"[인형 패턴] 찾아야 할 물건 개수 : {RequiredItemCount}개");

        if (SpeechBubble != null)
        {
            SpriteRenderer speechBubbleRenderer = SpeechBubble.GetComponent<SpriteRenderer>();
            switch (RequiredItem)
            {
                case DollItemType.Button:
                    speechBubbleRenderer.sprite = SpeechBubbleSprites[1];
                    break;
                case DollItemType.Ribbon:
                    speechBubbleRenderer.sprite = SpeechBubbleSprites[2];
                    break;
            }
        }

        SpawnHiddenItems();
    }

    private void PlayMiniGameTimer()
    {
        CurrentTime -= Time.deltaTime;
        CurrentAnxiety = Mathf.Lerp(MaxAnxiety, 0, CurrentTime / TimeLimit);

        if (CurrentTime <= 30f && !isHintShown)
        {
            ShowItemHints();
        }

        if (CurrentTime <= 0)
        {
            Debug.Log("[인형 패턴] 제한 시간 초과로 크리쳐가 도망갔습니다.");
            isGameStarted = false;

            CleanSpawnedItem();
            BaseCreature.Escape();
        }
    }

    private void TryDeliverItem()
    {
        PlayerInventory player = FindAnyObjectByType<PlayerInventory>();
        if (player == null) return;

        //임시
        bool playerHasItem = false;

        if (RequiredItem == DollItemType.Button && player.ButtonCount > 0)
        {
            player.ButtonCount--;
            playerHasItem = true;
        }
        else if (RequiredItem == DollItemType.Ribbon && player.RibbonCount > 0)
        {
            player.RibbonCount--;
            playerHasItem = true;
        }

        if (playerHasItem)
        {
            CurrentDeliverd++;
            Debug.Log($"[인형 패턴] 물건 전달 완료 ({CurrentDeliverd} / {RequiredItemCount})");

            if (CurrentDeliverd >= RequiredItemCount)
            {
                SuccessPattern();
            }
        }
    }

    private void SuccessPattern()
    {
        isGameStarted = false;
        isCapturable = true;

        CleanSpawnedItem();

        if (SpeechBubble != null) SpeechBubble.SetActive(false);
        if (HeartEffect != null) HeartEffect.SetActive(true);
    }

    private void SpawnHiddenItems()
    {
        if (SpawnLocations == null || SpawnLocations.Length < RequiredItemCount)
        {
            Debug.LogWarning("스폰 위치가 요구 개수보다 적습니다.");
            return;
        }

        GameObject prefabToSpawn = (RequiredItem == DollItemType.Button) ? ButtonPrefab : RibbonPrefab;

        List<Transform> availableLocations = new List<Transform>(SpawnLocations);

        for (int i = 0; i < RequiredItemCount; i++)
        {
            int randomIndex = Random.Range(0, availableLocations.Count);
            Transform spawnPoint = availableLocations[randomIndex];

            Vector3 finalSpawnPos = spawnPoint.position;
            RaycastHit hit;

            if (Physics.Raycast(spawnPoint.position + Vector3.up * 2f, Vector3.down, out hit, 20f, GroundLayer))
            {
                // 레이저가 땅에 부딪혔다면, 부딪힌 그 좌표(hit.point)를 가져오고 오프셋을 더합니다.
                finalSpawnPos = hit.point + (Vector3.up * ItemYOffset);
            }
            else
            {
                Debug.LogWarning($"[인형 패턴] {spawnPoint.name} 아래에 땅이 없어 원래 위치에 스폰합니다.");
            }

            GameObject newItem = Instantiate(prefabToSpawn, finalSpawnPos, spawnPoint.rotation);

            ItemObj_Doll itemScript = newItem.GetComponent<ItemObj_Doll>();
            if (itemScript != null)
            {
                itemScript.OwnerDoll = this;
            }

            SpawnedItems.Add(newItem);
            availableLocations.RemoveAt(randomIndex);
        }

        Debug.Log($"[인형 패턴] 랜덤 위치에 아이템이 생성 되었습니다.");
    }

    private void CleanSpawnedItem()
    {
        //오브젝트 위 화살표
        foreach (GameObject arrow in SpawnedArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        SpawnedArrows.Clear();

        //UI화살표
        foreach (var kvp in OffScreenUIArrows)
        {
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);
        }
        OffScreenUIArrows.Clear();

        //단추, 리본 프리팹
        foreach (GameObject item in SpawnedItems)
        {
            if (item != null) Destroy(item);
        }
        SpawnedItems.Clear();
    }

    private void ShowItemHints()
    {
        isHintShown = true;
        Debug.Log($"[인형 패턴] 남은 아이템 위치를 표시합니다.");

        if (HintArrowPrefab == null)
        {
            Debug.LogError($"[인형 패턴] HintArrowPrefab이 비어있습니다.");
            return;
        }

        CreateArrows();
    }

    private void CreateArrows()
    {
        foreach (GameObject item in SpawnedItems)
        {
            // 아직 먹지 않은(null이 아닌) 아이템 위에만 화살표 생성
            if (item != null)
            {
                if (HintArrowPrefab != null)
                {
                    Vector3 arrowPos = item.transform.position + Vector3.up * 2.0f;
                    GameObject arrow = Instantiate(HintArrowPrefab, arrowPos, Quaternion.identity);
                    SpawnedArrows.Add(arrow);
                }
            }

            if (UIArrowPrefab != null && UICanvasTransform != null)
            {
                if (!OffScreenUIArrows.ContainsKey(item))
                {
                    RectTransform uiArrow = Instantiate(UIArrowPrefab, UICanvasTransform);
                    uiArrow.gameObject.SetActive(true);
                    OffScreenUIArrows.Add(item, uiArrow);
                }
            }
        }
    }

    public void RefreshArrows()
    {
        // 기존 화살표 전부 삭제
        foreach (GameObject arrow in SpawnedArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        SpawnedArrows.Clear();

        // 남은 아이템 위에만 다시 화살표 생성
        CreateArrows();
    }

    public void OnItemPickedUp(GameObject pickedItem)
    {
        // 1. 관리 리스트에서 방금 먹은 아이템을 완전히 빼버립니다. (Destroy 딜레이 버그 방지)
        if (SpawnedItems.Contains(pickedItem))
        {
            SpawnedItems.Remove(pickedItem);
        }

        if (OffScreenUIArrows.ContainsKey(pickedItem))
        {
            if (OffScreenUIArrows[pickedItem] != null)
            {
                Destroy(OffScreenUIArrows[pickedItem].gameObject);
            }
            OffScreenUIArrows.Remove(pickedItem);
        }

        // 2. 힌트가 켜져있는 상태라면 화살표를 즉시 갱신합니다.
        if (isHintShown)
        {
            RefreshArrows();
        }
    }

    private void UpdateOffScreenUI()
    {
        // 화면의 중심 좌표 계산
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // 화살표가 화면 가장자리를 뚫고 나가지 않도록 한계선 설정
        float boundsX = (Screen.width / 2f) - EdgePadding;
        float boundsY = (Screen.height / 2f) - EdgePadding;

        foreach (var kvp in OffScreenUIArrows)
        {
            GameObject item = kvp.Key;
            RectTransform uiArrow = kvp.Value;

            if (item == null || uiArrow == null) continue;

            // 아이템의 3D 월드 좌표를 2D 화면 좌표로 변환
            Vector3 screenPos = MainCamera.WorldToScreenPoint(item.transform.position);

            // 카메라 뒤(z<0)에 있거나 화면 밖을 벗어났는지 확인
            bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

            if (isOffScreen)
            {
                uiArrow.gameObject.SetActive(true); // 화면 밖이면 화살표를 켭니다!

                Vector3 dir = screenPos - screenCenter;

                // 아이템이 카메라 등 뒤에 있다면 방향을 반대로 뒤집어줍니다.
                if (screenPos.z < 0)
                {
                    dir *= -1f;
                }

                // 화살표 회전 (원본 이미지가 "위(↑)"를 바라보고 있다고 가정합니다)
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                uiArrow.localEulerAngles = new Vector3(0, 0, angle + ArrowRotationOffset);

                float maxRatio = Mathf.Max(Mathf.Abs(dir.x / boundsX), Mathf.Abs(dir.y / boundsY));
                Vector2 finalPos = (Vector2)screenCenter + ((Vector2)dir / maxRatio);

                uiArrow.position = finalPos;
            }
            else
            {
                // 화면 안으로 들어오면 UI 화살표는 숨깁니다. (3D 화살표가 보일 테니까요!)
                uiArrow.gameObject.SetActive(false);
            }
        }
    }
}
