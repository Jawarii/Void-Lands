using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingArrowSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float coolDown = 5f;
    public float cdElapsedTime = 0f;
    public float adjustedSpeed = 0.1f;
    public override void ActivateSkill()
    {
        if (cdElapsedTime > 0f) return;
        playerAttack.animTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd + bowReleaseAnim.length;
        playerAttack.releaseTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd;
        playerAttack.player.GetComponent<PlayerMovement>().speed *= adjustedSpeed;
        playerAttack.animator.SetBool("isAttacking", true);
        playerAttack.animator.SetFloat("AtkSpeed", playerAttack.player.GetComponent<PlayerStats>().atkSpd);
        playerAttack.player.GetComponent<PlayerMovement>().canDash = false;
        //playerAttack.player.GetComponent<PlayerMovement>().canMove = false;
        StartCoroutine(SpawnArrow());
    }
    IEnumerator SpawnArrow()
    {
        yield return new WaitForSeconds(bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd);

        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.z * -1));
        Vector3 directionToMouse = mouseWorldPosition - playerAttack.transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        Instantiate(_arrow, playerAttack.transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle + 90f)));
        playerAttack.PlayArrowClip(0.38f);
        Destroy(gameObject);
    }
}
