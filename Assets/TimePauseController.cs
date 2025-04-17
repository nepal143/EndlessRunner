using UnityEngine;

public class TimePauseController : MonoBehaviour
{
    public GameObject uiPanel;

    private bool isPaused = false;
    private float longPressTime = 0.4f;
    private float minPauseDuration = 0.4f;
    private float pauseStartTime;
    private float touchStartTime;
    private Vector2 startTouchPos;
    private bool touchInProgress = false;

    private float minSwipeDistance = 150f;     // Minimum distance in pixels
    private float minSwipeHoldTime = 0.1f;      // Minimum time finger must be held before releasing

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
        pauseStartTime = Time.unscaledTime;

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

        if (Time.unscaledTime - pauseStartTime < minPauseDuration)
            return;

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

                case TouchPhase.Stationary:
                    if (selectedResumeMethod == ResumeMethod.LongPress &&
                        touchInProgress &&
                        (Time.unscaledTime - touchStartTime) >= longPressTime)
                    {
                        ResumeTime();
                        touchInProgress = false;
                    }
                    break;

                case TouchPhase.Ended:
                    if (!touchInProgress) break;

                    Vector2 endTouchPos = touch.position;
                    float swipeTime = Time.unscaledTime - touchStartTime;
                    float swipeDistance = Vector2.Distance(endTouchPos, startTouchPos);
                    float deltaX = endTouchPos.x - startTouchPos.x;
                    float deltaY = Mathf.Abs(endTouchPos.y - startTouchPos.y);

                    // Avoid vertical swipes or diagonal swipes
                    if (deltaY > Mathf.Abs(deltaX))
                    {
                        touchInProgress = false;
                        break;
                    }

                    // Debug.Log($"Swipe: {deltaX}px in {swipeTime}s (dist: {swipeDistance})");

                    if (selectedResumeMethod == ResumeMethod.SwipeRight &&
                        deltaX > 0 &&
                        swipeDistance > minSwipeDistance &&
                        swipeTime >= minSwipeHoldTime)
                    {
                        ResumeTime();
                    }
                    else if (selectedResumeMethod == ResumeMethod.SwipeLeft &&
                        deltaX < 0 &&
                        swipeDistance > minSwipeDistance &&
                        swipeTime >= minSwipeHoldTime)
                    {
                        ResumeTime();
                    }

                    touchInProgress = false;
                    break;

                case TouchPhase.Canceled:
                    touchInProgress = false;
                    break;
            }
        }
    }
}
