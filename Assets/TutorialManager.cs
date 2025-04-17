using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialScreens; // Include all screens, last one is "Tap to Start"

    private int currentScreenIndex = 0;

    void Start()
    {
        Time.timeScale = 0; // Pause the game
        ShowScreen(0);      // Show first screen
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentScreenIndex >= tutorialScreens.Length - 1)
            {
                StartGame();
            }
            else
            {
                ShowNextScreen();
            }
        }
    }

    void ShowScreen(int index)
    {
        for (int i = 0; i < tutorialScreens.Length; i++)
        {
            tutorialScreens[i].SetActive(i == index);
        }
    }

    void ShowNextScreen()
    {
        currentScreenIndex++;
        ShowScreen(currentScreenIndex);
    }

    void StartGame()
    {
        tutorialScreens[currentScreenIndex].SetActive(false);
        Time.timeScale = 1; // Resume the game
        this.enabled = false; // 🚫 Disable this script to stop further input
    }
}
