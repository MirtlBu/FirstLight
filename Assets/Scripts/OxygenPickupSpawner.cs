using UnityEngine;

// Attach to any GameObject. Spawns oxygen pickup prefabs at random positions in a sphere.
public class OxygenPickupSpawner : MonoBehaviour
{
    public GameObject pickupPrefab;

    [Header("Spawn Area")]
    public int   count      = 10;
    public float radius     = 200f;   // spawn within this sphere radius from this object
    public float minRadius  = 20f;    // minimum distance from center (avoid spawning too close)

    void Start()
    {
        if (pickupPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = RandomPointInShell(minRadius, radius);
            Instantiate(pickupPrefab, transform.position + pos, Random.rotation);
        }
    }

    Vector3 RandomPointInShell(float min, float max)
    {
        Vector3 dir = Random.onUnitSphere;
        float dist  = Random.Range(min, max);
        return dir * dist;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.15f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, minRadius);
    }
}
