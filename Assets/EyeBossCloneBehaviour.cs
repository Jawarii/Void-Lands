using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBossCloneBehaviour : MonoBehaviour
{
    public List<GameObject> patterns = new List<GameObject>();
    public Vector3 spawnPosition;
    public float castTime = 3f;
    private ExplosionController purpleExplosionController;
    private BossMovementController movementController;

    void Start()
    {
        movementController = GetComponent<BossMovementController>();
        purpleExplosionController = patterns[0].GetComponent<ExplosionController>();
        movementController.inPursuit = true;
        LargeExplosion();
        Destroy(gameObject, castTime);
    }

    public void LargeExplosion()
    {
        EnemyStats enemyStats = transform.GetComponent<EnemyStats>();
        Vector3 scale = new Vector3(15f, 15f, 1);

        Vector3 spawnPosition = transform.position;

        StartCoroutine(purpleExplosionController.InvokeSkill(spawnPosition, enemyStats, scale));
    }
}
