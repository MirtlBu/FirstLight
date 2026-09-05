using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win / Lose")]
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Win flash")]
    public Light      starLight;
    public float      winFlashDuration = 2f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (winScreen  != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);

        OxygenSystem.Instance.onDepleted.AddListener(OnOxygenDepleted);
    }

    public void OnStarFound()
    {
        Debug.Log("[FirstLight] *** STAR FOUND — YOU WIN ***");
        StartCoroutine(WinSequence());
    }

    public void OnOxygenDepleted()
    {
        Debug.Log("[FirstLight] *** OXYGEN DEPLETED — YOU LOSE ***");
        if (loseScreen != null) loseScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        if (OxygenSystem.Instance == null) return;
        float pct = OxygenSystem.Instance.NormalizedLeft * 100f;
        string label = $"O2: {pct:F0}%";
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;
        style.normal.textColor = pct > 50f ? Color.cyan : pct > 25f ? Color.yellow : Color.red;
        GUI.Label(new Rect(20, 20, 200, 40), label, style);

        if (PlayerSensor.Instance?.nearest != null)
        {
            float dist = PlayerSensor.Instance.nearest.normalizedDistance * PlayerSensor.Instance.nearest.maxRange;
            float temp = PlayerSensor.Instance.colorTemperature;
            GUI.Label(new Rect(20, 55, 400, 40), $"Nearest: {dist:F0}m  |  colorTemp: {temp:F2}", style);
        }
    }

    IEnumerator WinSequence()
    {
        // Flash the star light to full brightness
        if (starLight != null)
        {
            float elapsed = 0f;
            float startIntensity = starLight.intensity;
            while (elapsed < winFlashDuration)
            {
                starLight.intensity = Mathf.Lerp(startIntensity, 50f, elapsed / winFlashDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);
        if (winScreen != null) winScreen.SetActive(true);
    }
}
