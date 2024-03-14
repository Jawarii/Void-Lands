using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float hp = 100.0f;
    public float damage = 2.0f;
    public float timeSince = 0;
    // Start is called before the first frame update
    void Update()
    {
        timeSince += Time.deltaTime;
        if (Input.GetKeyDown("space"))
        {
            timeSince = 0;
            if (hp > 0)
            {
                hp -= damage;
                if (hp < 0)
                    hp = 0;
            }
        }
    }
}
