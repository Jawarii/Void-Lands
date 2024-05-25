using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingLaserController : MonoBehaviour
{
    public GameObject lineIndicatorPrefab;
    public GameObject animationPrefab;
    public GameObject _animation;
    public EnemyStats enemyStats;
    public float castTime = 1.5f;
    public float rotationSpeed = 90f; // Rotation speed in degrees per second
    public GameObject player; // Reference to the player GameObject
    public GameObject bossGo;

    public IEnumerator InvokeSkill(Vector3 position, EnemyStats enemyStats, Vector3 scale, Quaternion rotation)
    {
        bossGo = GameObject.Find("RelicBoss");
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetPosition = position;
        Vector3 indicatorPosition = position;
        GameObject indicator = Instantiate(lineIndicatorPrefab, indicatorPosition, rotation);
        transform.localScale = scale;
        indicator.transform.localScale = indicator.transform.localScale * scale.x;
        indicator.transform.position *= scale.x;
        indicator.GetComponentInChildren<LineIndicatorBehaviour>().castTime = castTime;

        yield return new WaitForSeconds(castTime);

        _animation = Instantiate(animationPrefab, targetPosition, rotation);
        _animation.transform.localScale = scale;
        _animation.GetComponentInChildren<LineSkillDamageController>().enemyStats = enemyStats;
    }
    public IEnumerator HandleRotation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 3)
        {
            if (player != null && _animation != null)
            {
                // Calculate direction to the player
                Vector3 directionToPlayer = (player.transform.position - _animation.transform.position).normalized;
                // Calculate the target rotation
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 90f; // Adjust for correct orientation
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                // Rotate towards the target rotation
                _animation.transform.rotation = Quaternion.RotateTowards(_animation.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Send the current rotation of the animation laser to SetAnimatorBasedOnAngle
                float currentRotationAngle = _animation.transform.rotation.eulerAngles.z - 90;
                currentRotationAngle = NormalizeAngle(currentRotationAngle);
                bossGo.GetComponent<BossMovementController>().SetAnimatorBasedOnAngle(currentRotationAngle);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
