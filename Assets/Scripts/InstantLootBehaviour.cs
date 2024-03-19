using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class InstantLootBehaviour : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    public GameObject popupPrefab;
    public GameObject inventory;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("InstantLoot"))
        {
            source.Stop();  
            source.PlayOneShot(clip);
            GameObject popup = Instantiate(popupPrefab, other.gameObject.transform.position, other.gameObject.transform.rotation);
            popup.gameObject.GetComponentInChildren<TMP_Text>().text = "+" + other.gameObject.GetComponentInChildren<TMP_Text>().text;
            inventory.GetComponent<InventoryController>().goldAmount += other.gameObject.GetComponent<GoldDropBehaviour>().goldAmount;
            Destroy(other.gameObject);
        }
    }
}
