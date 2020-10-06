using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ServantMoveScript : MonoBehaviour
{
    Rigidbody2D rigidBody2D;

    //Text Variebles
    public GameObject textBox;
    private TextScript textObject;

    //Movement Sequence
    List<Vector2> servantCallMovement = new List<Vector2>{
        new Vector2(0f, -5f),
        new Vector2(8f, 0f),
        new Vector2(0f, -0.8f)
    };
    List<Vector2> servantToDoorMovement = new List<Vector2>{
        new Vector2(0f, 0.8f),
        new Vector2(-8f, 0f),
        new Vector2(0f, 5f)
    };

    //previosPosition is use to calculate the distance traveled
    Vector2 previosPosition;
    int currentMoveSequence = 0;

    //Moveing speed variable
    float speed = 3f;
    //Moving variable, to check if a movement is true
    bool moving = false;
    //the position it is aiming to get
    Vector2 currentTargetPoint;
    //a rounded float position of this rigidbody
    Vector2 RigidbodyPosition;
    //the moving direction that changes depending on the current moveSequence
    Vector2 currentMoveDirection;

    //Variables for checking if a list of movement has been made
    List<List<Vector2>> currentArray =  new List<List<Vector2>>();

    //Variable for checking if a movement is finished
    List<string> activatedMovements = new List<string>();

    //Current Text Displayed
    string currentText;
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        textObject = GetComponent<TextScript>();
        //initialize for the first movement
        previosPosition = rigidBody2D.position;
    }

    // Update is called once per frame
    void Update()
    {
        //update for the current text
        currentText = textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        
        //TODO
        if (currentText == "Servant!" || moving && !activatedMovements.Contains("Servant!"))
        {
            NPCMovement(servantCallMovement, false);
        }
        else if (currentText == "KING: What is going on? I wonder..." || moving && !activatedMovements.Contains("KING: What is going on? I wonder..."))
        {
            NPCMovement(servantToDoorMovement, true);
        }
    }

    //Moving function (movement list, hide the object at the end of the movement)
    public void NPCMovement(List<Vector2> moveArray, bool hide)
    {
        //currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveArray[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveArray[currentMoveSequence].y) * 10f) / 10f);
        RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);

        //Initailize first movement
        if (!currentArray.Contains(moveArray))
        {
            currentMoveSequence = 0;
            currentArray.Add(moveArray);
            previosPosition = rigidBody2D.position;
            currentMoveDirection = new Vector2(0, 0);
            currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveArray[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveArray[currentMoveSequence].y) * 10f) / 10f);
            moving = true;
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
                activatedMovements.Add(currentText);
                moving = false;
                if (hide)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        //Moving!
        else
        {
            Debug.Log("MOVINGG  " + currentTargetPoint + RigidbodyPosition + currentMoveDirection);
            rigidBody2D.constraints = RigidbodyConstraints2D.None;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidBody2D.MovePosition(rigidBody2D.position + currentMoveDirection * speed * Time.deltaTime);
        }
    }
}

