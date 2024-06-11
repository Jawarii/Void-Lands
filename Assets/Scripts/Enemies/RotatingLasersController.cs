using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLasersController : MonoBehaviour
{
    public GameObject lineIndicatorPrefab;
    public GameObject animationPrefab;
    public GameObject _animation;
    public EnemyStats enemyStats;
    public float castTime = 1.5f;
    public float rotationSpeed = 30f; // Rotation speed in degrees per second
    public GameObject bossGo;

    public IEnumerator InvokeSkill(Vector3 position, EnemyStats enemyStats, Vector3 scale, Quaternion rotation)
    {
        bossGo = GameObject.Find("RelicBoss");
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

    public IEnumerator HandleRotation(bool rotateClockwise)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 3)
        {
            if (_animation != null)
            {
                // Calculate rotation step
                float rotationStep = rotationSpeed * Time.deltaTime * (rotateClockwise ? -1 : 1);
                // Apply rotation
                _animation.transform.Rotate(0, 0, rotationStep);

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
