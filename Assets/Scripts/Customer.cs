using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("주문 정보")]
    public BreadType MyOrder;
    public bool wantsPackaging;

    private DayEvent DE;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        if (DE == null)
        {
            Debug.LogError($"[Customer] DayEvent를 찾을 수 없습니다.");
        }
    }

    public void SetOrder(BreadType order, bool packaging)
    {
        MyOrder = order;
        wantsPackaging = packaging;

        string packStr = wantsPackaging ? "포장" : "매장";
        Debug.Log($"[Customer] 현재 주문 : {MyOrder} / {packStr}");
    }

    public void ReceiveBread(FinishedBread bread, bool isPackaged)
    {
        if (bread.MyBreadType != MyOrder)
        {
            Debug.LogWarning($"[Customer] 주문 불일치 : 빵 종류가 다릅니다.");
            return;
        }
        if (wantsPackaging != isPackaged)
        {
            Debug.LogWarning($"[Customer] 포장 불일치 : 포장 여부가 다릅니다.");
            return;
        }

        Debug.Log($"[Customer] 손님에게 빵을 전달하였습니다.");
        if (DE == null) return;

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
}
