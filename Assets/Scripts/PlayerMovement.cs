using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;
    public bool canMove = true;
    public bool canDash = true;
    public bool isDashing = false;
    public bool locChosen = true;
    public bool arrived = false;
    public float dashCd = 2.0f;
    public float currentDashCd = 0.0f;
    private Vector3 direction;
    private Vector3 destination;
    private float dist = 0;
    public Animator animator;
    public AudioSource source_;
    public AudioClip clip_;
    public bool isDashAttack = false;

    void Update()
    {
        currentDashCd -= Time.deltaTime;
        if (canMove)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0).normalized;
            movement *= speed * Time.deltaTime;

            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            if (horizontalInput == 0)
            {
                animator.SetFloat("Speed", Mathf.Abs(verticalInput));
            }
            Vector3 newPosition = transform.position + movement;

            // Calculate the new Z rotation based on the movement direction.
            if (movement != Vector3.zero)
            {
                //float newRotationZ = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
               // transform.rotation = Quaternion.Euler(0, 0, -newRotationZ);
               if (horizontalInput > 0)
                {
                    transform.localScale = new Vector3 (2.5f, 2.5f, 1);
                }
               if (horizontalInput < 0)
                {
                    transform.localScale = new Vector3(-2.5f, 2.5f, 1);
                }
            }

            transform.position = newPosition;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash && currentDashCd <= 0.0f)
        {
            // Calculate the destination based on the player's direction.
            animator.SetBool("CanDash", true);
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
            destination = transform.position + direction * 2.5f; // Change 2.0f to your desired dash distance
            canMove = false;
            canDash = false;
            isDashing = true;
            currentDashCd = dashCd;
        }
        if (isDashAttack)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
            destination = transform.position + direction * 1.5f; // Change 2.0f to your desired dash distance
            canDash = false;
            canMove = false;
            isDashing = true;
            isDashAttack = false;
        }

        if (isDashing)
        {
            float distanceToDestination = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y));
            if (distanceToDestination > 0.1f)
            {
                // Move the player toward the destination, affecting only the X and Y axes.
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y), speed * 4.0f * Time.deltaTime);
            }
            else
            {
                canMove= true;
                canDash= true;
                isDashing= false;
                animator.SetBool("CanDash", false);
            }
        }
    }
    public void PlayMovementClip()
    {
        if (source_ != null)
        {
            source_.PlayOneShot(clip_);
        }    
    }
}
