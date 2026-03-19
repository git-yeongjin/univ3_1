using UnityEngine;

public class ShowCase : MonoBehaviour
{
    private GameManager GM;

    [Header("판매할 음식")]
    public GameObject Cake;
    public GameObject Muffin;
    public GameObject Pudding;


    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        if (GM == null)
        {
            Debug.LogError("GM을 찾지 못했습니다.");
            return;
        }

        if (Cake != null) Cake.SetActive(GM.DollCake);
        if (Muffin != null) Muffin.SetActive(GM.MushroomMuffin);
        if (Pudding != null) Pudding.SetActive(GM.SlimePudding);
    }
}
