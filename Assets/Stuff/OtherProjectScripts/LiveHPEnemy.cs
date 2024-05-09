using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class LiveHPEnemy : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float maxHp1 = 0;
    public float hp1 = 0;
    public Camera camera;
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            target = transform.parent.parent;
        }
        stats = target.gameObject;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        slider.value = hp1 / maxHp1;
        camera = Camera.main;   
    }

    // Update is called once per frame
    void Update()
    {
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        slider.value = hp1 / maxHp1;
        transform.rotation = camera.transform.rotation;
       // transform.position = target.position + offset;

        if (target.localScale.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

}
