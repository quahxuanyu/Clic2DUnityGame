using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServentMoveScript : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    //Movement Sequence
    List<Vector2> moveSequence = new List<Vector2>{
        new Vector2(0, -2),
        new Vector2(5, 0)
    };
    //previosPosition is use to calculate the distance traveled
    Vector2 previosPosition;
    int currentMoveSequence = 0;
    //the position it is aiming to get
    Vector2 currentTargetPoint;
    //a rounded float position of this rigidbody
    Vector2 RigidbodyPosition;
    //the moving direction that changes depending on the current moveSequence
    Vector2 currentMoveDirection;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        //initialize for the first movement
        previosPosition = rigidBody2D.position;
        currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveSequence[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveSequence[currentMoveSequence].y) * 10f) / 10f);
        RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);
        Debug.Log(currentTargetPoint);
        Debug.Log(RigidbodyPosition);
        if (RigidbodyPosition.x - currentTargetPoint.x < 0 && currentTargetPoint.x != 0f)
        {
            Debug.Log("ONE");
            currentMoveDirection += new Vector2(1f, 0f);
        }
        else if (RigidbodyPosition.x - currentTargetPoint.x != 0)
        {
            Debug.Log("TWO");
            currentMoveDirection += new Vector2(-1f, 0f);
        }

        if (RigidbodyPosition.y - currentTargetPoint.y < 0 && currentTargetPoint.y != 0f)
        {
            Debug.Log("THREE");
            currentMoveDirection += new Vector2(0f, 1f);
        }
        else if (RigidbodyPosition.y - currentTargetPoint.y != 0)
        {
            Debug.Log("FOUR");
            currentMoveDirection += new Vector2(0f, -1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveSequence[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveSequence[currentMoveSequence].y) * 10f) / 10f);
        RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);

        Debug.Log("Rigid Body X: " + RigidbodyPosition.x);
        Debug.Log("Target X: " + currentTargetPoint.x);
        if (RigidbodyPosition.x == currentTargetPoint.x)
        {
            Debug.Log("IT IS TRUEEEEEEEEEEE!!!!!!!!!!!");
        }
        Debug.Log("Rigid Body Y: " + RigidbodyPosition.y);
        Debug.Log("Target Y: " + currentTargetPoint.y);
        


        if (RigidbodyPosition == currentTargetPoint)
        {
            if (currentMoveSequence != moveSequence.Count-1)
            {
                currentMoveSequence++;
            }
            previosPosition = rigidBody2D.position;
            currentMoveDirection = new Vector2(0f, 0f);
            currentTargetPoint = new Vector2(Mathf.Round((previosPosition.x + moveSequence[currentMoveSequence].x) * 10f) / 10f, Mathf.Round((previosPosition.y + moveSequence[currentMoveSequence].y) * 10f) / 10f);
            RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);
            if (RigidbodyPosition.x - currentTargetPoint.x < 0 && currentTargetPoint.x != 0f)
            {
                Debug.Log("ONE");
                currentMoveDirection += new Vector2(1f, 0f);
            }
            else if (RigidbodyPosition.x - currentTargetPoint.x != 0)
            {
                Debug.Log("TWO");
                currentMoveDirection += new Vector2(-1f, 0f);
            }

            if (RigidbodyPosition.y - currentTargetPoint.y < 0 && currentTargetPoint.y != 0f)
            {
                Debug.Log("THREE");
                currentMoveDirection += new Vector2(0f, 1f);
            }
            else if (RigidbodyPosition.y - currentTargetPoint.y != 0)
            {
                Debug.Log("FOUR");
                currentMoveDirection += new Vector2(0f, -1f);
            }
            Debug.Log("New!!");
            Debug.Log(currentMoveDirection);
        }
        else
        {
            Debug.Log("MOVING");
            Debug.Log(currentMoveDirection);
            rigidBody2D.MovePosition(rigidBody2D.position + currentMoveDirection * 2f * Time.deltaTime);
        }
    }
}
