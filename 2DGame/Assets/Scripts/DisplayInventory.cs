using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public GameObject player;
    RubyController playerObject;
    string text;
    TextMeshProUGUI displayText;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = player.GetComponent<RubyController>();
        displayText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text = "Inventory: " + String.Join(" ", playerObject.inventoryAmount.Keys.ToArray().Where(K => playerObject.inventoryAmount[K] > 0));
        displayText.SetText(text);
    }
}
