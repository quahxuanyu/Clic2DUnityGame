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
    public string currentTextObjectName;
    public TextMeshProUGUI displayText;
    public GameObject optionBox;
    public OptionScript optionObject;
    public bool hasNextPage = false;
    public bool hasNextOption = false;
    public bool notOption = true;
    public int currentPage = 0;
    public List<string> options = new List<string>();
    public string optionTree = "";
    
    // Start is called before the first frame update
    void Start()
    {
        optionObject = optionBox.GetComponent<OptionScript>();
        gameObject.SetActive(false);
        //Get raw text into a dictionary 
        texts = rawTexts.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim().Replace(@"\\n", Environment.NewLine))
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToDictionary(line => line.Split('@')[0], line => line.Split('@')[1]);
        displayText = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    //DISPLAY DIALOG
    public void DisplayDialog(bool state)
    {
        gameObject.SetActive(state);
        if (notOption && state)
        {
            //Check if text is a normal text
            if (texts.ContainsKey(currentTextObjectName + optionTree + (currentPage + 1).ToString()))
            {
                hasNextPage = true;
                hasNextOption = false;
                currentPage += 1;
            }
            //Check if text if a option
            else if (texts.ContainsKey(currentTextObjectName + optionTree + (currentPage + 1).ToString() + "O"))
            {
                hasNextPage = true;
                hasNextOption = true;
                notOption = false;
                currentPage += 1;
            }
            //No text
            else
            {
                optionTree = "";
                hasNextPage = false;
                hasNextOption = false;
                currentPage = 0;
                gameObject.SetActive(false);
            }
        }
        else
        {
            optionObject.gameObject.SetActive(state);
        }

        //if it's a option
        if (hasNextOption && hasNextPage)
        {
            notOption = false;
            options = texts[currentTextObjectName + optionTree + (currentPage).ToString() + "O"].Split('|').ToList();
            optionObject.DisplayOption(true, options);
        }
        //if it's not a option, a normal text
        else if (hasNextPage)
        {
            displayText.SetText(texts[currentTextObjectName + optionTree + (currentPage).ToString()]);
        }
    }
}
