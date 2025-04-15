using UnityEngine;
using TMPro;
using System.Collections;

public class CurrentNumberManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI numberText;       // Assign in Inspector
    public TextMeshProUGUI multiplierText;   // Assign in Inspector
    public float animationDuration = 0.5f;
    public float multiplierDisplayTime = 1.5f;

    [Header("Current State")]
    public int currentNumber = 0;

    private Coroutine animationCoroutine;
    private Coroutine multiplierCoroutine;

    private int count1 = 0;
    private int count10 = 0;
    private int count100 = 0;

    void Start()
    {
        UpdateNumberInstant(currentNumber);
        multiplierText.alpha = 0; // Hide on start
    }

    public void IncreaseNumber(int amount)
    {
        int newTarget = currentNumber + amount;

        // Update multiplier counts
        switch (amount)
        {
            case 1: count1++; break;
            case 10: count10++; break;
            case 100: count100++; break;
        }

        // Update multiplier display
        ShowMultiplierText(amount);

        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Start animating number
        animationCoroutine = StartCoroutine(AnimateNumber(currentNumber, newTarget, animationDuration));
        currentNumber = newTarget;
    }

    public void SetNumber(int value)
    {
        // Stop animations
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        if (multiplierCoroutine != null) StopCoroutine(multiplierCoroutine);

        currentNumber = value;
        UpdateNumberInstant(currentNumber);

        if (value == 0)
        {
            count1 = count10 = count100 = 0;
            multiplierText.text = "";
            multiplierText.alpha = 0;
        }
    }

    void UpdateNumberInstant(int number)
    {
        if (numberText != null)
        {
            numberText.text = number.ToString();
        }
    }

    IEnumerator AnimateNumber(int from, int to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            int displayedValue = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            numberText.text = displayedValue.ToString();
            elapsed += Time.deltaTime;
            yield return null;
        }
        numberText.text = to.ToString();
    }

    void ShowMultiplierText(int amount)
    {
        string display = "";
        switch (amount)
        {
            case 1: display = "1x" + count1; break;
            case 10: display = "10x" + count10; break;
            case 100: display = "100x" + count100; break;
        }

        multiplierText.text = display;

        if (multiplierCoroutine != null)
            StopCoroutine(multiplierCoroutine);
        
        multiplierCoroutine = StartCoroutine(AnimateMultiplierText());
    }

    IEnumerator AnimateMultiplierText()
    {
        multiplierText.alpha = 0;
        multiplierText.transform.localScale = Vector3.one * 0.5f;

        float elapsed = 0f;
        float fadeInTime = 0.2f;
        float scaleTarget = 1.2f;

        // Fade in and scale up
        while (elapsed < fadeInTime)
        {
            float t = elapsed / fadeInTime;
            multiplierText.alpha = Mathf.Lerp(0, 1, t);
            multiplierText.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * scaleTarget, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        multiplierText.alpha = 1;
        multiplierText.transform.localScale = Vector3.one;

        // Wait
        yield return new WaitForSeconds(multiplierDisplayTime);

        // Fade out
        elapsed = 0f;
        float fadeOutTime = 0.3f;

        while (elapsed < fadeOutTime)
        {
            float t = elapsed / fadeOutTime;
            multiplierText.alpha = Mathf.Lerp(1, 0, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        multiplierText.alpha = 0;
    }
}
