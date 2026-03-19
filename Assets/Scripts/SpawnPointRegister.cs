using UnityEngine;

public class SpawnPointRegister : MonoBehaviour
{
    void Start()
    {
        DayEvent DE = FindAnyObjectByType<DayEvent>();

        if (DE != null)
        {
            DE.CustomerSpawnPoint = this.transform;
        }
        else
        {
            Debug.Log($"DayEvent를 찾을 수 없습니다.");
        }
    }
}
