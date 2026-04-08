using UnityEngine;

public class Ending : MonoBehaviour
{
    public GameObject GoodEnding;
    public GameObject BadEnding;
    public GameObject CleanEnding;

    void Start()
    {
        if (GoodEnding != null) GoodEnding.SetActive(false);
        if (BadEnding != null) BadEnding.SetActive(false);
        if (CleanEnding != null) CleanEnding.SetActive(false);
    }

    public void CheckGameEnding()
    {
        int totalScore = GameManager.Instance.CustomerScore;

        Debug.Log($"[엔딩 정산] 최종 누적 점수 : {totalScore}");

        if (totalScore >= 150)
        {
            if (GoodEnding != null) GoodEnding.SetActive(true);
        }
        else
        {
            if (BadEnding != null) BadEnding.SetActive(true);
        }
    }
}
