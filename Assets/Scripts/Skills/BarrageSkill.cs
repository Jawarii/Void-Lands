using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageSkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float arrowSpacing = 1.0f; // Spacing between arrows
    public float duration = 3.0f; // Total duration for spawning arrows
    public int totalArrows = 20; // Total number of arrows to spawn
    public float radius = 5.0f; // Radius around the player to spawn arrows
    public float adjustedSpeed = 0.35f;

    public override void ActivateSkill()
    {
        playerAttack.animTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd + bowReleaseAnim.length;
        playerAttack.releaseTime = bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd;
        playerAttack.player.GetComponent<PlayerMovement>().speed *= adjustedSpeed;
        playerAttack.animator.SetBool("isAttacking", true);
        playerAttack.animator.SetFloat("AtkSpeed", playerAttack.player.GetComponent<PlayerStats>().atkSpd);
        playerAttack.player.GetComponent<PlayerMovement>().canDash = false;
        //playerAttack.player.GetComponent<PlayerMovement>().canMove = false;
        StartCoroutine(SpawnArrowsOverTime());
    }

    IEnumerator SpawnArrowsOverTime()
    {
        yield return new WaitForSeconds(bowAnim.length / playerAttack.player.GetComponent<PlayerStats>().atkSpd);
        float timeBetweenArrows = duration / totalArrows;
        float timeElapsed = 0.0f;

        for (int i = 0; i < totalArrows; i++)
        {
            // Increase frequency over time
            timeBetweenArrows = Mathf.Lerp(0.1f, duration / totalArrows, 1 - (timeElapsed / duration));

            // Random position within a radius around the player
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = playerAttack.transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

            // Calculate direction in front of the player
            Vector3 directionToMouse = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)) - playerAttack.transform.position).normalized;

            // Instantiate arrow
            Instantiate(_arrow, spawnPosition, Quaternion.LookRotation(Vector3.forward, -directionToMouse));

            playerAttack.PlayArrowClip(0.38f);

            yield return new WaitForSeconds(timeBetweenArrows);
            timeElapsed += timeBetweenArrows;
        }

        playerAttack.animator.SetBool("isAttacking", false);
        Destroy(gameObject);
    }
}
