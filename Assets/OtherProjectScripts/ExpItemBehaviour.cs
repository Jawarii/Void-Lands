using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItemBehaviour : MonoBehaviour
{
    public GameObject player;
    public float speed = 8.0f;
    public int expValue = 1;
    float dist = 0;
    private bool inRange = false;
    private bool isAquired = false;
    private float animTime = 0.2f;
    private Vector2 posOrigin;
    float distLimit = 0;
    private PlayerStats playerStats;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        posOrigin = gameObject.transform.position;
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (inRange)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            dist = Vector3.Distance(player.transform.position, transform.position);

            if (animTime <= 0)
            {
                transform.position += direction * speed * Time.deltaTime;

                if (dist <= 0.5f)
                {
                    playerStats.IncreaseExp(expValue);
                    Destroy(gameObject);
                }
            }
            else
            {   
                distLimit = Vector3.Distance(posOrigin, transform.position);
                transform.position -= direction * 3 * Time.deltaTime;
                             
                animTime -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUpRange"))
        {
            if (!isAquired)
            {
                transform.localScale *= 1.3f;              
                isAquired = true;
            }
            inRange = true;
        }
    }
}
