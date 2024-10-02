using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultishotSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float adjustedSpeed = 0.2f;
    public override void ActivateSkill()
    {
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

        for (int i = 0; i < 5; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 70f + i * 10f);
            Instantiate(_arrow, playerAttack.transform.position, rotation);
        }

        playerAttack.PlayArrowClip(0.38f);
        Destroy(gameObject);

    }
}
