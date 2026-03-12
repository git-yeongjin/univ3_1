using UnityEngine;

public enum Mold
{
    none,
    Cake,
    Pudding,
    Muffin
}

public class MoldType : MonoBehaviour
{
    public Mold CurrentMold;

    public Mold GetMoldName()
    {
        return CurrentMold;
    }
}
