using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("References")]
    public TextMeshProUGUI scoreText;
    public AudioSource audioSource;         // Reference to the AudioSource
    public AudioClip scoreSoundEffect;      // Sound to play when score increases

    public int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        // Play sound effect if assigned
        if (audioSource != null && scoreSoundEffect != null)
        {
            audioSource.PlayOneShot(scoreSoundEffect);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
