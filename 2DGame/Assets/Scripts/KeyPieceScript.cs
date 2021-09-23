using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPieceScript : MonoBehaviour
{
    public GameObject keyPuzzle;
    KeyPuzzleScript keyPuzzleObject;

    int keyNumber;
    int currentPuzzleBox;


    bool isPicked = false;
    bool moving = false;

    Vector3 targetPos;

    void Start()
    {
        keyPuzzleObject = keyPuzzle.GetComponent<KeyPuzzleScript>();
        keyNumber = int.Parse(gameObject.name.Substring(8, 1));
    }

    void Update()
    {
        // If the piece is moving into position, just move, then exit
        if (moving)
        {
            transform.position =
                Vector3.MoveTowards(transform.position,
                keyPuzzleObject.getPuzzleBoxKeyPosition(currentPuzzleBox),
                keyPuzzleObject.getSpeed() * Time.deltaTime);
            // Ok, piece is in position, stop moving
            if (Vector3.Distance(transform.position,
                keyPuzzleObject.getPuzzleBoxKeyPosition(currentPuzzleBox)) < 0.001f)
            {
                moving = false;
            }
            return;
        }
        // If the puzzle is solved and we're just doing the final animation, just return
        if (keyPuzzleObject.getFixing())
        {
            return;
        }
        if (isPicked)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, -0.7f);

        }
    }

    void OnMouseDown()
    {
        isPicked = true;
        moving = false;
    }

    void OnMouseUp()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        isPicked = false;
        currentPuzzleBox = keyPuzzleObject.getPuzzleBox(mousePos);
        // If we're in a puzzle box, start moving the piece into position
        if (currentPuzzleBox > 0)
        {
            moving = true;
        }
    }

    public bool isCorrect()
    {
        return !isPicked && !moving && (keyNumber == currentPuzzleBox);
    }
}
