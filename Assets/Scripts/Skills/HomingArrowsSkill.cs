using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingArrowsSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip finisherAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float adjustedSpeed = 0.4f;
    public override void ActivateSkill()
    {
        playerAttack.animTime = bowAnim.length * 5f / playerAttack.player.GetComponent<PlayerStats>().atkSpd + bowReleaseAnim.length;
        playerAttack.releaseTime = bowAnim.length * 5f / playerAttack.player.GetComponent<PlayerStats>().atkSpd;
        playerAttack.player.GetComponent<PlayerMovement>().speed *= adjustedSpeed;
        playerAttack.animator.SetBool("isAttacking", true);
        playerAttack.animator.SetFloat("AtkSpeed", playerAttack.player.GetComponent<PlayerStats>().atkSpd);
        playerAttack.player.GetComponent<PlayerMovement>().canDash = false;
        //playerAttack.player.GetComponent<PlayerMovement>().canMove = false;

        for (float i = 1; i < 6; i++)
        {
            StartCoroutine(SpawnArrow(i));
        }

    }
    IEnumerator SpawnArrow(float arrowNumber)
    {
        yield return new WaitForSeconds(bowAnim.length * arrowNumber / playerAttack.player.GetComponent<PlayerStats>().atkSpd);

        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.z * -1));
        Vector3 directionToMouse = mouseWorldPosition - playerAttack.transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        Instantiate(_arrow, playerAttack.transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle + 90f)));
        playerAttack.PlayArrowClip(0.38f);

        if (arrowNumber == 5)
            Destroy(gameObject);
    }
}
