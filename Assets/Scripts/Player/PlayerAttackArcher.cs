using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAttackArcher : MonoBehaviour
{
    public Animator animator;
    public float animTime;
    public float releaseTime;
    public GameObject player;
    public float runSpeed;
    public float adjustedSpeed;
    
    public GameObject _arrow;
    public AudioSource source_;
    public AudioClip clip_;

    public SkillsSO skillSo;

    //TEST REMOVE LATER
    public GameObject skillScriptObj;
    public SkillButtonInformation buttonInfo;

    public List<GameObject> skillButtons = new List<GameObject>();
    public SkillsScript skillScript;

    void Start()
    {
        runSpeed = player.GetComponent<PlayerMovement>().speed;
        adjustedSpeed = 0.0f * runSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        runSpeed = player.GetComponent<PlayerMovement>().speed;
        adjustedSpeed = 0.0f * runSpeed;

        if (animTime > 0)
            animTime -= Time.deltaTime;
        if (releaseTime > 0)
            releaseTime -= Time.deltaTime;
        if (releaseTime <= 0.0f)
            animator.SetBool("isReleasing", true);
       
        if (animTime <= 0)
        {
            player.GetComponent<PlayerMovement>().speed = player.GetComponent<PlayerStats>().speed;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isReleasing", false);
            player.GetComponent<PlayerMovement>().canDash = true;
            player.GetComponent<PlayerMovement>().canMove = true;

            if (Input.GetKey("q"))
            {
                if (skillButtons[0] != null)
                {
                    skillButtons[0].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    StartCoroutine(ButtonClickTimer(0));
                    buttonInfo = skillButtons[0].GetComponent<SkillButtonInformation>();
                    if (buttonInfo.skillsScript != null)
                    {
                        animator.runtimeAnimatorController = buttonInfo.newAnimatorController;
                        skillScriptObj = Instantiate(buttonInfo.skillsScript);
                    }
                    if (skillScriptObj != null && skillScriptObj.GetComponent<SkillsScript>() != null)
                    {
                        skillScript = skillScriptObj.GetComponent<SkillsScript>();
                        skillScript.InitiateSkill();
                    }
                }
            }
            if (Input.GetKey("e"))
            {
                if (skillButtons[1] != null)
                {
                    skillButtons[1].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    StartCoroutine(ButtonClickTimer(1));
                    buttonInfo = skillButtons[1].GetComponent<SkillButtonInformation>();
                    if (buttonInfo.skillsScript != null)
                    {
                        animator.runtimeAnimatorController = buttonInfo.newAnimatorController;
                        skillScriptObj = Instantiate(buttonInfo.skillsScript);
                    }
                    if (skillScriptObj != null && skillScriptObj.GetComponent<SkillsScript>() != null)
                    {
                        skillScript = skillScriptObj.GetComponent<SkillsScript>();
                        skillScript.InitiateSkill();
                    }
                }
            }
            if (Input.GetKey("r"))
            {
                if (skillButtons[2] != null)
                {
                    skillButtons[2].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    StartCoroutine(ButtonClickTimer(2));
                    buttonInfo = skillButtons[2].GetComponent<SkillButtonInformation>();
                    if (buttonInfo.skillsScript != null)
                    {
                        animator.runtimeAnimatorController = buttonInfo.newAnimatorController;
                        skillScriptObj = Instantiate(buttonInfo.skillsScript);               
                    }
                    if (skillScriptObj != null && skillScriptObj.GetComponent<SkillsScript>() != null)
                    {
                        skillScript = skillScriptObj.GetComponent<SkillsScript>();
                        skillScript.InitiateSkill();
                    }
                }
            }
            if (Input.GetMouseButton(1))
            {
                if (skillButtons[3] != null)
                {
                    skillButtons[3].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    StartCoroutine(ButtonClickTimer(3));
                    buttonInfo = skillButtons[3].GetComponent<SkillButtonInformation>();
                    if (buttonInfo.skillsScript != null)
                    {
                        animator.runtimeAnimatorController = buttonInfo.newAnimatorController;
                        skillScriptObj = Instantiate(buttonInfo.skillsScript);
                    }
                    if (skillScriptObj != null && skillScriptObj.GetComponent<SkillsScript>() != null)
                    {
                        skillScript = skillScriptObj.GetComponent<SkillsScript>();
                        skillScript.InitiateSkill();
                    }
                }
            }
        }
    }
    public void PlayArrowClip(float startTime)
    {
        if (source_ != null && clip_ != null)
        {
            source_.Stop();
            source_.time = startTime; // Set the time from which the audio should start
            source_.Play();
        }
    }

    IEnumerator ButtonClickTimer(int _index)
    {
        yield return new WaitForSeconds(0.1f);
        skillButtons[_index].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
