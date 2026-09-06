using System.Collections;
using UnityEngine;

// Attach to an oxygen canister GameObject.
// Restores oxygen when the player gets close enough. No trigger/tag setup required.
public class OxygenPickup : MonoBehaviour
{
    [Header("Oxygen")]
    public float oxygenAmount  = 30f;
    public float pickupRadius  = 5f;

    [Header("Audio")]
    public AudioClip pickupSound;
    [Range(0f, 1f)]
    public float     soundVolume = 1f;

    [Header("Respawn")]
    public bool  respawn       = false;
    public float respawnDelay  = 20f;

    bool         _collected;
    Transform    _player;

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    void Update()
    {
        if (_collected) return;
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= pickupRadius)
            StartCoroutine(Collect());
    }

    IEnumerator Collect()
    {
        _collected = true;

        OxygenSystem.Instance?.AddOxygen(oxygenAmount);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);

        // Scale pop
        Vector3 baseScale = transform.localScale;
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale, baseScale * 1.5f, elapsed / 0.2f);
            yield return null;
        }

        gameObject.SetActive(false);
        transform.localScale = baseScale;

        if (respawn)
        {
            yield return new WaitForSeconds(respawnDelay);
            gameObject.SetActive(true);
            _collected = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
