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

    float _startTime;
    int   _currentLevel = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (winScreen  != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);

        _startTime = Time.time;
        OxygenSystem.Instance.onDepleted.AddListener(OnOxygenDepleted);
    }

    void GoToGameOver()
    {
        float survived = Time.time - _startTime;
        LeaderboardManager.Instance?.SetPending(survived, _currentLevel);
        SceneManager.LoadScene("GameOver");
    }

    public void OnStarFound()
    {
        Debug.Log("[FirstLight] *** STAR FOUND — YOU WIN ***");
        StartCoroutine(WinSequence());
    }

    public void OnOxygenDepleted()
    {
        Debug.Log("[FirstLight] *** OXYGEN DEPLETED — YOU LOSE ***");
        GoToGameOver();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
