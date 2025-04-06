using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI requiredNumberText;
    public TextMeshProUGUI currentNumberText; // Reference to the actual UI text
    public CurrentNumberManager currentNumberManager;

    [Header("Typing Animation")]
    public ScoreManager scoreManager;
    public float typeSpeed = 0.05f;

    [Header("Sound Effects")]
    public AudioClip wrongSound;
    public AudioClip correctSound;

    private int requiredNumber;
    private bool alreadyResetting = false;

    void Start()
    {
        StartCoroutine(AnimateRequiredNumberText());
    }

    void Update()
    {
        if (currentNumberText == null || alreadyResetting)
            return;

        if (!int.TryParse(currentNumberText.text, out int currentDisplayedNumber))
            return;

        if (currentDisplayedNumber == requiredNumber && requiredNumber != 0)
        {
            Debug.Log("Correct number! Score +1");
            if (correctSound != null)
                AudioSource.PlayClipAtPoint(correctSound, Camera.main.transform.position);

            ScoreManager.Instance.AddScore(1);
            ResetNumbers();
        }
        else if (currentDisplayedNumber > requiredNumber)
        {
            Debug.Log("Too high");

            if (wrongSound != null)
                AudioSource.PlayClipAtPoint(wrongSound, Camera.main.transform.position);

            ResetNumbers();
        }
    }

    void ResetNumbers()
    {
        alreadyResetting = true;
        currentNumberManager.SetNumber(0);
        StartCoroutine(AnimateRequiredNumberText());
        Invoke(nameof(ResetCheckFlag), 0.1f);
    }

    IEnumerator AnimateRequiredNumberText()
    {
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        Color originalColor = requiredNumberText.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        requiredNumber = Random.Range(111, 999);
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
