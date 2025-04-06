using UnityEngine;
using System.Collections;

public class EndlessRunnerController : MonoBehaviour
{
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
    }

    void Update()
    {
        if (!isStunned)
        {
            HandleSwipeInput();
        }

        if (!isHitAnimating)
        {
            MoveForward(); // Only move forward if not in hit animation
        }

        MoveToLane(); // Keep aligning to lane always
    }

    void HandleSwipeInput()
    {
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
                if (swipeDelta.x > 50)
                    ChangeLane(1);
                else if (swipeDelta.x < -50)
                    ChangeLane(-1);
            }

            isSwiping = false;
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
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
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
