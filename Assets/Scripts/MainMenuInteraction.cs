using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera menuCamera;
    public Transform startPlanet;
    public Transform quitPlanet;

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
                if (startPlanet != null && hit.transform.IsChildOf(startPlanet))
                    target = startPlanet;
                else if (quitPlanet != null && hit.transform.IsChildOf(quitPlanet))
                    target = quitPlanet;
            }
        }

        _hoveredPlanet = target;
        AnimateScale(startPlanet, _startScale, target == startPlanet);
        AnimateScale(quitPlanet, _quitScale, target == quitPlanet);
    }

    void AnimateScale(Transform planet, Vector3 baseScale, bool hovered)
    {
        if (planet == null) return;

        Vector3 targetScale = hovered ? baseScale * hoverScale : baseScale;
        planet.localScale = Vector3.Lerp(planet.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("[FirstLight] Quit requested.");
    }
}
