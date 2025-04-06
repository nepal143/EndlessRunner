using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI requiredNumberText;
    public CurrentNumberManager currentNumberManager;

    [Header("Typing Animation")]
    public float typeSpeed = 0.05f;

    private int requiredNumber;
    private bool alreadyResetting = false;

    void Start()
    {
        StartCoroutine(AnimateRequiredNumberText());
    }

    void Update()
    {
        if (currentNumberManager == null || alreadyResetting)
            return;

        int current = currentNumberManager.currentNumber;
        Debug.Log("Current number: " + current);
        Debug.Log("Required number: " + requiredNumber);
        if (current == requiredNumber)
        {
            Debug.Log("Correct number! Score +1");
            ScoreManager.Instance.AddScore(1);
            ResetNumbers(); // Reset only after scoring
        }
        else if (current > requiredNumber)
        {
            Debug.Log("Too high");
            ResetNumbers(); // No score, just reset
        }
    }


    void ResetNumbers()
    {
        alreadyResetting = true;

        // Instantly reset current number
        currentNumberManager.SetNumber(0);

        // Animate new prompt
        StartCoroutine(AnimateRequiredNumberText());

        // Allow checking again after short delay
        Invoke(nameof(ResetCheckFlag), 0.1f);
    }

    IEnumerator AnimateRequiredNumberText()
    {
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        Color originalColor = requiredNumberText.color;

        // Fade out
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // New required number
        requiredNumber = Random.Range(111, 999);

        // Typewriter effect
        string fullText = "Give me " + requiredNumber;
        requiredNumberText.text = "";
        requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        for (int i = 0; i < fullText.Length; i++)
        {
            requiredNumberText.text += fullText[i];
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    void ResetCheckFlag()
    {
        alreadyResetting = false;
    }
}
