using UnityEngine;
using FirstGearGames.SmoothCameraShaker;
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
    private void Start()
    {
        speed = GetComponent<PlayerStats>().speed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
     
    private void Update()
    {
        currentDashCd -= Time.deltaTime;
        RotateObjectToMouse();
        if (canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

            animator.SetFloat("Speed", moveDirection.magnitude);

            //if (horizontalInput != 0)
            //{
            //    transform.localScale = new Vector3(Mathf.Sign(horizontalInput) * 1f, 1f, 1f);
            //}
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
            //CameraShakerHandler.Shake(myShake);
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
                rb.velocity = dashDirection * 12f; // Adjust force multiplier as needed
            }
        }
        else if (canMove)
        {
            rb.velocity = moveDirection * speed;
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
        //tra
        //transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + 90f));
    }
}
