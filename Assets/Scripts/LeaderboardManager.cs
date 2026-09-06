using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public float  time;   // seconds survived
    public int    level;
    public string date;
}

[Serializable]
class LeaderboardData { public List<LeaderboardEntry> entries = new(); }

// Persistent singleton — survives scene loads.
// Carries the pending score from GameScene to GameOver scene.
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    public const int MaxEntries = 10;

    // Set by GameManager before loading GameOver scene
    public float PendingTime  { get; private set; }
    public int   PendingLevel { get; private set; }

    string _filePath;
    LeaderboardData _data;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        Load();
    }

    // Called by GameManager when the game ends
    public void SetPending(float survivalTime, int level)
    {
        PendingTime  = survivalTime;
        PendingLevel = level;
    }

    // Called by GameOverUI after the player enters their name
    public void SubmitPending(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";

        _data.entries.Add(new LeaderboardEntry
        {
            name  = playerName.Trim(),
            time  = PendingTime,
            level = PendingLevel,
            date  = DateTime.Now.ToString("yyyy-MM-dd")
        });

        _data.entries.Sort((a, b) => b.time.CompareTo(a.time));
        if (_data.entries.Count > MaxEntries)
            _data.entries.RemoveRange(MaxEntries, _data.entries.Count - MaxEntries);

        Save();
    }

    public List<LeaderboardEntry> GetEntries() => _data.entries;

    void Load()
    {
        if (File.Exists(_filePath))
        {
            try { _data = JsonUtility.FromJson<LeaderboardData>(File.ReadAllText(_filePath)); }
            catch { _data = new LeaderboardData(); }
        }
        else
        {
            _data = new LeaderboardData();
        }
    }

    void Save()
    {
        File.WriteAllText(_filePath, JsonUtility.ToJson(_data, true));
    }
}
