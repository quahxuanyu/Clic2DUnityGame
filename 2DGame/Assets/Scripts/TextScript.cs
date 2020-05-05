using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextScript : MonoBehaviour
{
    public Vector2 interactablePos;
    public TextAsset rawTexts;
    Dictionary<string, string> texts = new Dictionary<string, string>();
    public string currentText;
    TextMeshProUGUI displayText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        texts = rawTexts.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim().Replace(@"\\n", Environment.NewLine))
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToDictionary(line => line.Split('@')[0], line => line.Split('@')[1]);
        displayText = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //DISPLAY DIALOG
    public void DisplayDialog(bool state)
    {
        displayText.SetText(texts[currentText]);
        gameObject.SetActive(state);
    }

    //string GetText(string name)
    //{
        
    //}
}
