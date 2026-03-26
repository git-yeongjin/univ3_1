using UnityEngine;

public class Customer : MonoBehaviour
{
    public BreadType MyOrder;
    public bool wantsPackaging;

    private DayEvent DE;
    private GameManager GM;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
        GM = FindAnyObjectByType<GameManager>();
    }

    public void SetOrder(BreadType order, bool packaging)
    {
        MyOrder = order;
        wantsPackaging = packaging;

        string packStr = wantsPackaging ? "포장" : "매장";
        Debug.Log($"현재 주문 : {MyOrder} / {packStr}");
    }

    public void ReceiveBread(FinishedBread bread, bool isPackaged)
    {
        if (bread.MyBreadType == MyOrder)
        {
            if (wantsPackaging == isPackaged)
            {
                Debug.Log($"주문한 빵과 일치합니다.");
                if (DE != null)
                {
                    int score = 0;
                    switch (MyOrder)
                    {
                        case BreadType.DollCake:
                            score = 1;
                            break;
                        case BreadType.MushroomMuffin:
                            score = 2;
                            break;
                        case BreadType.SlimePudding:
                            score = 3;
                            break;
                    }

                    if (GM != null)
                    {
                        if (GM.CustomerCountPenaltyEvent)
                        {
                            score = 1;
                            Debug.Log("위생 패널티로 카운트가 1만 오릅니다.");
                        }
                        else if (GM.CustomerCountDoubleEvent)
                        {
                            score *= 2;
                            Debug.Log("위생 우수로 카운트가 2배로 적용 됩니다.");
                        }
                    }

                    DE.CustomerLeft(score);
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("포장 불일치");
            }
        }
        else
        {
            Debug.Log($"주문 불일치");
        }

    }
}
