using UnityEngine;

public class Customer : MonoBehaviour
{
    public BreadType MyOrder;
    public bool wantsPackaging;

    private DayEvent DE;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
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
                    int scroe = 0;
                    switch (MyOrder)
                    {
                        case BreadType.DollCake:
                            scroe = 1;
                            break;
                        case BreadType.MushroomMuffin:
                            scroe = 2;
                            break;
                        case BreadType.SlimePudding:
                            scroe = 3;
                            break;
                    }
                    DE.CustomerLeft(scroe);

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
