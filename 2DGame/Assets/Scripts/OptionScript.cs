using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionScript : MonoBehaviour
{
    public int numOfButtons;
    public GameObject textBox;
    public Button buttonPrefab;
    public TextScript textObject;
    Button currentButton;
    TextMeshProUGUI optionTextObject;
    TextMeshProUGUI questionTextObject;
    private object createButton;
 
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        textObject = textBox.GetComponent<TextScript>();
        questionTextObject = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //DISPLAY OPTION
    public void DisplayOption(bool state, List<string> list)
    {
        numOfButtons = list.Count - 1;
        questionTextObject.SetText(list[0]);
        RectTransform size = (RectTransform)gameObject.transform;
        //Instantiate buttons
        for (int i = 1; i < list.Count; i++)
        {
            currentButton = Instantiate(buttonPrefab, new Vector2((size.rect.width / list.Count - 1) * i, (size.rect.height / 2) - 35f), Quaternion.identity, gameObject.transform);
            optionTextObject = currentButton.GetComponentInChildren<TextMeshProUGUI>();
            currentButton.name = list[i];
            optionTextObject.text = list[i];
        }
        gameObject.SetActive(state);
    }
}
