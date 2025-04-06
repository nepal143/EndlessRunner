using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("References")]
    public TextMeshProUGUI scoreText;

    public int score = 0;


    void Start()
    {
        UpdateScoreUI(); // Ensure UI starts with Score: 0
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
