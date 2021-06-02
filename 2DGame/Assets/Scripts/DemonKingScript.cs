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
    public GameObject gameController;
    GameControllerScript gameControllerObject;

    public GameObject playerObject;
    Rigidbody2D demonKingRigidbody2D;

    public GameObject textObject;
    TextScript textObjectScript;


    ParticleSystem transformParticles;

    SpriteRenderer spriteObject;
    Color color;

    public string sceneLoaded;

    string currentScene;
    // Princess transform
    int transformPrincessCountDown = 0;
    int disapearPrincessCountDown = 0;
    int appearPrincessCountDown = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController");
        gameControllerObject = gameController.GetComponent<GameControllerScript>();
        spriteObject = GetComponent<SpriteRenderer>();
        currentScene = SceneManager.GetActiveScene().name;
        playerObject = GameObject.Find("Player");
        textObject = playerObject.GetComponent<PlayerController>().textBox;
        textObjectScript = playerObject.GetComponent<PlayerController>().textObject;
        transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        //Demon king TODO
        PrincessAppear("PrincessChamber", "DEMON: HA! Cannot find your dearest princess? Hmmm?");
        PrincessDisappear("PrincessChamber", "By the summer solstice. I am waiting.");
        if (gameControllerObject.currentText == "By the summer solstice. I am waiting. ")
        {
            StartCoroutine(HideAfter(3f));
        }

        PrincessAppear("Forest", "DEMON: BOO!");
        PrincessDisappear("Forest", "MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!!");

        PrincessAppear("Beach", "Your Majesty.");
        ChangeToPrincess("Beach", "//Change to Princess");
        PrincessDisappear("Beach", "Wait here until I return.");
        PrincessDisappear("Beach", "I will do what I want. That is none of your business.");

        PrincessAppear("DiningRoomFinale", "DEMON: Your Majesty.");
        ChangeToPrincess("DiningRoomFinale", "PRINCESS/DEMON: Father.");
        PrincessDisappear("DiningRoomFinale", "KING: NOOOO!");
        ChangeToPrincess("DiningRoomFinale", "Good Lord! My dearest, you have returned!");

        //Change to princess
        if (transformPrincessCountDown > 0)
        {
            --transformPrincessCountDown; 
            if (transformPrincessCountDown > 100)
            {
                color = spriteObject.color;
                if (color.a > 0)
                {
                    color.a = color.a - 0.02f;
                }
                spriteObject.color = color;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Art/Sprites/Characters/Princess", typeof(Sprite));
                gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                spriteObject.color = color;
                if (color.a < 1)
                {
                    color.a = color.a + 0.02f;
                }
                spriteObject.color = color;
                gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(true);
            }
        }

        //Princess Disappear
        if (disapearPrincessCountDown > 0)
        {
            --disapearPrincessCountDown;

            color = spriteObject.color;
            if (color.a > 0)
            {
                color.a = color.a - 0.02f;
            }
            spriteObject.color = color;

            if (disapearPrincessCountDown == 1)
            {
                gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(false);
            }
        }

        //Princess Appear
        if (appearPrincessCountDown > 0)
        {
            --appearPrincessCountDown;

            color = spriteObject.color;
            if (color.a < 1)
            {
                color.a = color.a + 0.02f;
            }
            spriteObject.color = color;
            gameObject.transform.GetComponent<BoxCollider2D>().gameObject.SetActive(true);
        }
    }

    public void ChangeToPrincess(string scene, string text)
    {
        if (currentScene == scene && transformPrincessCountDown == 0)
        {
            if (gameControllerObject.currentText == text)
            {
                Debug.Log("CHANGE PRINCESS  ");
                transformPrincessCountDown = 200;
                transformParticles.Play();
                gameControllerObject.currentText = text + " ";
            }
        }
    }
    public void PrincessAppear(string scene, string text)
    {
        if (currentScene == scene && appearPrincessCountDown == 0)
        {
            if (gameControllerObject.currentText == text)
            {
                Debug.Log("PRINCESS Appear ");
                appearPrincessCountDown = 150;
                transformParticles.Play();
                gameControllerObject.currentText = text + " ";
            }
        }
    }

    public void PrincessDisappear(string scene, string text)
    {
        if (currentScene == scene && disapearPrincessCountDown == 0)
        {
            if (gameControllerObject.currentText == text)
            {
                Debug.Log("PRINCESS DISAPEAR ");
                disapearPrincessCountDown = 400;
                transformParticles.Play();
                gameControllerObject.currentText = text + " ";
            }
        }
    }

    public void StartAppear()
    {
        StartCoroutine(AppearAfter(2.5f, gameObject.name));
    }

    public void PlayParticals()
    {
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
        textObjectScript.virtualActivationFuntion(dialogueText, playerObject.transform.position);
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
        textObjectScript.virtualActivationFuntion("KingInnerDialogueChamber", playerObject.transform.position);
        Debug.Log("TextObjectNotOption: " + textObjectScript.notOption);
        gameObject.SetActive(false);
    }
}
