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
    private InventoryController inventoryController;
    private GoldDropBehaviour goldDropBehaviour;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("InstantLoot"))
        {
            //source.Stop();  
            source.PlayOneShot(clip);
            GameObject popup = Instantiate(popupPrefab, other.gameObject.transform.position + new Vector3(0.0f, 0.45f, 0.0f), Quaternion.identity);
            popup.gameObject.GetComponentInChildren<TMP_Text>().text = "+" + other.gameObject.GetComponentInChildren<TMP_Text>().text;
            inventoryController = inventory.GetComponent<InventoryController>();
            goldDropBehaviour = other.gameObject.GetComponent<GoldDropBehaviour>();
            inventoryController.AddGold(goldDropBehaviour.goldAmount);
            Destroy(other.gameObject);
        }
    }
}
