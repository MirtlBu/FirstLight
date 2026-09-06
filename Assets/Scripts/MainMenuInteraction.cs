using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera menuCamera;
    public Transform startPlanet;
    public Transform quitPlanet;

    [Header("Credits")]
    public GameObject creditsPanel;

    [Header("Hover")]
    public float hoverScale = 1.03f;
    public float scaleSpeed = 8f;

    Transform _hoveredPlanet;
    Vector3 _startScale;
    Vector3 _quitScale;

    void Awake()
    {
        if (menuCamera == null)
            menuCamera = Camera.main;

        if (startPlanet != null)
            _startScale = startPlanet.localScale;
        if (quitPlanet != null)
            _quitScale = quitPlanet.localScale;
    }

    void Update()
    {
        UpdateHover();

        if (Input.GetMouseButtonDown(0) && _hoveredPlanet != null)
        {
            if (_hoveredPlanet == startPlanet)
                StartGame();
            else if (_hoveredPlanet == quitPlanet)
                QuitGame();
        }
    }

    void UpdateHover()
    {
        Transform target = null;

        if (menuCamera != null)
        {
            Ray ray = menuCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (IsTarget(hit.transform, startPlanet))
                    target = startPlanet;
                else if (IsTarget(hit.transform, quitPlanet))
                    target = quitPlanet;
            }
        }

        _hoveredPlanet = target;
        AnimateScale(startPlanet, _startScale, target == startPlanet);
        AnimateScale(quitPlanet, _quitScale, target == quitPlanet);
    }

    bool IsTarget(Transform hitTransform, Transform target)
    {
        return target != null && (hitTransform == target || hitTransform.IsChildOf(target));
    }

    void AnimateScale(Transform planet, Vector3 baseScale, bool hovered)
    {
        if (planet == null) return;

        Vector3 targetScale = hovered ? baseScale * hoverScale : baseScale;
        planet.localScale = Vector3.Lerp(planet.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void StartGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("[FirstLight] Quit requested.");
    }

    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
