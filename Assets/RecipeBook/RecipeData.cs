using UnityEngine;
using System.Collections.Generic;


public enum MoldCategory
{
    None,
    Cake,
    Pudding,
    Muffin
}
[CreateAssetMenu(fileName = "Bread Recipe", menuName = "Bakery/Recipe")]
public class RecipeData : ScriptableObject
{
    public MoldCategory moldCategory;
    //빵 이름
    public string BreadName;
    //빵 재료
    public List<string> BreadMaterial;
    //필요한 틀
    public Mold RequiredMold;
    //빵 굽는 시간
    public float PerfectBakeTime;
    //굽는 시간 오차범위
    public float ErrorMargin = 1.0f;
}
