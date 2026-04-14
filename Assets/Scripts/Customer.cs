using UnityEngine;
using TMPro;
using System.Collections;

public class Customer : MonoBehaviour
{

    [Header("말풍선 UI")]
    public GameObject SpeechBubble; // 손님 머리 위 말풍선 배경 오브젝트
    public TMP_Text DialogueText;

    [Header("이동 설정")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 10.0f;
    private Vector3 pickupPosition;

    [Header("주문 정보")]
    public BreadType MyOrder;
    public bool wantsPackaging;
    private string myDialogue;

    private bool isWaitingForClick = false;
    private DayEvent DE;
    private DayEventUI dayEventUI;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        if (DE == null)
        {
            Debug.LogError($"[Customer] DayEvent를 찾을 수 없습니다.");
        }
        dayEventUI = FindAnyObjectByType<DayEventUI>();
        if (SpeechBubble != null) SpeechBubble.SetActive(false);
    }

    public void SetOrder(BreadType order, bool packaging, string dialogue, Vector3 targetPos)
    {
        MyOrder = order;
        wantsPackaging = packaging;
        myDialogue = dialogue;
        pickupPosition = targetPos;

        string packStr = wantsPackaging ? "포장" : "매장";
        Debug.Log($"[Customer] 현재 주문 : {MyOrder} / {packStr}");

        isWaitingForClick = true;
        Debug.Log($"[Customer] 손님이 대기 중입니다. 마우스로 클릭해주세요.");
    }

    void OnMouseDown()
    {
        if (isWaitingForClick)
        {
            isWaitingForClick = false; // 중복 클릭 방지
            StartCoroutine(ShowDialogueRoutine());
        }
    }

    private IEnumerator ShowDialogueRoutine()
    {
        //말풍선에 텍스트를 넣고 켜기
        if (SpeechBubble != null && DialogueText != null)
        {
            DialogueText.text = myDialogue;
            SpeechBubble.SetActive(true);
            Debug.Log($"[Customer] 말풍선 출력: {myDialogue}");
        }

        yield return new WaitForSeconds(5.0f);

        if (SpeechBubble != null) SpeechBubble.SetActive(false);

        //빌지 띄우기
        if (dayEventUI != null)
        {
            dayEventUI.OrderedBread(MyOrder, wantsPackaging);
            Debug.Log($"[Customer] 대사가 끝나고 빌지가 팝업되었습니다. 빌지가 닫힐 때까지 대기합니다.");
        }

        if (dayEventUI != null && dayEventUI.OrderDetailPanel != null)
        {
            yield return new WaitUntil(() => dayEventUI.OrderDetailPanel.activeSelf == false);
        }
        Debug.Log($"[Customer] 빌지 확인 완료! 픽업대로 이동합니다.");

        Quaternion originalRotation = transform.rotation;
        Vector3 direction = (pickupPosition - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = transform.rotation;
        if (direction != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(direction);
        }

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation;
        while (Vector3.Distance(transform.position, pickupPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pickupPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = pickupPosition;
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, turnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = originalRotation;
    }

    public bool ReceiveBread(FinishedBread bread, bool isPackaged)
    {
        DE.SellBread(bread.MyBreadType);

        if (bread.MyBreadType != MyOrder || wantsPackaging != isPackaged)
        {
            Debug.LogWarning($"[Customer] 주문 불일치 : 빵 종류가 다릅니다.");

            if (DE != null)
            {
                DE.CustomerLeft(0);
            }
            Destroy(gameObject);
            return false;
        }

        Debug.Log($"[Customer] 손님에게 빵을 전달하였습니다.");
        if (DE != null)
        {
            int score = 0;

            switch (MyOrder)
            {
                case BreadType.DollCake: score = 1; break;
                case BreadType.MushroomMuffin: score = 2; break;
                case BreadType.SlimePudding: score = 3; break;
            }

            if (GameManager.Instance.CustomerCountPenaltyEvent)
            {
                score = 1;
                Debug.Log($"[Customer] 위생 패널티로 카운트가 1만 오릅니다.");
            }
            else if (GameManager.Instance.CustomerCountDoubleEvent)
            {
                score *= 2;
                Debug.Log($"[Customer] 위생 버프로 카운트가 2배로 적용 됩니다.");
            }

            DE.CustomerLeft(score);
            Destroy(gameObject);
        }

        return true;
    }
}
