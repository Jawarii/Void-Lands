using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayedHPEnemy : MonoBehaviour
{
    public Slider slider;
    public GameObject stats;
    public float hp1 = 0;
    public float maxHp1 = 0;
    public float oldSlider = 0;
    private bool updt = true;
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

    void Update()
    {
        maxHp1 = stats.GetComponent<EnemyStats>().maxHp;
        hp1 = stats.GetComponent<EnemyStats>().hp;
        if (stats.GetComponent<EnemyStats>().timeSince >= 0.3)
        {
            if (updt)
            {
                oldSlider = slider.value;
                updt = false;
            }
            if (slider.value != hp1 / maxHp1)
            {
                slider.value -= (oldSlider - (hp1 / maxHp1)) * Time.deltaTime * 20;
                if (slider.value < hp1 / maxHp1)
                {
                    slider.value = hp1 / maxHp1;
                    updt = true;
                }
            }
            else updt = true;
        }
        transform.rotation = camera.transform.rotation;
       // transform.position = target.position + offset;

        if (target.localScale.x < 0)
        {
            transform.localScale = new Vector3(-1f* Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
