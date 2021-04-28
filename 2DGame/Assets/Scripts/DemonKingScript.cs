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

    string currentScene;
    // Princess transform
    int transformPrincessCountDown = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteObject = GetComponent<SpriteRenderer>();
        currentScene = SceneManager.GetActiveScene().name;
        playerObject = GameObject.Find("Player");
        textObject = playerObject.GetComponent<PlayerController>().textBox;
        textObjectScript = playerObject.GetComponent<PlayerController>().textObject;
    }

    void Update()
    {
        currentText = textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
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
                PlayParticals();
                currentText = "DEMON: BOO! ";
            }

            if (currentText == "MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!!")
            {
                color = spriteObject.color;
                color.a = 0;
                spriteObject.color = color;
                gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(false);
                PlayParticals();
                currentText = "MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!! ";
            }
        }

        if (currentScene == "Beach")
        {
            if (currentText == "//Change to Princess")
            {
                Debug.Log("CHANGE PRINCESS  ");
                transformPrincessCountDown = 150;
                var transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
                transformParticles.Play();
                currentText = "//Change to Princess ";
                currentScene = "NotBeach";
            }
        }

        if (transformPrincessCountDown > 0)
        {
            --transformPrincessCountDown;
            if (transformPrincessCountDown == 1)
            {
                var transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
                transformParticles.Stop();
            }

            if (transformPrincessCountDown == 20)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Art/Sprites/Characters/Princess", typeof(Sprite));
                gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }
    }

    public void StartAppear()
    {
        StartCoroutine(AppearAfter(2.5f, gameObject.name));
    }

    public void Beach()
    {
        StartCoroutine(AppearAfter(2.5f, "1DemonKingBeach", -4.75f, -2.35f));
    }

    public void PlayParticals()
    {
        var transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
        transformParticles.Play();
        transformPrincessCountDown = 200;
    }

    public IEnumerator AppearAfter(float time, string dialogueText = "", float x = 3, float y = -3)
    {
        // Wait for an amount of time before appearing and displaying next dialogue
        Debug.Log("HERE!");
        yield return new WaitForSeconds(time);
        Debug.Log("WHOO HOO");
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
        textObjectScript.currentTextObjectName = dialogueText;
        textObjectScript.virtualActivation = true;
        textObjectScript.DisplayDialog(true);
        demonKingRigidbody2D.MovePosition(new Vector2(x, y));
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
