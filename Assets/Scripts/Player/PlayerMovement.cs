using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using FirstGearGames.SmoothCameraShaker;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1f;
    public bool canMove = true;
    public bool canDash = true;
    public bool isDashing = false;
    public bool isMoving = false;
    public bool isDashAttack = false;
    public bool locChosen = true;
    public bool arrived = false;
    public float dashCd = 2.0f;
    public float currentDashCd = 0.0f;
    public float dashDistance = 1.5f; // Adjust the dash distance here

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 dashDirection;
    private Animator animator;
    private Vector2 dashStartPosition;
    private float expectedDashDuration;

    public AudioSource dashSource_;
    public AudioSource stepsSource_;

    public AudioClip stepsClip_;
    public AudioClip dashClip_;

    public ShakeData myShake;

    // UI references
    private Image dashSlider;
    private TextMeshProUGUI cooldownText; // Use TMP for the text
    private GameObject dashSlot;

    private void Start()
    {
        speed = GetComponent<PlayerStats>().speed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Find UI elements for cooldown display
        dashSlot = GameObject.Find("DashSlot");
        if (dashSlot != null)
        {
            dashSlider = dashSlot.transform.Find("Grayout").GetComponent<Image>();
            cooldownText = dashSlot.transform.Find("CooldownText").GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (currentDashCd > 0)
        {
            currentDashCd -= Time.deltaTime;
            UpdateCooldownUI();
        }

        if (currentDashCd <= 0 && !canDash)
        {
            canDash = true;
            cooldownText.text = ""; // Clear the cooldown text
        }

        RotateObjectToMouse();
        if (canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            if (horizontalInput != 0.0f || verticalInput != 0.0f)
                isMoving = true;
            else
                isMoving = false;

            moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

            animator.SetFloat("Speed", moveDirection.magnitude * GetComponent<PlayerMovement>().speed);
        }
        else
        {
            rb.velocity = new Vector2(0f, 0f);
            animator.SetFloat("Speed", 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash && currentDashCd <= 0.0f)
        {
            animator.SetBool("CanDash", true);
            dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            dashStartPosition = rb.position;
            canMove = false;
            canDash = false;
            isDashing = true;
            currentDashCd = dashCd;
            PlayDashClip();
            CalculateExpectedDashDuration();
            GetComponent<TrailRenderer>().enabled = true;

            StartCoroutine(ScaleDashSlotEffect()); // Scale the DashSlot
            UpdateCooldownUI();                   // Immediately update the cooldown UI
        }

        if (isDashAttack)
        {
            dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            dashStartPosition = rb.position;
            canDash = false;
            canMove = false;
            isDashing = true;
            isDashAttack = false;

            CalculateExpectedDashDuration();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            float distanceTraveled = Vector2.Distance(dashStartPosition, rb.position);
            if (distanceTraveled >= dashDistance || Time.time >= expectedDashDuration)
            {
                isDashing = false;
                canMove = true;
                canDash = true;
                animator.SetBool("CanDash", false);
                rb.velocity = Vector2.zero;
                GetComponent<TrailRenderer>().enabled = false;
            }
            else
            {
                rb.velocity = dashDirection * 12f; // Adjust force multiplier as needed
            }
        }
        else if (canMove)
        {
            rb.velocity = moveDirection * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void PlayMovementClip()
    {
        if (stepsSource_ != null)
        {
            stepsSource_.PlayOneShot(stepsClip_);
        }
    }

    public void PlayDashClip()
    {
        if (dashSource_ != null)
        {
            dashSource_.Stop();
            dashSource_.volume = 1f;
            dashSource_.PlayOneShot(dashClip_);
        }
    }

    private void CalculateExpectedDashDuration()
    {
        float distance = Vector2.Distance(rb.position, rb.position + dashDirection * dashDistance);
        expectedDashDuration = Time.time + distance / (12f); // Adjust force multiplier as needed
    }

    private void UpdateCooldownUI()
    {
        if (dashSlider != null)
        {
            dashSlider.fillAmount = currentDashCd / dashCd; // Update slider based on cooldown
        }

        if (cooldownText != null)
        {
            if (currentDashCd > 0)
            {
                cooldownText.text = currentDashCd.ToString("F1"); // Show cooldown with 1 decimal
            }
            else
            {
                cooldownText.text = ""; // Clear text when cooldown is over
                dashSlider.fillAmount = 0.0f;
            }
        }
    }

    private IEnumerator ScaleDashSlotEffect()
    {
        if (dashSlot != null)
        {
            // Scale the DashSlot to 0.9
            dashSlot.transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);

            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);

            // Scale it back to 1.0
            dashSlot.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    void RotateObjectToMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.z * -1));
        Vector3 directionToMouse = mouseWorldPosition - transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg + 45f;

        if (angle > 0.0f && angle <= 90f)
        {
            animator.SetFloat("Rotation", 1f);
            animator.SetFloat("Rotation2", 0f);
        }
        else if (angle > 90f && angle <= 180f)
        {
            animator.SetFloat("Rotation", 0f);
            animator.SetFloat("Rotation2", 1f);
        }
        else if (angle > -90f && angle <= 0f)
        {
            animator.SetFloat("Rotation", 0f);
            animator.SetFloat("Rotation2", -1f);
        }
        else
        {
            animator.SetFloat("Rotation", -1f);
            animator.SetFloat("Rotation2", 0f);
        }
    }
}
