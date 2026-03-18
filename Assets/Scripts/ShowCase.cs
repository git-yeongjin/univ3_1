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
        }

        if (!GM.DollCake)
        {
            Cake.SetActive(false);
        }
        if (!GM.MushroomMuffin)
        {
            Muffin.SetActive(false);
        }
        if (!GM.SlimePudding)
        {
            Pudding.SetActive(false);
        }
    }

    void Update()
    {

    }
}
