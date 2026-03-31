using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header("크리쳐 세팅")]
    public CreatureData creatureData;

    private CreatureSpawner mySpawner;

    void OnValidate()
    {
        if (creatureData != null)
        {
            SetCreatureState();
        }
    }

    public void SetupSpawnInfo(CreatureSpawner spawner, float bonusAlertness)
    {
        mySpawner = spawner;
        //Alertness += bonusAlertness;
    }

    public void Escape()
    {
        if (mySpawner != null)
        {
            mySpawner.ReportEscape();
        }
        Debug.Log($"[Creature] {gameObject.name} 크리쳐가 도망갔습니다.");
        Destroy(gameObject);
    }

    public void Capture()
    {
        if (mySpawner != null)
        {
            mySpawner.ReportCapture();
        }
        if (GameManager.Instance != null && creatureData != null)
        {
            switch (creatureData.type)
            {
                case CreatureType.Doll:
                    GameManager.Instance.DollCakeCount++;
                    Debug.Log($"[Creature] 케이크 판매 갯수가 증가했습니다.");
                    break;
                case CreatureType.Mushroom:
                    GameManager.Instance.MushroomMuffinCount++;
                    Debug.Log($"[Creature] 머핀 판매 갯수가 증가했습니다.");
                    break;
                case CreatureType.SlimeHorse:
                    GameManager.Instance.SlimePuddingCount++;
                    Debug.Log($"[Creature] 푸딩 판매 갯수가 증가했습니다.");
                    break;
            }

            GameManager.Instance.IncreaseCustomer();
        }
        Destroy(gameObject);
    }

    private void SetCreatureState()
    {
        if (creatureData == null) return;

        switch (creatureData.type)
        {
            case CreatureType.none:
                Debug.LogError($"[Creature] {gameObject.name}에 크리쳐 타입이 없음");
                break;
            case CreatureType.Doll:
                break;
            case CreatureType.Frog:
                break;
            case CreatureType.Mushroom:
                break;
            case CreatureType.SlimeHorse:
                break;
            case CreatureType.Mammoth:
                break;
            default:
                return;
        }
        //Debug.Log($"{gameObject.name}의 크리쳐 타입 : {creatureData.type} 세팅 완료");
    }
}
