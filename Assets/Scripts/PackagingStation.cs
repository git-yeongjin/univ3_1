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
        currentCustomer = FindAnyObjectByType<Customer>();
    }

    public void AskPackaging(FinishedBread bread)
    {
        Debug.Log($"포장대에 [{bread.MyBreadType}]빵이 올라갔습니다.");

        currentBread = bread;
        DayEventUI dayEventUI = FindAnyObjectByType<DayEventUI>();

        if (currentCustomer != null && dayEventUI != null)
        {
            dayEventUI.ShowPackagingUI(this);
        }
        else
        {
            Debug.LogWarning("손님이 없어서 포장을 취소합니다.");
        }
    }

    public void ProceedPackaging()
    {
        Debug.Log("포장해서 먹는 중");
        if (currentCustomer != null)
        {
            currentCustomer.ReceiveBread(currentBread, true);
        }
    }

    public void CancelPackaging()
    {
        Debug.Log("매장에서 먹는 중");
        if (currentCustomer != null)
        {
            currentCustomer.ReceiveBread(currentBread, false);
        }
    }

    public void FindCustomer(Customer customer)
    {
        currentCustomer = customer;
        Debug.Log($"[PackagingStation] 손님을 찾는 중");
    }
}
