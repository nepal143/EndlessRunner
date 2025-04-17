using UnityEngine;
using System.Collections;

public class EndlessRunnerController : MonoBehaviour
{
    [Header("UI Panels that control input")]
    public GameObject panel1; // Swipe Left
    public GameObject panel2; // Swipe Right
    public GameObject panel3; // Hold to Sprint

    private bool swipeLeftUnlocked = false;
    private bool swipeRightUnlocked = false;
    private bool sprintUnlocked = false;

    private bool panel1WasActive = false;
    private bool panel2WasActive = false;
    private bool panel3WasActive = false;

    private int currentLane = 0; // -1 = left, 0 = center, 1 = right
    private float laneOffset = 1f;
    private float laneSwitchSpeed = 10f;
    private Vector3 targetPosition;

    private Vector2 swipeStart;
    private bool isSwiping = false;

    [Header("Forward Movement")]
    public float forwardSpeed = 5f;
    private bool isStunned = false;
    private bool isHitAnimating = false;

    [Header("Sprinting")]
    public float sprintMultiplier = 2f;
    private bool isSprinting = false;
    public float sprintHoldDelay = 0.4f;
    private float holdTimer = 0f;

    [Header("Animator")]
    public Animator animator;

    [Header("Blinking")]
    public GameObject[] blinkObjects; // Assign only the visible parts like body, hat, etc.

    [Header("Audio")]
    public AudioClip hitSound;

    private BoxCollider playerCollider;

void Start()
{
    playerCollider = GetComponent<BoxCollider>();
    targetPosition = transform.position;

    // Automatically grant all movement powers if not a trial
    if (WebGLBridge.Instance != null && !WebGLBridge.Instance.isTrial)
    {
        GrantAllPowers();
    }
}
void GrantAllPowers()
{
    Debug.Log("🕹️ Granting all movement powers to player (non-trial mode)");

    swipeLeftUnlocked = true;
    swipeRightUnlocked = true;
    sprintUnlocked = true;

    // Optionally hide the tutorial panels
    if (panel1 != null) panel1.SetActive(false);
    if (panel2 != null) panel2.SetActive(false);
    if (panel3 != null) panel3.SetActive(false);
}

    void Update()
    {
        CheckAndUnlockAbilities();

        if (!isHitAnimating)
        {
            MoveForward(); // Only move forward if not in hit animation
        }

        MoveToLane();

        if (!isStunned)
        {
            HandleSwipeInput();
            HandleSprintHold();
        }
        else
        {
            isSprinting = false;
            holdTimer = 0f;
        }
    }

    void CheckAndUnlockAbilities()
    {
        if (panel1 != null && panel1.activeInHierarchy && !panel1WasActive)
        {
            swipeLeftUnlocked = true;
            panel1WasActive = true;
        }

        if (panel2 != null && panel2.activeInHierarchy && !panel2WasActive)
        {
            swipeRightUnlocked = true;
            panel2WasActive = true;
        }

        if (panel3 != null && panel3.activeInHierarchy && !panel3WasActive)
        {
            sprintUnlocked = true;
            panel3WasActive = true;
        }
    }

    void HandleSwipeInput()
    {
        if (!swipeLeftUnlocked && !swipeRightUnlocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipeDelta = swipeEnd - swipeStart;

            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                if (swipeDelta.x > 50 && swipeRightUnlocked)
                    ChangeLane(1);
                else if (swipeDelta.x < -50 && swipeLeftUnlocked)
                    ChangeLane(-1);
            }

            isSwiping = false;
        }
    }

    void HandleSprintHold()
    {
        if (!sprintUnlocked) return;

        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= sprintHoldDelay)
            {
                isSprinting = true;
            }
        }
        else
        {
            holdTimer = 0f;
            isSprinting = false;
        }
    }

    void ChangeLane(int direction)
    {
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, -1, 1);
        targetPosition = new Vector3(currentLane * laneOffset, transform.position.y, transform.position.z);
    }

    void MoveToLane()
    {
        Vector3 desiredPosition = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * laneSwitchSpeed);
    }

    void MoveForward()
    {
        float speed = isSprinting ? forwardSpeed * sprintMultiplier : forwardSpeed;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trash") && !isStunned)
        {
            StartCoroutine(HandleTrashCollision());
        }
    }

    IEnumerator HandleTrashCollision()
    {
        isStunned = true;

        // Knockback
        transform.position -= new Vector3(0, 0, 1f);

        // Play hit sound
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // Start hit animation
        if (animator != null)
        {
            animator.SetBool("Hit", true);
            isHitAnimating = true;
        }

        // Wait for animation duration
        yield return new WaitForSeconds(0.5f);

        if (animator != null)
        {
            animator.SetBool("Hit", false);
            isHitAnimating = false;
        }

        // Make collider a trigger to avoid repeat hits
        if (playerCollider != null)
        {
            playerCollider.isTrigger = true;
        }

        // Blink effect
        yield return StartCoroutine(BlinkCharacter(0.5f));

        // Restore collider
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }

        isStunned = false;
    }

    IEnumerator BlinkCharacter(float duration)
    {
        float blinkInterval = 0.1f;
        float timer = 0f;

        while (timer < duration)
        {
            SetBlinkObjectsVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetBlinkObjectsVisible(true);
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval * 2;
        }

        SetBlinkObjectsVisible(true);
    }

    void SetBlinkObjectsVisible(bool visible)
    {
        foreach (GameObject obj in blinkObjects)
        {
            if (obj != null)
            {
                Renderer r = obj.GetComponent<Renderer>();
                if (r != null)
                {
                    r.enabled = visible;
                }
            }
        }
    }
}
