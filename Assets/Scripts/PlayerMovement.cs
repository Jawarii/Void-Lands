using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1f;
    public bool canMove = true;
    public bool canDash = true;
    public bool isDashing = false;
    public bool isDashAttack = false;
    public bool locChosen = true;
    public bool arrived = false;
    public float dashCd = 2.0f;
    public float currentDashCd = 0.0f;
    public float dashDistance = 2.5f; // Adjust the dash distance here

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 dashDirection;
    private Animator animator;
    private Vector2 dashStartPosition;
    private float expectedDashDuration;

    public AudioSource source_;
    public AudioClip clip_;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        currentDashCd -= Time.deltaTime;

        if (canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

            animator.SetFloat("Speed", moveDirection.magnitude);

            if (horizontalInput != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontalInput) * 2.5f, 2.5f, 1f);
            }
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

            CalculateExpectedDashDuration();
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
            }
            else
            {
                rb.velocity = dashDirection * speed * 3.5f; // Adjust force multiplier as needed
            }
        }
        else if (canMove)
        {
            rb.velocity = moveDirection * speed;
        }
    }

    public void PlayMovementClip()
    {
        if (source_ != null)
        {
            source_.PlayOneShot(clip_);
        }
    }

    private void CalculateExpectedDashDuration()
    {
        float distance = Vector2.Distance(rb.position, rb.position + dashDirection * dashDistance);
        expectedDashDuration = Time.time + distance / (speed * 2.5f); // Adjust force multiplier as needed
    }
}
