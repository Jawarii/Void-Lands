using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject lineIndicatorPrefab;
    public GameObject animationPrefab;
    public EnemyStats enemyStats;
    public float castTime = 1.5f;
    public IEnumerator InvokeSkill(Vector3 position, EnemyStats enemyStats, Vector3 scale, Quaternion rotation)
    {
        Vector3 targetPosition = position;
        Vector3 indicatorPosition = position;
        GameObject indicator = Instantiate(lineIndicatorPrefab, indicatorPosition, rotation);
        transform.localScale = scale;       
        indicator.transform.localScale = indicator.transform.localScale * scale.x;
        //indicator.transform.position *= scale.x;
        indicator.GetComponentInChildren<LineIndicatorBehaviour>().castTime = castTime;

        yield return new WaitForSeconds(castTime);

        GameObject _animation = Instantiate(animationPrefab, targetPosition, rotation);
        _animation.transform.localScale = scale;
        _animation.GetComponentInChildren<LineSkillDamageController>().enemyStats = enemyStats;
    }
}
