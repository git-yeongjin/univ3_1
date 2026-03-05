using UnityEngine;

public class Creture : MonoBehaviour
{
    public enum CretureType
    {
        none,
        c1,
        c2,
        c3,
        c4
    }
    [Header("크리쳐 세팅")]
    public CretureType cretureType;
    public int Hp;

    void Start()
    {
        SetCretureState();
    }


    void Update()
    {

    }

    private void SetCretureState()
    {
        switch (cretureType)
        {
            case CretureType.none:
                Debug.LogError($"{gameObject.name}에 크리쳐 타입이 없음");
                break;
            case CretureType.c1:
                break;
            case CretureType.c2:
                break;
            case CretureType.c3:
                break;
            case CretureType.c4:
                break;
            default:

                return;
        }
        Debug.Log($"{gameObject.name}의 크리쳐 타입 : {cretureType}");
    }
}
