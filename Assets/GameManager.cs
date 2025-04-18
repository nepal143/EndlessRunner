using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Destroy Effects")]
    public ParticleSystem correctDestroyEffect;
    public ParticleSystem wrongDestroyEffect;

    private int requiredNumber;
    private bool alreadyResetting = false;

    // 🔁 Digit Sum Logic
    private int currentDigitSum = 5;
    private const int minDigitSum = 5;
    private const int maxDigitSum = 18;

    private bool isFirstNumber = true;
    int CountMatchingDigits(int a, int b)
{
    string strA = a.ToString();
    string strB = b.ToString();

    int matchCount = 0;
    for (int i = 0; i < Mathf.Min(strA.Length, strB.Length); i++)
    {
        if (strA[i] == strB[i])
        {
            matchCount++;
        }
    }

    return matchCount;
}

    void Start()
    {
        StartCoroutine(AnimateRequiredNumberText());
    }

    void Update()
    {
        if (alreadyResetting) return;

        int currentDisplayedNumber = currentNumberManager.currentNumber;

        if (currentDisplayedNumber >= requiredNumber && requiredNumber != 0)
        {
            bool isCorrect = currentDisplayedNumber == requiredNumber;

           if (isCorrect)
{
    Debug.Log("✅ Correct number! All digits match.");

    if (correctSound != null)
        AudioSource.PlayClipAtPoint(correctSound, Camera.main.transform.position);

    ScoreManager.Instance.AddScore(5);
    CleanupTrashAndCubes(correctDestroyEffect); 
}
else
{
    int matchCount = CountMatchingDigits(currentDisplayedNumber, requiredNumber);

    if (matchCount > 0)
    {
        int bonus = matchCount == 1 ? 2 : 3;
        Debug.Log($"🔢 {matchCount} digit(s) match. Partial score: +{bonus}");

        if (wrongSound != null)
            AudioSource.PlayClipAtPoint(wrongSound, Camera.main.transform.position);

        ScoreManager.Instance.AddScore(bonus);
        CleanupTrashAndCubes(wrongDestroyEffect);
    }
    else
    {
        Debug.Log("❌ No matching digits. Wrong!");

        if (wrongSound != null)
            AudioSource.PlayClipAtPoint(wrongSound, Camera.main.transform.position);

        CleanupTrashAndCubes(wrongDestroyEffect);
    }
}

            // ✅ SEND SCORE BEFORE RESET
            SendScoreToServer(isCorrect);
            ResetNumbers();
        }
    }

    void SendScoreToServer(bool isCorrect)
    {
        if (WebGLBridge.Instance == null)
        {
            Debug.LogWarning("⚠️ WebGLBridge not found.");
            return;
        }

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.score : 0;

        // Access private selectedBlocks field via reflection
        System.Reflection.FieldInfo field = typeof(CurrentNumberManager).GetField("selectedBlocks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Dictionary<int, int> selectedBlocksCopy = field != null ? new Dictionary<int, int>((Dictionary<int, int>)field.GetValue(currentNumberManager)) : new Dictionary<int, int>();

// Manual JSON formatting for selectedBlocks
string selectedBlocksJson = "{";
foreach (var kvp in selectedBlocksCopy)
{
    selectedBlocksJson += $"\"{kvp.Key}\": {kvp.Value}, ";
}
selectedBlocksJson = selectedBlocksJson.TrimEnd(',', ' ') + "}";

// Build the final JSON
string jsonData =
    "[{" +
        $"\"targetNumber\": {requiredNumber}, " +
        $"\"numberMade\": {currentNumberManager.currentNumber}, " +
        $"\"selectedBlocks\": {selectedBlocksJson}, " +
        $"\"result\": \"{(isCorrect ? "Correct" : "Incorrect")}\"" +
    "}]";
        Debug.Log($"📡 Sending data to server: {jsonData}");

        WebGLBridge.Instance.UpdateScore(finalScore, jsonData);
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

    // Set new number
    if (isFirstNumber && WebGLBridge.Instance != null && WebGLBridge.Instance.isTrial)
    {
        requiredNumber = 111;
        Debug.Log("🎯 First trial number set to 111");
    }
    else
    {
        requiredNumber = GenerateNumberWithDigitSum(currentDigitSum);
        currentDigitSum++;
        if (currentDigitSum > maxDigitSum)
            currentDigitSum = minDigitSum;
    }

    string fullText = "" + requiredNumber;
    requiredNumberText.text = "";
    requiredNumberText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

    // Type letter by letter
    for (int i = 0; i < fullText.Length; i++)
    {
        requiredNumberText.text += fullText[i];
        yield return new WaitForSeconds(typeSpeed);
    }

    // 🟢 Start Game after the first number is shown
    if (isFirstNumber && WebGLBridge.Instance != null)
    {
        WebGLBridge.Instance.StartGame();
        Debug.Log("🚀 WebGL StartGame() called after showing the first number.");
    }

    isFirstNumber = false;
}

    void ResetCheckFlag()
    {
        alreadyResetting = false;
    }

    void CleanupTrashAndCubes(ParticleSystem effect)
    {
        Debug.Log("🧹 CleanupTrashAndCubes() called");

        DestroyTaggedObjects("Trash", effect);
        DestroyTaggedObjects("Cubes", effect);
    }

    void DestroyTaggedObjects(string tag, ParticleSystem effect)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        if (objects.Length == 0)
        {
            Debug.Log($"⚠️ No objects with tag '{tag}' found.");
        }

        foreach (GameObject obj in objects)
        {
            Debug.Log($"🔥 Destroying object with tag '{tag}': {obj.name}");

            if (effect != null)
            {
                Instantiate(effect, obj.transform.position, Quaternion.identity);
            }

            Destroy(obj);
        }
    }

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
