using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Transform floatDmgPf;
    private float lifeTime;
    private Color textColor;
    private const float DISAPPEAR_TIMER_MAX = 0.5f;
    private Vector3 moveVector;
    private static int sortingOrder;
    public bool isCrit = false;
    public bool isPlayer = false;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    
    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString("#,#"));
        if (isCrit)
        {
            textMesh.color = Color.yellow;
            textMesh.fontSize *= 1.3f;
        }
        else if (isPlayer)
        {
            textMesh.color = Color.red;
        }
        else
        {
            textMesh.color = Color.white;
        }
        lifeTime = DISAPPEAR_TIMER_MAX;    
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    public void Create(Vector3 pos, int damageAmount, bool isCritical)
    {
        isCrit = isCritical;
        floatDmgPf = transform;
        if (isPlayer)
        {
            moveVector = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.35f, 0.55f));
        }    
        else
        {
            moveVector = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.1f, 0.3f));
        }
        Transform damagePopupTransform = Instantiate(floatDmgPf, pos + moveVector, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);
    }
    public void Update()
    {
        if (isCrit)
        {
            if (lifeTime > DISAPPEAR_TIMER_MAX * 0.85f)
            {
                float increaseScaleAmount = 10f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else if (lifeTime >= DISAPPEAR_TIMER_MAX * 0.6f && lifeTime <= DISAPPEAR_TIMER_MAX * 0.75f)
            {
                float decreaseScaleAmount = 10f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }
        }
        else
        {
            if (lifeTime > DISAPPEAR_TIMER_MAX * 0.85f)
            {
                float increaseScaleAmount = 6f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else if (lifeTime >= DISAPPEAR_TIMER_MAX * 0.6f && lifeTime <= DISAPPEAR_TIMER_MAX * 0.75f)
            {
                float decreaseScaleAmount = 6f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }
        }

        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
