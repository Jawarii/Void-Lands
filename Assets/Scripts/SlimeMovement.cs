using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SlimeMovement : MonoBehaviour
{
    public float speed_ = 1.0f;
    private Vector3 direction;
    private Vector3 destination;
    private Vector3 originPos;
    private float restTime = 0.0f;
    private bool isMoving = false;
    private float horizontalDir = 1.0f;
    private float verticalDir = 1.0f;
    public Animator animator_;
    public bool inPursuit = false;
    public GameObject playerObject;
    public float atkCd = 2.0f;
    public float currentAtkCd = 0.0f;
    public float atkTime = 1.0f;
    public float currentAtkTime;
    public bool canMove = true;
    public GameObject colliderObject;
    void Start()
    {
        originPos = transform.position;
        restTime = Random.Range(2f, 6f);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        currentAtkTime = atkTime;
    }

    void Update()
    {
        if (currentAtkCd > 0.0f)
        {
            currentAtkCd -= Time.deltaTime;
        }
        restTime -= Time.deltaTime;
        if (restTime <= 0 && !isMoving && !inPursuit)
        {
            Vector3 direction = new Vector3(Random.Range(0.5f, 2.0f) * horizontalDir, Random.Range(-1.0f, 1.0f), 0);
            destination = originPos + direction;
            isMoving = true;
            transform.localScale = new Vector3(horizontalDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator_.SetFloat("Speed", speed_);
            horizontalDir *= -1;
        }
        if (isMoving && !inPursuit)
        {
            float distanceToDestination = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y));
            if (distanceToDestination > 0.1f)
            {
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y), speed_ * Time.deltaTime);
            }
            else
            {
                animator_.SetFloat("Speed", 0.0f);
                isMoving = false;
                restTime = Random.Range(2f, 6f);
            }
        }
        if (inPursuit)
        {
            ChasePlayer();
        }
    }

    public void ChasePlayer()
    {
        if (playerObject != null)
        {
            destination = playerObject.transform.position;

            float distanceToDestination = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y));


            if (distanceToDestination > 0.7f && canMove && currentAtkCd <= 0)
            {
                // Determine the direction to move
                Vector3 movementDirection = destination - transform.position;

                if (movementDirection.x > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (movementDirection.x < 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                animator_.SetBool("canAttack", false);
                animator_.SetFloat("Speed", speed_);
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(destination.x, destination.y), speed_ * Time.deltaTime);
            }
            else
            {
                if (currentAtkCd <= 0)
                {
                    canMove= false;
                    animator_.SetBool("canAttack", true);
                    animator_.SetFloat("Speed", 0);
                    currentAtkCd = atkCd;
                    currentAtkTime = atkTime;
                }
                else
                {
                    currentAtkTime -= Time.deltaTime;
                    if (currentAtkTime < 0.9f && currentAtkTime > 0.75f)
                    {
                        colliderObject.SetActive(true);
                    }
                    else if (currentAtkTime < 0.6f && currentAtkTime > 0.45f)
                    {
                        colliderObject.SetActive(true);
                    }
                    else if (currentAtkTime < 0.3f && currentAtkTime > 0.15f)
                    {
                        colliderObject.SetActive(true);
                    }
                    else
                    {
                        colliderObject.SetActive(false);
                    }
                    if (currentAtkTime < 0)
                    {
                        animator_.SetBool("canAttack", false);      
                        canMove= true;
                        colliderObject.SetActive(false);
                    }

                }
            }
        }
    }
}
