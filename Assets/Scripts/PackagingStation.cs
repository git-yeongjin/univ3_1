using UnityEngine;

public class PackagingStation : MonoBehaviour
{
    private DayEvent DE;
    [SerializeField]
    private Customer currentCustomer;
    private FinishedBread currentBread;

    void Start()
    {
        DE = FindAnyObjectByType<DayEvent>();
    }

    public void AskPackaging(FinishedBread bread, string stationname)
    {
        currentCustomer = FindAnyObjectByType<Customer>();
        Debug.Log($"[{bread.MyBreadType}]빵을 [{stationname}]에 올렸습니다.");

        currentBread = bread;
        DayEventUI dayEventUI = FindAnyObjectByType<DayEventUI>();

        if (currentCustomer != null && dayEventUI != null)
        {
            if (stationname == "포장")
            {
                Debug.Log("포장으로 판매했습니다.");
                if (currentCustomer != null) currentCustomer.ReceiveBread(currentBread, true);
            }
            else if (stationname == "매장")
            {
                Debug.Log("매장으로 판매했습니다");
                if (currentCustomer != null) currentCustomer.ReceiveBread(currentBread, false);
            }
        }
        else
        {
            Debug.LogWarning("손님이 없어서 판매을 취소합니다.");
        }
    }

    public void FindCustomer(Customer customer)
    {
        currentCustomer = customer;
        Debug.Log($"[PackagingStation] 손님을 찾는 중");
    }
}
