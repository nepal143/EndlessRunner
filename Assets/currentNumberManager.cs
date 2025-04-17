using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CurrentNumberManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI multiplierText;
    public float animationDuration = 0.5f;
    public float multiplierDisplayTime = 1.5f;

    [Header("Current State")]
    public int currentNumber = 0;

    private Coroutine animationCoroutine;
    private Coroutine multiplierCoroutine;

    // New: Dictionary to track how many times each value is added
    private Dictionary<int, int> selectedBlocks = new Dictionary<int, int>
    {
        { 1, 0 },
        { 10, 0 },
        { 100, 0 }
    };

    void Start()
    {
        UpdateNumberInstant(currentNumber);
        multiplierText.alpha = 0;
    }

    public void IncreaseNumber(int amount)
    {
        int newTarget = currentNumber + amount;

        // Update dictionary count
        if (selectedBlocks.ContainsKey(amount))
        {
            selectedBlocks[amount]++;
        }

        ShowMultiplierText(amount);

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateNumber(currentNumber, newTarget, animationDuration));
        currentNumber = newTarget;
    }

    void Update()
    {
        // Log the current state of selectedBlocks
        if (selectedBlocks[1] > 0 || selectedBlocks[10] > 0 || selectedBlocks[100] > 0)
        {
            string log = "[CurrentNumberManager] selectedBlocks: { ";
            foreach (var kvp in selectedBlocks)
            {
                log += kvp.Key + ": " + kvp.Value + ", ";
            }
            log = log.TrimEnd(',', ' ') + " }";
            Debug.Log(log);
        }
    }
public void SetNumber(int value)
{
    if (animationCoroutine != null) StopCoroutine(animationCoroutine);
    if (multiplierCoroutine != null) StopCoroutine(multiplierCoroutine);

    currentNumber = value;
    UpdateNumberInstant(currentNumber);

    if (value == 0)
    {
        // FIX: Make a copy of keys before modifying the dictionary
        List<int> keys = new List<int>(selectedBlocks.Keys);
        foreach (int key in keys)
        {
            selectedBlocks[key] = 0;
        }

        multiplierText.text = "";
        multiplierText.alpha = 0;
    }
}    void UpdateNumberInstant(int number)
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
        multiplierText.text = "+" + amount.ToString();

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

        yield return new WaitForSeconds(multiplierDisplayTime);

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
