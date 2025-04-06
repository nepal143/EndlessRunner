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
        UpdateNumberInstant(currentNumber); // Initialize
    }

    public void IncreaseNumber(int amount)
    {
        int newTarget = currentNumber + amount;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateNumber(currentNumber, newTarget, animationDuration));
        currentNumber = newTarget;
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

        numberText.text = to.ToString(); // Final value
    }
}
