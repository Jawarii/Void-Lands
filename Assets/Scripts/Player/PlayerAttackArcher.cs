using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class PlayerAttackArcher : MonoBehaviour
{
    public Animator animator;
    public float animTime;
    public float releaseTime;
    public GameObject player;
    public float runSpeed;
    public float adjustedSpeed;
    public bool isAttacking = false;

    public GameObject _arrow;
    public AudioSource source_;
    public AudioClip clip_;

    public SkillsSO skillSo;

    public GameObject skillScriptObj;
    public SkillButtonInformation buttonInfo;

    public List<GameObject> skillButtons = new List<GameObject>();
    public SkillsScript skillScript;

    public float coolDown0 = 0;
    public float coolDown1 = 0;
    public float coolDown2 = 0;
    public float coolDown3 = 0;

    public bool hasImbueBuff = false;

    void Start()
    {
        runSpeed = player.GetComponent<PlayerMovement>().speed;
        //adjustedSpeed = 0.0f * runSpeed;
    }

    void Update()
    {
        runSpeed = player.GetComponent<PlayerMovement>().speed;
        //adjustedSpeed = 0.0f * runSpeed;
        if (animator != null)
        {
            //animator.SetFloat("AtkSpeed", player.GetComponent<PlayerStats>().atkSpd);
        }

        if (animTime > 0)
            animTime -= Time.deltaTime;
        if (releaseTime > 0)
            releaseTime -= Time.deltaTime;
        if (releaseTime <= 0.0f)
            animator.SetBool("isReleasing", true);

        if (animTime <= 0 && !player.GetComponent<PlayerStats>().isDead)
        {
            isAttacking = false;
            player.GetComponent<PlayerMovement>().speed = player.GetComponent<PlayerStats>().speed;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isReleasing", false);
            player.GetComponent<PlayerMovement>().canDash = true;
            player.GetComponent<PlayerMovement>().canMove = true;

            if (Input.GetKey("q") && coolDown0 == 0)
            {
                HandleSkillActivation(0, ref coolDown0);
            }
            if (Input.GetKey("e") && coolDown1 == 0)
            {
                HandleSkillActivation(1, ref coolDown1);
            }
            if (Input.GetKey("r") && coolDown2 == 0)
            {
                HandleSkillActivation(2, ref coolDown2);
            }
            if (Input.GetMouseButton(1) && coolDown3 == 0)
            {
                HandleSkillActivation(3, ref coolDown3);
            }
        }
    }

    private void HandleSkillActivation(int skillIndex, ref float coolDown)
    {
        if (player.GetComponent<PlayerStats>().isDead == true)
            return;

        if (skillButtons[skillIndex] != null)
        {
            skillButtons[skillIndex].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            StartCoroutine(ButtonClickTimer(skillIndex));
            buttonInfo = skillButtons[skillIndex].GetComponent<SkillButtonInformation>();
            if (buttonInfo.skillsScript != null)
            {
                animator.runtimeAnimatorController = buttonInfo.newAnimatorController;
                skillScriptObj = Instantiate(buttonInfo.skillsScript);
            }
            if (skillScriptObj != null && skillScriptObj.GetComponent<SkillsScript>() != null)
            {
                skillScript = skillScriptObj.GetComponent<SkillsScript>();
                skillScript.InitiateSkill();
                skillSo = buttonInfo.skillSo;
                coolDown = skillSo.coolDown;
                isAttacking = true;
                StartCoroutine(HandleCooldown(skillIndex, coolDown));
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
        yield return new WaitForSeconds(0.05f);
        skillButtons[_index].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    IEnumerator HandleCooldown(int skillIndex, float cooldown)
    {
        // Adjust cooldown based on cooldown reduction
        cooldown = cooldown * (1f - player.GetComponent<PlayerStats>().cdReduction / 100f);
        float remainingCooldown = cooldown;

        // Get the cooldown overlay and cooldown text
        Image cooldownOverlay = skillButtons[skillIndex].transform.Find("Grayout").GetComponent<Image>();
        TMP_Text cooldownText = skillButtons[skillIndex].transform.Find("CooldownText").GetComponent<TMP_Text>();

        // Set the overlay and activate the cooldown text
        cooldownOverlay.fillAmount = 1;

        while (remainingCooldown > 0)
        {
            // Update the remaining cooldown and fill amount of the overlay
            remainingCooldown -= Time.deltaTime;
            cooldownOverlay.fillAmount = remainingCooldown / cooldown;

            // Update the cooldown text with one decimal place
            cooldownText.text = remainingCooldown.ToString("F1");

            yield return null;
        }

        // Reset the overlay and deactivate the cooldown text
        cooldownOverlay.fillAmount = 0;
        cooldownText.text = "";

        // Reset the appropriate cooldown variable
        switch (skillIndex)
        {
            case 0:
                coolDown0 = 0;
                break;
            case 1:
                coolDown1 = 0;
                break;
            case 2:
                coolDown2 = 0;
                break;
            case 3:
                coolDown3 = 0;
                break;
        }
    }
}
