using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera menuCamera;
    public Transform startPlanet;
    public Transform quitPlanet;
    public Transform[] decorativeObjects;

    [Header("Hover")]
    public float hoverScale = 1.03f;
    public float scaleSpeed = 8f;

    Transform _hoveredPlanet;
    Vector3 _startScale;
    Vector3 _quitScale;
    Vector3[] _decorativeScales;

    void Awake()
    {
        if (menuCamera == null)
            menuCamera = Camera.main;

        if (decorativeObjects == null)
            decorativeObjects = new Transform[0];

        if (startPlanet != null)
            _startScale = startPlanet.localScale;
        if (quitPlanet != null)
            _quitScale = quitPlanet.localScale;

        _decorativeScales = new Vector3[decorativeObjects != null ? decorativeObjects.Length : 0];
        for (int i = 0; i < _decorativeScales.Length; i++)
            if (decorativeObjects[i] != null)
                _decorativeScales[i] = decorativeObjects[i].localScale;
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
            else
                Debug.Log($"[FirstLight] Menu object selected: {_hoveredPlanet.name}");
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
                else
                {
                    for (int i = 0; i < decorativeObjects.Length; i++)
                    {
                        if (IsTarget(hit.transform, decorativeObjects[i]))
                        {
                            target = decorativeObjects[i];
                            break;
                        }
                    }
                }
            }
        }

        _hoveredPlanet = target;
        AnimateScale(startPlanet, _startScale, target == startPlanet);
        AnimateScale(quitPlanet, _quitScale, target == quitPlanet);

        for (int i = 0; i < _decorativeScales.Length; i++)
        {
            Transform decorativeObject = decorativeObjects[i];
            if (decorativeObject != null)
                AnimateScale(decorativeObject, _decorativeScales[i], target == decorativeObject);
        }
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
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("[FirstLight] Quit requested.");
    }
}
