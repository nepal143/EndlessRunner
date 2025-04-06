using UnityEngine;
using TMPro;
using System.Collections;

public class CurrentNumberManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI numberText; // Assign in Inspector
    public float animationDuration = 0.5f;

    [Header("Current State")]
    public int currentNumber = 0;

    private Coroutine animationCoroutine;

    void Start()
    {
        UpdateNumberInstant(currentNumber); // Initialize the number display
    }

    public void IncreaseNumber(int amount)
    {
        int newTarget = currentNumber + amount;

        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Start animating to the new number
        animationCoroutine = StartCoroutine(AnimateNumber(currentNumber, newTarget, animationDuration));
        currentNumber = newTarget;
    }

    public void SetNumber(int value)
    {
        // Stop animation if running
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        currentNumber = value;
        UpdateNumberInstant(currentNumber); // Immediately show new value
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

        // Ensure final value is shown
        numberText.text = to.ToString();
    }
}
