using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialScreens; // Assign each tutorial panel in order
    public Button[] nextButtons;         // Assign the "Next" buttons from each screen
    public GameObject tapToStartScreen;  // Final screen with "Tap to Start"

    private int currentScreenIndex = 0;
    private bool waitingForTap = false;

    void Start()
    {
        Time.timeScale = 0; // Pause the game
        ShowScreen(0);

        // Add listeners to next buttons
        for (int i = 0; i < nextButtons.Length; i++)
        {
            int index = i;
            nextButtons[i].onClick.AddListener(() => ShowNextScreen(index));
        }
    }

    void Update()
    {
        if (waitingForTap && Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            StartGame();
        }
    }

    void ShowScreen(int index)
    {
        for (int i = 0; i < tutorialScreens.Length; i++)
        {
            tutorialScreens[i].SetActive(i == index);
        }

        tapToStartScreen.SetActive(false);
    }

    void ShowNextScreen(int index)
    {
        if (index + 1 < tutorialScreens.Length)
        {
            ShowScreen(index + 1);
        }
        else
        {
            ShowFinalScreen();
        }
    }

    void ShowFinalScreen()
    {
        foreach (GameObject screen in tutorialScreens)
        {
            screen.SetActive(false);
        }

        tapToStartScreen.SetActive(true);
        waitingForTap = true;
    }

    void StartGame()
    {
        tapToStartScreen.SetActive(false);
        Time.timeScale = 1; // Resume the game
        waitingForTap = false;
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
