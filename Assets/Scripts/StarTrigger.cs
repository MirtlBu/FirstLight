using UnityEngine;
using UnityEngine.Events;

// Attach to the real star. When player enters, trigger win sequence.
public class StarTrigger : MonoBehaviour
{
    public float       triggerRadius = 5f;
    public UnityEvent  onFound;

    bool _triggered;

    void Update()
    {
        if (_triggered) return;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.transform.position) <= triggerRadius)
        {
            _triggered = true;
            onFound?.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
