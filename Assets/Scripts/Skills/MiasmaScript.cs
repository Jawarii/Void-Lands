using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiasmaScript : SkillsScript
{
    public float coolDown = 5f;
    public float cdElapsedTime = 0f;
    public float duration = 5f;
    public GameObject _bow;

    public AudioSource audioSource;
    public AudioClip audioClip;

    // Reference to the prefab you want to instantiate
    public GameObject effectPrefab;

    // Reference to the instantiated effect object
    private GameObject instantiatedEffect;

    public override void ActivateSkill()
    {
        playerAttack.hasImbueBuff = true;
        StartCoroutine(DisableBuff());
        audioSource = GameObject.Find("BuffsSoundSource").GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 playerPos = playerTransform.position;
        Vector3 animatationPos = new Vector3(playerPos.x, playerPos.y + 0.35f, playerPos.z);
        // Instantiate the effectPrefab at the player's location and make the player its parent
        instantiatedEffect = Instantiate(effectPrefab, animatationPos, Quaternion.identity, playerTransform);
    }

    public IEnumerator DisableBuff()
    {
        yield return new WaitForSeconds(duration);
        if (instantiatedEffect != null)
        {
            Destroy(instantiatedEffect);
        }
        playerAttack.hasImbueBuff = false;
        Destroy(gameObject);
    }
}
