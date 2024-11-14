using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

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
    public bool isBoss = false;
    public CinemachineVirtualCamera _camera;
    public float cameraOrthoSize;
    public float scaleModifier = 4f;
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        _camera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        cameraOrthoSize = _camera.m_Lens.OrthographicSize;
    }

    public void Setup(int damageAmount, float radius)
    {
        textMesh.SetText(damageAmount.ToString("#,#"));
        float newRadius = ((radius * 2) - 1f) * 0.8f;
        if (newRadius < 0)
            newRadius = 0;

        textMesh.fontSize += newRadius * textMesh.fontSize;
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
        transform.localScale = Vector3.one * cameraOrthoSize / scaleModifier;

    }

    public void Create(Vector3 pos, float radius, int damageAmount, bool isCritical)
    {
        isCrit = isCritical;
        floatDmgPf = transform;
        if (isPlayer)
        {
            moveVector = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.35f, 0.55f));
        }
        else
        {
            moveVector = new Vector3(Random.Range(-0.45f * radius, 0.45f * radius), Random.Range(0.8f * radius, radius));
        }
        Transform damagePopupTransform = Instantiate(floatDmgPf, pos + moveVector, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, radius);
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
