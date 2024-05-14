using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public GameObject circleIndicatorPrefab;
    public GameObject animationPrefab;
    public EnemyStats enemyStats;
    public float castTime = 1.5f;
    public IEnumerator InvokeSkill(Vector3 position, EnemyStats enemyStats, Vector3 scale)
    {        
        Vector3 targetPosition = position;
        GameObject indicator = Instantiate(circleIndicatorPrefab, targetPosition, Quaternion.identity);
        transform.localScale = scale;
        indicator.transform.localScale = scale;
        indicator.GetComponent<CircleIndicatorBehaviour>().castTime = castTime;
        yield return new WaitForSeconds(castTime);
        GameObject _animation = Instantiate(animationPrefab, targetPosition, Quaternion.identity);
        _animation.transform.localScale = scale;
        _animation.GetComponent<CircularSkillDamageController>().enemyStats= enemyStats;
    }
}
