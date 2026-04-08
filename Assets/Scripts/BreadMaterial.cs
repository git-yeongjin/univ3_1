using UnityEngine;

public class BreadMaterial : MonoBehaviour
{
    public enum BreadMaterialName
    {
        None,

        ButtonChocolate,
        RibbonCandy,
        CreamCotton,
        NeedleSpringcle,

        SporePowder,
        MushroomCream,
        Chocoball,

        SlimeGelatin,
        BrainFruit
    }

    public BreadMaterialName Material;

    public string GetMaterialName()
    {
        return Material.ToString();
    }
}
