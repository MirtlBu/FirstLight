using UnityEngine;
using UnityEngine.EventSystems;

// Attach to any Button. Scales up the RectTransform on hover.
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float speed      = 8f;

    RectTransform _rect;
    Vector3       _baseScale;
    Vector3       _targetScale;

    void Awake()
    {
        _rect       = GetComponent<RectTransform>();
        _baseScale  = _rect.localScale;
        _targetScale = _baseScale;
    }

    void Update()
    {
        _rect.localScale = Vector3.Lerp(_rect.localScale, _targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData _) => _targetScale = _baseScale * hoverScale;
    public void OnPointerExit(PointerEventData _)  => _targetScale = _baseScale;
}
