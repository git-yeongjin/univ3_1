using UnityEngine;

public class BreadMaterial : MonoBehaviour
{
    public enum BreadMaterialName
    {
        none,
        Eye,
        Jam,
    }

    public BreadMaterialName Name;

    public string GetMaterialName()
    {
        return Name.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("dddd");
    }
}
