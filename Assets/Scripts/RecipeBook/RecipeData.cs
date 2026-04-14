using UnityEngine;
using System.Collections.Generic;

public enum ResultBread
{
    None,
    DollCake,
    MushroomMuffin,
    SlimePudding,

    Rollcake
}

[CreateAssetMenu(fileName = "Bread Recipe", menuName = "Bakery/Recipe")]
public class RecipeData : ScriptableObject
{
    public ResultBread Result;

    //빵 이름
    public string BreadName;
    //빵 재료
    public List<string> BreadMaterial;
    //빵 굽는 시간
    public float PerfectBakeTime;
}
