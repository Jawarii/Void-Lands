using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float slashTime_;
    public float stabTime_;
    public float currentTime;
    public GameObject player;
    public float runSpeed;
    public float adjustedSpeed;
    public GameObject colliderObject;
    public bool isSlash = false;
    public bool isStab = false;
    public AudioSource source_;
    public AudioClip clip_;
    
    void Start()
    {
        runSpeed = player.GetComponent<PlayerMovement>().speed;
        adjustedSpeed = 0.1f * runSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            player.GetComponent<PlayerMovement>().speed = runSpeed;
            animator.SetBool("CanSlash", false);
            animator.SetBool("CanStab", false);
            isSlash = false;
            isStab = false;

            player.GetComponent<PlayerMovement>().canDash = true;
            if (Input.GetMouseButtonDown(0))
            {
                player.GetComponent<PlayerMovement>().speed = adjustedSpeed;
                currentTime = slashTime_;
                animator.SetBool("CanSlash", true);
                player.GetComponent<PlayerMovement>().canDash = false;
                isSlash = true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                player.GetComponent<PlayerMovement>().speed = adjustedSpeed;
                currentTime = stabTime_;
                animator.SetBool("CanStab", true);
                player.GetComponent<PlayerMovement>().canDash = false;
                isStab = true;
            }
        }
        else
        {
            if (isSlash)
            {
                if (currentTime <= 0.35f && currentTime > 0.19f)
                {
                    colliderObject.SetActive(true);
                }
                else if (currentTime <= 0.15f && currentTime > 0.01f)
                {
                    colliderObject.SetActive(true);
                }
                else
                {
                    colliderObject.SetActive(false);
                }
            }
            if (isStab)
            {
                if (currentTime <= 0.35f && currentTime > 0.15f)
                {
                    colliderObject.SetActive(true);
                }
                else
                {
                    colliderObject.SetActive(false);
                }
            }
        }
    }
    public void PlaySlashClip()
    {
        source_.Stop();
        source_.PlayOneShot(clip_);
    }
}
