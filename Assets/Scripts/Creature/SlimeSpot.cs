using UnityEngine;

public class SlimeSpot : MonoBehaviour
{
    public float SlowValue = 5.0f;

    private static int ActiveSpotCount = 0;
    private bool isPlayerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            ActiveSpotCount++;
            if (ActiveSpotCount == 1)
            {
                NightEventPlayer nightEventPlayer = FindAnyObjectByType<NightEventPlayer>();
                if (nightEventPlayer != null) nightEventPlayer.MoveSpeed -= SlowValue;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
        ActiveSpotCount--;
        if (ActiveSpotCount <= 0)
        {
            ActiveSpotCount = 0;
            if (other.CompareTag("Player"))
            {
                NightEventPlayer nightEventPlayer = FindAnyObjectByType<NightEventPlayer>();
                if (nightEventPlayer != null) nightEventPlayer.MoveSpeed += SlowValue;
            }
        }
    }

    void OnDestroy()
    {
        if (isPlayerInside)
        {
            ActiveSpotCount--;
            if (ActiveSpotCount <= 0)
            {
                ActiveSpotCount = 0;

                NightEventPlayer nightEventPlayer = FindAnyObjectByType<NightEventPlayer>();
                if (nightEventPlayer != null)
                {
                    nightEventPlayer.MoveSpeed += SlowValue;
                }
            }
        }
    }
}
