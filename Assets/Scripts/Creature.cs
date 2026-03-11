using UnityEngine;

public class Creature : MonoBehaviour
{
    public enum CreatureType
    {
        none,
        Doll,
        Frog,
        Mushroom,
        SlimeHorse,
        Mammoth
    }
    [Header("크리쳐 세팅")]
    public CreatureType creatureType;
    public int Hp;

    [Header("인형")]
    //현재 호감도
    public float CurrentAffection;
    //목표 호감도
    public float MaxAffection;
    //현재 불안도
    public float CurrentAnxiety;
    //최대 불안도 -> 넘기면 포획 실패
    public float MaxAnxiety;

    void Start()
    {

    }

    void OnValidate()
    {
        SetCretureState();
    }

    void Update()
    {

    }

    private void SetCretureState()
    {
        switch (creatureType)
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
        Debug.Log($"{gameObject.name}의 크리쳐 타입 : {creatureType}");
    }
}
