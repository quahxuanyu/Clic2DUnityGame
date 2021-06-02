using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ServantMoveScript : MonoBehaviour
{
    Rigidbody2D rigidBody2D;

    //Text Variebles
    public GameObject textObject;
    private TextScript textObjectScript;
    public GameObject gameController;
    GameControllerScript gameControllerObject;

    //player and demon king Variables
    GameObject playerObject;
    GameObject demonKing;
    DemonKingScript demonKingObjectScript;

    //Movement Sequence
    List<Vector2> servantCallMovement = new List<Vector2>{
        new Vector2(0f, -5f),
        new Vector2(8.1f, 0f),
        new Vector2(0f, -0.8f)
    };
    List<Vector2> servantToDoorMovement = new List<Vector2>{
        new Vector2(0f, 0.8f),
        new Vector2(-8.1f, 0f),
        new Vector2(0f, 4.5f)
    };
    List<Vector2> servantCorridorMovement = new List<Vector2>{
        new Vector2(8.3f, 0f),
        new Vector2(0f, 0.2f)
    };
    List<Vector2> servantChamberMovement = new List<Vector2>{
        new Vector2(1.0f, 0f),
        new Vector2(0f, 0.1f)
    };
    List<Vector2> servantChamberMovement2 = new List<Vector2>{
        new Vector2(0f, -1f)
    };
    //previosPosition is use to calculate the distance traveled
    Vector2 previosPosition;
    int currentMoveSequence = 0;

    //Moveing speed variable
    float speed = 6f;
    //the total distance moved
    float totalDistaced = 0f;
    //Moving variable, to check if a movement is true
    bool moving = false;
    //the position it is aiming to get
    Vector2 currentTargetPoint;
    //a rounded float position of this rigidbody
    Vector2 RigidbodyPosition;
    //the moving direction that changes depending on the current moveSequence
    Vector2 currentMoveDirection;

    SpriteRenderer spriteObject;
    Color color;

    //Variables for checking if a list of movement has been made
    List<List<Vector2>> currentArray =  new List<List<Vector2>>();

    //Variable for checking if a movement is finished
    List<string> activatedMovements = new List<string>();

    void Start()
    {
        gameController = GameObject.Find("GameController");
        gameControllerObject = gameController.GetComponent<GameControllerScript>();
        playerObject = GameObject.Find("Player");

        if (SceneManager.GetActiveScene().name == "PrincessChamber")
        {
            demonKing = GameObject.Find("DemonKing");
            demonKingObjectScript = demonKing.GetComponent<DemonKingScript>();
        }

        spriteObject = GetComponent<SpriteRenderer>();

        rigidBody2D = GetComponent<Rigidbody2D>();

        textObjectScript = textObject.GetComponent<TextScript>();

        //initialize for the first movement
        previosPosition = rigidBody2D.position;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO
        if (SceneManager.GetActiveScene().name == "DiningRoom")
        {
            if (gameControllerObject.currentText == "Servant!" || moving && !activatedMovements.Contains("Servant!"))
            {
                Debug.Log("One");
                NPCMovement(servantCallMovement, false, false);
            }
            else if (gameControllerObject.currentText == "*Sevant Leaves...*" || moving && !activatedMovements.Contains("*Sevant Leaves...*"))
            {
                Debug.Log("two");
                NPCMovement(servantToDoorMovement, true, true);
            }
        }

        if (SceneManager.GetActiveScene().name == "Corridor")
        {
            if (gameControllerObject.currentText == "Follow me, your majesty..." || moving && !activatedMovements.Contains("Follow me, your majesty..."))
            {
                Debug.Log("three");
                NPCMovement(servantCorridorMovement, true, false);
            }
        }

        if (SceneManager.GetActiveScene().name == "PrincessChamber")
        {
            if (gameControllerObject.currentText == "Leave, servant." || moving && !activatedMovements.Contains("Leave, servant."))
            {
                Debug.Log("four");
                NPCMovement(servantChamberMovement, true, false);
            }
            else if (gameControllerObject.currentText == "Servant!!" || moving && !activatedMovements.Contains("Servant!!"))
            {
                NPCMovement(servantChamberMovement2, false, false);
            }
        }
    }

    //Moving function (movement list, hide the object at the end of the movement, hide the textBox and once reached destination increament and turn on)
    public void NPCMovement(List<Vector2> moveArray, bool hide, bool hideAndIncrement)
    {
        //currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveArray[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveArray[currentMoveSequence].y) * 10f) / 10f);
        RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);

        //Initailize first movement
        if (!currentArray.Contains(moveArray))
        {
            Debug.Log("Initialize");
            color = spriteObject.color;
            color.a = 255;
            spriteObject.color = color;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Sprites/Characters/Servent");
            totalDistaced = 0f;
            currentMoveSequence = 0;
            currentArray.Add(moveArray);
            previosPosition = rigidBody2D.position;
            currentMoveDirection = new Vector2(0, 0);
            currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveArray[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveArray[currentMoveSequence].y) * 10f) / 10f);
            moving = true;
            Debug.Log(currentTargetPoint);
            Debug.Log(RigidbodyPosition);
            Debug.Log(rigidBody2D.position);
            //get first movement directions
            if (RigidbodyPosition.x - currentTargetPoint.x < 0 && currentTargetPoint.x != 0f)
            {
                currentMoveDirection += new Vector2(1f, 0f);
            }
            else if (RigidbodyPosition.x - currentTargetPoint.x != 0)
            {
                currentMoveDirection += new Vector2(-1f, 0f);
            }

            if (RigidbodyPosition.y - currentTargetPoint.y < 0 && currentTargetPoint.y != 0f)
            {
                currentMoveDirection += new Vector2(0f, 1f);
            }
            else if (RigidbodyPosition.y - currentTargetPoint.y != 0)
            {
                currentMoveDirection += new Vector2(0f, -1f);
            }
            
        }

        if (hideAndIncrement && totalDistaced > 10f && totalDistaced < 11f)
        {
            Debug.Log("Possible Point");
            textObject.SetActive(false);
        }

        //Check if equals CURRENT target point
        if (RigidbodyPosition == currentTargetPoint)
        {
            //Check if it's NOT last movement, continue to the next movement
            if (currentMoveSequence != moveArray.Count - 1)
            {
                currentMoveSequence++;
                Debug.Log(currentMoveSequence);
                previosPosition = rigidBody2D.position;
                currentMoveDirection = new Vector2(0f, 0f);
                currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveArray[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveArray[currentMoveSequence].y) * 10f) / 10f);

                //Get directions of where it should be going
                if (RigidbodyPosition.x - currentTargetPoint.x < 0 && currentTargetPoint.x != 0f)
                {
                    currentMoveDirection += new Vector2(1f, 0f);
                }
                else if (RigidbodyPosition.x - currentTargetPoint.x != 0)
                {
                    currentMoveDirection += new Vector2(-1f, 0f);
                }

                if (RigidbodyPosition.y - currentTargetPoint.y < 0 && currentTargetPoint.y != 0f)
                {
                    currentMoveDirection += new Vector2(0f, 1f);
                }
                else if (RigidbodyPosition.y - currentTargetPoint.y != 0)
                {
                    currentMoveDirection += new Vector2(0f, -1f);
                }
            }
            //If it's the last movement in the given list of movements
            else
            {
                Debug.Log("Reached!");
                rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                activatedMovements.Add(gameControllerObject.currentText);
                moving = false;
                if (hideAndIncrement && textObject.activeInHierarchy == false)
                {
                    Debug.Log("End is true");
                    StartCoroutine(WaitFuntion(3.0f));
                }
                
                if (hide)
                {
                    color = spriteObject.color;
                    color.a = 0;
                    spriteObject.color = color;
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    if (!hideAndIncrement)
                    {
                        if (SceneManager.GetActiveScene().name == "PrincessChamber")
                        {
                            demonKingObjectScript.StartAppear();
                        }
                    }
                }
            }
        }
        //Moving!
        else
        {
            Debug.Log("MOVINGG  " + currentTargetPoint + RigidbodyPosition + currentMoveDirection);
            Debug.Log("Total Distace moved: " + totalDistaced);
            rigidBody2D.constraints = RigidbodyConstraints2D.None;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidBody2D.MovePosition(rigidBody2D.position + currentMoveDirection * speed * Time.deltaTime);
            totalDistaced += speed * Time.deltaTime;
        }
    }

    IEnumerator WaitFuntion(float time)
    {
        // Wait for an amount of time before displaying next dialogue
        yield return new WaitForSeconds(time);
        textObjectScript.virtualActivationFuntion(gameObject.name, playerObject.transform.position);
        gameObject.SetActive(false);
    }
}

