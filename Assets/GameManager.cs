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

    [Header("Sound Effects")]
    public AudioClip wrongSound;
    public AudioClip correctSound;

    [Header("Destroy Effect")]
    public ParticleSystem destroyEffect;

    private int requiredNumber;
    private bool alreadyResetting = false;

    // 🔁 Digit Sum Logic
    private int currentDigitSum = 5;
    private const int minDigitSum = 5;
    private const int maxDigitSum = 18;

    void Start()
    {
        StartCoroutine(AnimateRequiredNumberText());
    }

    void Update()
    {
        if (currentNumberManager == null || alreadyResetting)
            return;

        int currentDisplayedNumber = currentNumberManager.currentNumber;

        if (currentDisplayedNumber >= requiredNumber && requiredNumber != 0)
        {
            if (currentDisplayedNumber == requiredNumber)
            {
                Debug.Log("✅ Correct number! Score +1");

                if (correctSound != null)
                    AudioSource.PlayClipAtPoint(correctSound, Camera.main.transform.position);

                ScoreManager.Instance.AddScore(1);
            }
            else
            {
                Debug.Log("❌ Too high!");

                if (wrongSound != null)
                    AudioSource.PlayClipAtPoint(wrongSound, Camera.main.transform.position);
            }

            CleanupTrashAndCubes(); // 💥 Clean up with particles
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

        // Fade out
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🔢 Set new number with controlled digit sum
        requiredNumber = GenerateNumberWithDigitSum(currentDigitSum);
        currentDigitSum++;
        if (currentDigitSum > maxDigitSum)
            currentDigitSum = minDigitSum;

        string fullText = "Give me " + requiredNumber;
        requiredNumberText.text = "";
        requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // Type letter by letter
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

    void CleanupTrashAndCubes()
    {
        Debug.Log("🧹 CleanupTrashAndCubes() called");

        DestroyTaggedObjects("Trash");
        DestroyTaggedObjects("Cubes");
    }

    void DestroyTaggedObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        if (objects.Length == 0)
        {
            Debug.Log($"⚠️ No objects with tag '{tag}' found.");
        }

        foreach (GameObject obj in objects)
        {
            Debug.Log($"🔥 Destroying object with tag '{tag}': {obj.name}");

            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, obj.transform.position, Quaternion.identity);
            }

            Destroy(obj);
        }
    }

    // 🧠 Helper Methods
    int GenerateNumberWithDigitSum(int targetSum)
    {
        while (true)
        {
            int number = Random.Range(111, 999);
            if (DigitSum(number) == targetSum)
                return number;
        }
    }

    int DigitSum(int number)
    {
        int sum = 0;
        while (number > 0)
        {
            sum += number % 10;
            number /= 10;
        }
        return sum;
    }
}
