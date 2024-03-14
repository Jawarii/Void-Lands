using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public float goldAmount = 0;
    public TMP_Text goldAmountText;
    public List<GameObject> inventory = new List<GameObject>();
    void Start()
    {
        goldAmountText.text = goldAmount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        goldAmountText.text = goldAmount.ToString();
    }
}
