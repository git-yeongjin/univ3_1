using UnityEngine;

public class Gizmo : MonoBehaviour
{
    public Color color = Color.yellow;
    public float Radius = 1f;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
