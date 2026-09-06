using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to any GameObject in MainMenu scene (e.g. Main Camera or a Manager).
// Handles click and hover for 3D quad buttons (StartLabel, QuitLabel).
public class MainMenuButtons : MonoBehaviour
{
    [Header("Collider objects (for click detection)")]
    public string startButtonName  = "StartPlanet";
    public string quitButtonName   = "QuitPlanet";

    [Header("Hover")]
    public float hoverScale  = 1.1f;
    public float hoverSpeed  = 8f;

    Camera     _cam;
    Transform  _startBtn;
    Transform  _quitBtn;
    Transform  _hovered;

    Vector3    _startBaseScale;
    Vector3    _quitBaseScale;

    void Start()
    {
        _cam = Camera.main;

        var s = GameObject.Find(startButtonName);
        var q = GameObject.Find(quitButtonName);

        if (s != null) { _startBtn = s.transform; _startBaseScale = _startBtn.localScale; }
        else Debug.LogWarning($"[MainMenuButtons] '{startButtonName}' not found.");

        if (q != null) { _quitBtn  = q.transform; _quitBaseScale  = _quitBtn.localScale; }
        else Debug.LogWarning($"[MainMenuButtons] '{quitButtonName}' not found.");
    }

    void Update()
    {
        UpdateHover();

        if (Input.GetMouseButtonDown(0) && _hovered != null)
        {
            if (_hovered == _startBtn) OnStart();
            else if (_hovered == _quitBtn) OnQuit();
        }
    }

    void UpdateHover()
    {
        Transform hit = Raycast();

        AnimateScale(_startBtn, _startBaseScale, hit == _startBtn);
        AnimateScale(_quitBtn,  _quitBaseScale,  hit == _quitBtn);

        _hovered = hit;
    }

    Transform Raycast()
    {
        if (_cam == null) return null;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return null;

        Transform t = hitInfo.transform;
        if (_startBtn != null && (t == _startBtn || t.IsChildOf(_startBtn))) return _startBtn;
        if (_quitBtn  != null && (t == _quitBtn  || t.IsChildOf(_quitBtn)))  return _quitBtn;
        return null;
    }

    void AnimateScale(Transform btn, Vector3 baseScale, bool hovered)
    {
        if (btn == null) return;
        Vector3 target = hovered ? baseScale * hoverScale : baseScale;
        btn.localScale = Vector3.Lerp(btn.localScale, target, Time.deltaTime * hoverSpeed);
    }

    void OnStart()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("GameScene");
    }

    void OnQuit()
    {
        Application.Quit();
    }
}
