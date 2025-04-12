using UnityEngine;

public class TimePauseController : MonoBehaviour
{
    public GameObject uiPanel;

    private bool isPaused = false;
    private float longPressTime = 0.4f;
    private float touchStartTime;
    private Vector2 startTouchPos;
    private bool touchInProgress = false;

    // Show dropdown in inspector
    public enum ResumeMethod { SwipeLeft, SwipeRight, LongPress }

    [Header("Select Resume Method From Dropdown")]
    [SerializeField] private ResumeMethod selectedResumeMethod = ResumeMethod.SwipeRight;

    private void Start()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPaused)
        {
            PauseTime();
        }
    }

    void PauseTime()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (uiPanel != null)
            uiPanel.SetActive(true);
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    void Update()
    {
        if (!isPaused) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    touchStartTime = Time.unscaledTime;
                    touchInProgress = true;
                    break;

                case TouchPhase.Moved:
                    float deltaX = touch.position.x - startTouchPos.x;

                    if (selectedResumeMethod == ResumeMethod.SwipeRight && deltaX > 100f)
                    {
                        ResumeTime();
                        touchInProgress = false;
                    }
                    else if (selectedResumeMethod == ResumeMethod.SwipeLeft && deltaX < -100f)
                    {
                        ResumeTime();
                        touchInProgress = false;
                    }
                    break;

                case TouchPhase.Stationary:
                    if (selectedResumeMethod == ResumeMethod.LongPress &&
                        touchInProgress && (Time.unscaledTime - touchStartTime) >= longPressTime)
                    {
                        ResumeTime();
                        touchInProgress = false;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    touchInProgress = false;
                    break;
            }
        }
    }
}
