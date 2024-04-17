using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingArrowSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public override void ActivateSkill()
    {
        playerAttack.animTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd + bowReleaseAnim.length;
        playerAttack.releaseTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd;
        playerAttack.player.GetComponent<PlayerMovement>().speed = playerAttack.adjustedSpeed;
        playerAttack.animator.SetBool("isAttacking", true);
        playerAttack.player.GetComponent<PlayerMovement>().canDash = false;
        playerAttack.player.GetComponent<PlayerMovement>().canMove = false;
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
