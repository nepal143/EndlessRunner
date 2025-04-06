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
    private bool alreadyChecked = false;

    void Start()
    {
        StartCoroutine(AnimateRequiredNumberText());
    }

    void Update()
    {
        if (currentNumberManager == null || alreadyChecked)
            return;

        int current = currentNumberManager.currentNumber;

        if (current == requiredNumber)
        {
            ScoreManager.Instance.AddScore(1);
            ResetNumbers();
        }
        else if (current > requiredNumber)
        {
            ResetNumbers();
        }
    }

    void ResetNumbers()
    {
        alreadyChecked = true;

        // Animate current number back to 0
        currentNumberManager.IncreaseNumber(-currentNumberManager.currentNumber);

        // Animate new "Give me" text
        StartCoroutine(AnimateRequiredNumberText());

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

        // Generate new number
        requiredNumber = Random.Range(111, 999);

        // Clear text and start typewriter effect
        string fullText = "Give me " + requiredNumber;
        requiredNumberText.text = "";
        requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Make fully visible

        for (int i = 0; i < fullText.Length; i++)
        {
            requiredNumberText.text += fullText[i];
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    void ResetCheckFlag()
    {
        alreadyChecked = false;
    }
}
