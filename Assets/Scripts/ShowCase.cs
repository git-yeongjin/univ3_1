using UnityEngine;

public class ShowCase : MonoBehaviour
{
    private GameManager GM;
    private FinishedBread MyBread;

    [Header("판매할 음식")]
    public GameObject Bread;


    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null)
        {
            Debug.LogError("GM을 찾지 못했습니다.");
            return;
        }

        if (Bread == null)
        {
            Debug.LogError("[Showcase] Bread 오브젝트를 찾지 못함");
            return;
        }

        MyBread = Bread.GetComponent<FinishedBread>();
        if (MyBread == null)
        {
            Debug.LogError("[Showcase] FinishedBread를 찾지 못함");
            return;
        }

        Bread.SetActive(false);
    }

    public void DisplayBread()
    {
        if (GM == null || MyBread == null) return;

        switch (MyBread.MyBreadType)
        {
            case BreadType.DollCake:
                Bread.SetActive(GM.DollCake);
                break;
            case BreadType.MushroomMuffin:
                Bread.SetActive(GM.MushroomMuffin);
                break;
            case BreadType.SlimePudding:
                Bread.SetActive(GM.SlimePudding);
                break;
        }

        Debug.Log($"빵을 진열 했습니다.");
    }

}
