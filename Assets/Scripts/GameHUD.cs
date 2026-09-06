using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to any GameObject in GameScene.
// Displays oxygen bar + distance to nearest signal emitter.
public class GameHUD : MonoBehaviour
{
    [Header("Oxygen")]
    public Slider    oxygenBar;
    public TMP_Text  oxygenLabel;   // e.g. "O2  74%"

    static readonly Color _colorNormal = Color.white;
    static readonly Color _colorWarn   = Color.yellow;
    static readonly Color _colorDanger = Color.red;

    Image _fillImage;

    [Header("Distance")]
    public TMP_Text  distanceLabel; // e.g. "SIGNAL  142 m"
    public TMP_Text  signalName;    // name of nearest emitter (optional)

    void Awake()
    {
        if (oxygenBar != null)
            _fillImage = oxygenBar.fillRect?.GetComponent<Image>();
    }

    void Update()
    {
        UpdateOxygen();
        UpdateDistance();
    }

    void UpdateOxygen()
    {
        if (OxygenSystem.Instance == null) return;

        float n = OxygenSystem.Instance.NormalizedLeft;

        if (oxygenBar != null)
            oxygenBar.value = n;

        if (_fillImage != null)
        {
            Color c = n <= 0.15f ? _colorDanger : n <= 0.30f ? _colorWarn : _colorNormal;
            _fillImage.color = c;
        }

        if (oxygenLabel != null)
            oxygenLabel.text = $"O2  {Mathf.CeilToInt(n * 100)}%";
    }

    void UpdateDistance()
    {
        if (PlayerSensor.Instance == null || PlayerSensor.Instance.nearest == null)
        {
            if (distanceLabel != null) distanceLabel.text = "";
            if (signalName   != null) signalName.text    = "";
            return;
        }

        var    emitter = PlayerSensor.Instance.nearest;
        float  dist    = emitter.normalizedDistance * emitter.maxRange;

        if (distanceLabel != null)
            distanceLabel.text = $"SIGNAL  {Mathf.RoundToInt(dist)} m";

        if (signalName != null)
            signalName.text = emitter.gameObject.name.ToUpper();
    }
}
