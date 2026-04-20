using UnityEngine;

public class PackagingStation : MonoBehaviour
{
    [Header("사운드")]
    public AudioClip DineInSound;
    public AudioClip TakeOutSound;

    [Header("현재 대기 중인 손님")]
    [SerializeField]
    private Customer currentCustomer;

    public bool AskPackaging(FinishedBread bread, string stationname)
    {
        Debug.Log($"[PackagingStation] [{bread.MyBreadType}] 빵을 [{stationname}] 구역에 올렸습니다.");
        currentCustomer = FindAnyObjectByType<Customer>();

        if (currentCustomer == null)
        {
            Debug.LogWarning($"[PackagingStation] 대기 중인 손님이 없어 판매를 취소합니다.");
            return false;
        }

        if (stationname == "tray2")
        {
            if (SoundManager.Instance != null && TakeOutSound != null)
            {
                SoundManager.Instance.PlaySFX(TakeOutSound);
            }

            Debug.Log("[PackagingStation] 포장으로 판매했습니다.");
            return currentCustomer.ReceiveBread(bread, true);
        }
        else if (stationname == "tray1")
        {
            if (SoundManager.Instance != null && DineInSound != null)
            {
                SoundManager.Instance.PlaySFX(DineInSound);
            }

            Debug.Log("[PackagingStation] 매장으로 판매했습니다");
            return currentCustomer.ReceiveBread(bread, false);
        }

        return false;
    }

    public void FindCustomer(Customer customer)
    {
        currentCustomer = customer;
        Debug.Log($"[PackagingStation] 손님을 찾는 중");
    }
}
