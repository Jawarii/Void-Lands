using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgroBehaviour : MonoBehaviour
{
    //public GameObject agroRadius;
    public GameObject enemy;
    void Awake()
    {
        enemy = transform.parent.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           EnemyMovementController enemyMovement = enemy.GetComponent<EnemyMovementController>();
           enemyMovement.inPursuit = true;
           gameObject.SetActive(false);
        }
    }
}
