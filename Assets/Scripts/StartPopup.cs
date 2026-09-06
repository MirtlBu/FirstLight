using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to any GameObject. Shows an intro popup at game start.
// Assign popupPanel in Inspector. Player clicks a button or presses any key to dismiss.
public class StartPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public Button     continueButton;

    [TextArea(5, 15)]
    public string introText =
        "Your ship has crashed in deep space.\n\n" +
        "You must hold out until rescue arrives.\n\n" +
        "Search the wreckage for oxygen canisters to recharge your suit's life support system.\n\n" +
        "P.S. Beware of black holes.";

    public TMP_Text bodyText;

    [Header("Animation")]
    public float startDelay   = 0.3f;
    public float animDuration = 0.5f;

    RectTransform _popupRect;
    bool          _dismissing;

    void Start()
    {
        if (bodyText != null)
            bodyText.text = introText;

        if (popupPanel != null)
        {
            _popupRect = popupPanel.GetComponent<RectTransform>();
            popupPanel.SetActive(true);
            if (_popupRect != null)
                _popupRect.localScale = Vector3.zero;
            StartCoroutine(AnimateIn());
        }

        Time.timeScale = 0f;

        if (continueButton != null)
            continueButton.onClick.AddListener(Dismiss);
    }

    void Update()
    {
        if (!_dismissing && popupPanel != null && popupPanel.activeSelf)
        {
            if (Input.anyKeyDown)
                Dismiss();
        }
    }

    // EaseOutBack: overshoots then settles
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    // EaseInBack: pulls back before collapsing
    float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return c3 * t * t * t - c1 * t * t;
    }

    IEnumerator AnimateIn()
    {
        // Wait a bit so scene has settled before popping up
        float delay = startDelay;
        while (delay > 0f) { delay -= Time.unscaledDeltaTime; yield return null; }

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            if (_popupRect != null)
                _popupRect.localScale = Vector3.one * EaseOutBack(t);
            yield return null;
        }
        if (_popupRect != null)
            _popupRect.localScale = Vector3.one;
    }

    IEnumerator AnimateOut()
    {
        float elapsed = 0f;
        float outDuration = animDuration * 0.7f;
        while (elapsed < outDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / outDuration);
            if (_popupRect != null)
                _popupRect.localScale = Vector3.one * (1f - EaseInBack(t));
            yield return null;
        }

        Time.timeScale = 1f;
        if (popupPanel != null) Destroy(popupPanel);
        Destroy(gameObject);
    }

    public void Dismiss()
    {
        if (_dismissing) return;
        _dismissing = true;
        StartCoroutine(AnimateOut());
    }
}
