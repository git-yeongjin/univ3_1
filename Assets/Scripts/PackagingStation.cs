using UnityEngine;

public class PackagingStation : MonoBehaviour
{
    [Header("현재 대기 중인 손님")]
    [SerializeField]
    private Customer currentCustomer;

    public void AskPackaging(FinishedBread bread, string stationname)
    {
        Debug.Log($"[PackagingStation] [{bread.MyBreadType}] 빵을 [{stationname}] 구역에 올렸습니다.");

        if (currentCustomer == null)
        {
            Debug.LogWarning($"[PackagingStation] 대기 중인 손님이 없어 판매를 취소합니다.");
            return;
        }

        if (stationname == "포장")
        {
            Debug.Log("[PackagingStation] 포장으로 판매했습니다.");
            if (currentCustomer != null) currentCustomer.ReceiveBread(bread, true);
        }
        else if (stationname == "매장")
        {
            Debug.Log("[PackagingStation] 매장으로 판매했습니다");
            if (currentCustomer != null) currentCustomer.ReceiveBread(bread, false);
        }
        else
        {
            Debug.LogWarning($"[PackagingStation] 알 수 없는 구역 이름입니다: {stationname}");
        }
    }

    public void FindCustomer(Customer customer)
    {
        currentCustomer = customer;
        Debug.Log($"[PackagingStation] 손님을 찾는 중");
    }
}
