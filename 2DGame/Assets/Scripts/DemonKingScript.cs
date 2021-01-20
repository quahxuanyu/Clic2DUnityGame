using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class DemonKingScript : MonoBehaviour
{
    public GameObject playerObject;
    Rigidbody2D demonKingRigidbody2D;

    public GameObject textObject;
    TextScript textObjectScript;

    string currentText;

    SpriteRenderer spriteObject;
    Color color;

    public string sceneLoaded;

    // Start is called before the first frame update
    void Start()
    {
        spriteObject = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (sceneLoaded == "PrincessChamber")
        {
            if (!playerObject)
            {
                playerObject = GameObject.Find("Player");
                demonKingRigidbody2D = gameObject.GetComponent<Rigidbody2D>();

                textObject = GameObject.Find("TextBox");
                textObjectScript = textObject.GetComponent<TextScript>();
            }

            currentText = textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (currentText == "By the summer solstice. I am waiting.")
            {
                color = spriteObject.color;
                color.a = 0;
                spriteObject.color = color;
                StartCoroutine(HideAfter(3f));
            }
        }

        if (sceneLoaded == "Forest")
        {
            demonKingRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            textObjectScript = textObject.GetComponent<TextScript>();
            currentText = textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            if (currentText == "DEMON: BOO!")
            {
                color = spriteObject.color;
                color.a = 255;
                spriteObject.color = color;
                gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(true);
            }

            if (currentText == "MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!!")
            {
                color = spriteObject.color;
                color.a = 0;
                spriteObject.color = color;
                gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(false);
            }
        }
    }

    public void StartAppear()
    {
        StartCoroutine(AppearAfter(2.5f));
    }

    IEnumerator AppearAfter(float time)
    {
        // Wait for an amount of time before appearing and displaying next dialogue
        yield return new WaitForSeconds(time);
        //Make sure all the dialogue is reset...
        textObjectScript.optionTree = "";
        textObjectScript.hasNextOption = false;
        textObjectScript.hasNextPage = false;
        textObjectScript.virtualActivation = false;
        textObjectScript.notOption = true;
        textObjectScript.currentPage = 0;
        textObjectScript.DisplayDialog(false);
        //Turn them back on with new dialogue!
        textObjectScript.interactablePos = playerObject.transform.position;
        textObjectScript.currentTextObjectName = gameObject.name;
        textObjectScript.virtualActivation = true;
        textObjectScript.DisplayDialog(true);
        demonKingRigidbody2D.MovePosition(new Vector2(3, -3));
        yield return new WaitForSeconds(0.1f);
        demonKingRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    IEnumerator HideAfter(float time)
    {
        Debug.Log("demon king START");
        yield return new WaitForSeconds(time);
        Debug.Log("demon king END");
        //Make sure all the dialogue is reset...
        textObjectScript.optionTree = "";
        textObjectScript.hasNextOption = false;
        textObjectScript.hasNextPage = false;
        textObjectScript.virtualActivation = false;
        textObjectScript.notOption = true;
        textObjectScript.currentPage = 0;
        textObjectScript.DisplayDialog(false);
        //Turn them back on with new dialogue!
        textObjectScript.interactablePos = playerObject.transform.position;
        textObjectScript.currentTextObjectName = "KingInnerDialogueChamber";
        textObjectScript.virtualActivation = true;
        textObjectScript.DisplayDialog(true);
        Debug.Log("TextObjectNotOption: " + textObjectScript.notOption);
        gameObject.SetActive(false);
    }
}
