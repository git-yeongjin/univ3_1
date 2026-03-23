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
        Destroy(gameObject);
    }

    public void Capture()
    {
        if (mySpawner != null)
        {
            mySpawner.ReportCapture();
        }
        Destroy(gameObject);
    }

    private void SetCreatureState()
    {
        switch (creatureData.type)
        {
            case CreatureType.none:
                Debug.LogError($"{gameObject.name}에 크리쳐 타입이 없음");
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
        Debug.Log($"{gameObject.name}의 크리쳐 타입 : {creatureData.type} 세팅 완료");
    }
}
