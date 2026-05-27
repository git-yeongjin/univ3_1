using UnityEngine;

public class BakeEvent : MonoBehaviour
{
    [Header("빵 재료 모음")]
    public GameObject RollCakeObj;
    public GameObject DollCakeObj;
    public GameObject MushroomMuffinObj;
    public GameObject SlimePuddingObj;

    private int CurrentDayCount;

    void Start()
    {
        if (RollCakeObj != null) RollCakeObj.SetActive(false);
        if (DollCakeObj != null) DollCakeObj.SetActive(false);
        if (MushroomMuffinObj != null) MushroomMuffinObj.SetActive(false);
        if (SlimePuddingObj != null) SlimePuddingObj.SetActive(false);

        CurrentDayCount = GameManager.Instance.DayCount;

        switch (CurrentDayCount)
        {
            case 0:
                if (RollCakeObj != null) RollCakeObj.SetActive(true);
                break;
            case 2:
                if (DollCakeObj != null) DollCakeObj.SetActive(true);
                break;
            case 6:
                if (MushroomMuffinObj != null) MushroomMuffinObj.SetActive(true);
                break;
            case 12:
                if (SlimePuddingObj != null) SlimePuddingObj.SetActive(true);
                break;
        }
    }
}
