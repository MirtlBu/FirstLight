using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Attach to any GameObject in the GameOver scene.
// Flow:
//   1. Name input popup animates in on load
//   2. Player enters name, clicks OK
//   3. Popup animates out → GAME OVER title + leaderboard revealed
public class GameOverUI : MonoBehaviour
{
    [Header("Name Popup")]
    public GameObject     popup;           // the popup panel RectTransform
    public TMP_InputField nameInput;
    public Button         okButton;

    [Header("Main Screen (hidden until popup closes)")]
    public GameObject     mainScreen;      // parent of title + leaderboard + menu button
    public TMP_Text       survivalTimeText;
    public Button         menuButton;

    [Header("Leaderboard")]
    public ScrollRect     scrollRect;
    public Transform      leaderboardContainer;
    public GameObject     rowPrefab;       // Row with children: Rank, Name, Time

    [Header("Animation")]
    public float animDuration = 0.45f;

    RectTransform _popupRect;
    bool          _dismissing;

    void Start()
    {
        // Hide main screen until popup is dismissed
        if (mainScreen != null) mainScreen.SetActive(false);

        // Show survival time
        float t = LeaderboardManager.Instance?.PendingTime ?? 0f;
        if (survivalTimeText != null)
            survivalTimeText.text = $"You survived {FormatTime(t)}";

        // Setup popup
        if (popup != null)
        {
            _popupRect = popup.GetComponent<RectTransform>();
            popup.SetActive(true);
            if (_popupRect != null) _popupRect.localScale = Vector3.zero;
            StartCoroutine(AnimateIn());
        }

        if (okButton   != null) okButton.onClick.AddListener(OnOK);
        if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
    }

    void OnOK()
    {
        if (_dismissing) return;
        _dismissing = true;

        string playerName = nameInput != null ? nameInput.text : "";
        LeaderboardManager.Instance?.SubmitPending(playerName);

        StartCoroutine(AnimateOut());
    }

    void ShowMainScreen()
    {
        if (mainScreen != null) mainScreen.SetActive(true);
        RefreshLeaderboard();
    }

    void RefreshLeaderboard()
    {
        if (leaderboardContainer == null || rowPrefab == null) return;

        foreach (Transform child in leaderboardContainer)
            Destroy(child.gameObject);

        var entries = LeaderboardManager.Instance?.GetEntries()
                      ?? new System.Collections.Generic.List<LeaderboardEntry>();

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var row   = Instantiate(rowPrefab, leaderboardContainer);
            SetText(row, "Rank",  $"{i + 1}.");
            SetText(row, "Name",  entry.name);
            SetText(row, "Time",  FormatTime(entry.time));
        }

        // Scroll to top
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    void SetText(GameObject row, string childName, string value)
    {
        var t = row.transform.Find(childName);
        if (t == null) return;
        var tmp = t.GetComponent<TMP_Text>();
        if (tmp != null) tmp.text = value;
    }

    string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60);
        int s = (int)(seconds % 60);
        return m > 0 ? $"{m}m {s:D2}s" : $"{s}s";
    }

    void OnMenu() => SceneManager.LoadScene("MainMenu");

    // ── Animations (same easing as StartPopup) ─────────────────────────────

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    float EaseInBack(float t)
    {
        float c1 = 1.70158f, c3 = c1 + 1f;
        return c3 * t * t * t - c1 * t * t;
    }

    IEnumerator AnimateIn()
    {
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            if (_popupRect != null)
                _popupRect.localScale = Vector3.one * EaseOutBack(t);
            yield return null;
        }
        if (_popupRect != null) _popupRect.localScale = Vector3.one;
    }

    IEnumerator AnimateOut()
    {
        float elapsed  = 0f;
        float duration = animDuration * 0.7f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (_popupRect != null)
                _popupRect.localScale = Vector3.one * (1f - EaseInBack(t));
            yield return null;
        }

        if (popup != null) popup.SetActive(false);
        ShowMainScreen();
    }
}
