using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Add to any GameObject in MainMenu scene.
// Finds buttons by name and wires OnClick automatically.
public class MainMenuButtons : MonoBehaviour
{
    [Header("Credits Panel (assign in Inspector)")]
    public GameObject creditsPanel;
    public Button     closeCreditsButton;

    void Awake()
    {
        Wire("Play",    OnPlay);
        Wire("Exit",    OnExit);
        Wire("Credits", OnCredits);

        if (closeCreditsButton != null)
            closeCreditsButton.onClick.AddListener(OnCloseCredits);
    }

    void OnPlay()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("GameScene");
    }

    void OnExit()
    {
        Application.Quit();
    }

    void OnCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    void OnCloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    void Wire(string buttonName, UnityEngine.Events.UnityAction action)
    {
        var go = GameObject.Find(buttonName);
        if (go == null) { Debug.LogWarning($"[MainMenuButtons] Button '{buttonName}' not found."); return; }
        var btn = go.GetComponent<Button>();
        if (btn == null) { Debug.LogWarning($"[MainMenuButtons] No Button component on '{buttonName}'."); return; }
        btn.onClick.AddListener(action);
    }
}
