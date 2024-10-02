using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelShotSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float arrowSpacing = 1.0f; // Spacing between arrows
    public float adjustedSpeed = 0.3f;
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
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        Vector3 directionToMouse = (mouseWorldPosition - playerAttack.transform.position).normalized;

        // Calculate perpendicular direction for parallel arrow positions
        Vector3 perpendicularDirection = Vector3.Cross(directionToMouse, Vector3.forward).normalized;

        // Spawn the center arrow
        Instantiate(_arrow, playerAttack.transform.position, Quaternion.LookRotation(Vector3.forward, -directionToMouse));

        // Spawn arrows on the left and right
        for (int i = 1; i <= 3; i++)
        {
            Vector3 leftPosition = playerAttack.transform.position - perpendicularDirection * arrowSpacing * i;
            Vector3 rightPosition = playerAttack.transform.position + perpendicularDirection * arrowSpacing * i;
            Instantiate(_arrow, leftPosition, Quaternion.LookRotation(Vector3.forward, -directionToMouse));
            Instantiate(_arrow, rightPosition, Quaternion.LookRotation(Vector3.forward, -directionToMouse));
        }

        playerAttack.PlayArrowClip(0.38f);
        Destroy(gameObject);
    }
}
