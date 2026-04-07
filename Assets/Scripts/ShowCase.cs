using UnityEngine;

public class ShowCase : MonoBehaviour
{
    private FinishedBread MyBread;

    public BreadType MyBreadType;

    [Header("판매할 음식")]
    public GameObject[] Breads;


    void Start()
    {
        foreach (GameObject bread in Breads)
        {
            if (bread != null) bread.SetActive(false);
        }
    }

    public void DisplayBread()
    {
        if (Breads == null || Breads.Length == 0) return;

        int currentCount = 0;
        switch (MyBreadType)
        {
            case BreadType.DollCake:
                currentCount = GameManager.Instance.DollCakeCount;
                break;
            case BreadType.MushroomMuffin:
                currentCount = GameManager.Instance.MushroomMuffinCount;
                break;
            case BreadType.SlimePudding:
                currentCount = GameManager.Instance.SlimePuddingCount;
                break;
        }

        int displayAmount = GetDisplayAmount(currentCount);

        for (int i = 0; i < Breads.Length; i++)
        {
            if (Breads[i] != null)
            {
                Breads[i].SetActive(i < displayAmount);
            }
        }

        Debug.Log($"[ShowCase] [{MyBreadType}] 재고: {currentCount}개 -> 진열된 빵: {displayAmount}개");
    }

    private int GetDisplayAmount(int count)
    {
        if (count >= 8) return 5;
        else if (count >= 6) return 4;
        else if (count >= 4) return 3;
        else if (count >= 2) return 2;
        else if (count >= 1) return 1;
        else return 0;
    }
}
