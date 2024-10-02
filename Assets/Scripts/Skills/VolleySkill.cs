using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleySkill : SkillsScript
{
    public AnimationClip bowAnim;
    public AnimationClip finisherAnim;
    public AnimationClip bowReleaseAnim;
    public GameObject _arrow;
    public float adjustedSpeed = 0.1f;

    public override void ActivateSkill()
    {
        playerAttack.animTime = bowAnim.length * 9f / playerAttack.player.GetComponent<PlayerStats>().atkSpd + bowReleaseAnim.length;
        playerAttack.releaseTime = bowAnim.length * 9f / playerAttack.player.GetComponent<PlayerStats>().atkSpd;
        playerAttack.player.GetComponent<PlayerMovement>().speed *= adjustedSpeed;
        playerAttack.animator.SetBool("isAttacking", true);
        playerAttack.animator.SetFloat("AtkSpeed", playerAttack.player.GetComponent<PlayerStats>().atkSpd);
        playerAttack.player.GetComponent<PlayerMovement>().canDash = false;
        //playerAttack.player.GetComponent<PlayerMovement>().canMove = false;

        for (float i = 1; i < 10; i++)
        {
            StartCoroutine(SpawnArrow(i));
        }

    }
    IEnumerator SpawnArrow(float arrowNumber)
    {
        // Wait based on the animation time and attack speed
        yield return new WaitForSeconds(bowAnim.length * arrowNumber / playerAttack.player.GetComponent<PlayerStats>().atkSpd);

        // Get the mouse position and direction to the mouse
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.transform.position.z * -1));
        Vector3 directionToMouse = mouseWorldPosition - playerAttack.transform.position;

        // Calculate a random offset along the direction of the arrow's flight path
        float randomOffsetMagnitude = Random.Range(-0.18f, 0.18f); // Adjust range to control the amount of randomness
        Vector3 perpendicularOffset = new Vector3(-directionToMouse.y, directionToMouse.x).normalized * randomOffsetMagnitude;

        // Adjust the spawn position by adding the random perpendicular offset
        Vector3 spawnPosition = playerAttack.transform.position + perpendicularOffset;

        // Calculate the angle for the arrow rotation
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // Spawn the arrow with random positioning
        Instantiate(_arrow, spawnPosition, Quaternion.Euler(new Vector3(0f, 0f, angle + 90f)));

        // Play the arrow sound
        playerAttack.PlayArrowClip(0.38f);

        // Destroy the GameObject after the final arrow
        if (arrowNumber == 9)
            Destroy(gameObject);
    }

}
