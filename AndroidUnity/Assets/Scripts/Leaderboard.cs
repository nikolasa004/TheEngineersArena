using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    // Existing Unity Inspector references remain valid.
    public List<TextMeshProUGUI> playerNames;
    public List<Image> levelOrbs;
    public List<TextMeshProUGUI> levels;
    public TextMeshProUGUI errorText;
    public Animator animator;

    // Menu.cs still uses these fields when the player changes their local name.
    public static bool nameChanged = false;
    public static string oldName;

    private readonly List<PlayerScore> scores = new List<PlayerScore>();

    private readonly Color paper = new Color(1f, 1f, 1f);
    private readonly Color bronze = new Color(0.8f, 0.5f, 0.2f);
    private readonly Color silver = new Color(0.8f, 0.8f, 0.8f);
    private readonly Color gold = new Color(1f, 0.8f, 0f);
    private readonly Color diamond = new Color(0f, 1f, 1f);
    private readonly Color hell = new Color(0.8f, 0f, 0f);

    private async void Start()
    {
        // Display empty placeholders immediately; a network connection is optional.
        UpdateTexts();

        try
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status != DependencyStatus.Available)
            {
                ShowUnavailable("Firebase dependencies are unavailable: " + status);
                return;
            }

            List<PlayerScore> downloadedScores = await GetLeaderboard();
            if (this == null) return;

            scores.Clear();
            scores.AddRange(downloadedScores.OrderByDescending(s => s.score).Take(10));
            UpdateTexts();
        }
        catch (Exception ex)
        {
            if (this == null) return;
            ShowUnavailable("Could not read the leaderboard: " + ex.Message);
        }
    }

    private void Update()
    {
        // Changing the player's name still works locally (Menu.cs / PlayerPrefs).
        // We no longer synchronize name changes with Firebase.
        if (nameChanged) nameChanged = false;
    }

    public async Task<List<PlayerScore>> GetLeaderboard()
    {
        DataSnapshot snapshot = await FirebaseDatabase.DefaultInstance
            .GetReference("leaderboard").GetValueAsync();

        List<PlayerScore> result = new List<PlayerScore>();
        if (!snapshot.Exists) return result;

        foreach (DataSnapshot child in snapshot.Children)
        {
            object nameValue = child.Child("name").Value;
            object scoreValue = child.Child("score").Value;
            if (nameValue == null || scoreValue == null) continue;

            int score;
            if (!int.TryParse(scoreValue.ToString(), out score)) continue;

            result.Add(new PlayerScore(nameValue.ToString(), score));
        }
        return result;
    }

    public void UpdateTexts()
    {
        if (playerNames == null || levelOrbs == null || levels == null) return;

        int numberOfSlots = Math.Min(playerNames.Count,
            Math.Min(levelOrbs.Count, levels.Count));

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (i >= scores.Count)
            {
                if (playerNames[i] != null)
                {
                    playerNames[i].text = "N/A";
                    playerNames[i].color = paper;
                }
                if (levels[i] != null) levels[i].text = "0";
                if (levelOrbs[i] != null) levelOrbs[i].color = paper;
                continue;
            }

            PlayerScore entry = scores[i];
            int level = entry.score;
            Color from, to;

            if (level < 51) { from = paper; to = bronze; }
            else if (level < 102) { from = bronze; to = silver; }
            else if (level < 153) { from = silver; to = gold; }
            else if (level < 204) { from = gold; to = diamond; }
            else if (level < 255) { from = diamond; to = hell; }
            else { from = hell; to = hell; }

            Color rankColor = Color.Lerp(from, to, (level % 51) / 51f);
            if (playerNames[i] != null)
            {
                playerNames[i].text = entry.name;
                playerNames[i].color = rankColor;
            }
            if (levels[i] != null) levels[i].text = level.ToString();
            if (levelOrbs[i] != null) levelOrbs[i].color = rankColor;
        }
    }

    private void ShowUnavailable(string reason)
    {
        Debug.LogWarning("[Leaderboard] " + reason);
        if (errorText != null)
            errorText.text = "Leaderboard temporarily unavailable.";
        // No crash; local XP, level, history and gameplay are unaffected.
    }

    public void LeaderboardClick()
    {
        if (SoundEffectsManager.Instance != null)
            SoundEffectsManager.Instance.PlayClickSound();

        if (animator != null)
            animator.SetTrigger("Leader");
    }
}

[Serializable]
public class PlayerScore
{
    public string name;
    public int score;

    public PlayerScore(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
