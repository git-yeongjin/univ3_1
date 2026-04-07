using UnityEngine;

public class SlimeProjectile : MonoBehaviour
{
    [Header("Settings")]
    // Drag your Slime Puddle Prefab here
    [SerializeField] private GameObject slimePuddlePrefab; 
    
    // This function is called when the projectile hits something
    void OnCollisionEnter(Collision collision)
    {
        // 1. Get contact point info
        ContactPoint contact = collision.contacts[0];

        // 2. Calculate rotation based on the floor's normal (angle)
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        // 3. Spawn the Puddle
        if (slimePuddlePrefab != null)
        {
            // Instantiate at the exact hit point with the calculated rotation
            GameObject puddle = Instantiate(slimePuddlePrefab, contact.point, spawnRotation);
            
            // Offset the position slightly to prevent Z-fighting (flickering) with the floor
            puddle.transform.position += contact.normal * 0.005f;
            
            // Auto-destroy the puddle after 10 seconds to save memory
            Destroy(puddle, 10f);
        }

        // 4. Destroy the projectile (the ball itself) on impact
        Destroy(gameObject);
    }
}