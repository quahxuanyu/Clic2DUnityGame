using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPieceScript : MonoBehaviour
{
    public GameObject KeyPuzzle;
    KeyPuzzleScript KeyPuzzleObject;

    public int currentPuzzleBox;

    bool isPicked = false;
    float speed = 1.5f;
    public bool moving = false;
    public bool fixingDone = false;

    Vector3 targetPos;
    Dictionary<string, Vector3> targetOffsets = new Dictionary<string, Vector3> {
        { "KeyPiece1", new Vector3(0.3f, -0.24f, 0f) },
        { "KeyPiece2", new Vector3(0.25f, -0.4f, 0f) },
        { "KeyPiece3", new Vector3(0.15f, -0.32f, 0f) },
        { "KeyPiece4", new Vector3(0f, -0.27f, 0f) },
        { "KeyPiece5", new Vector3(-0.1f, -0.03f, 0f) }
    };
    Dictionary<string, Vector3> mouseOffsets = new Dictionary<string, Vector3> {
        { "KeyPiece1", new Vector3(0.3f, -0.3f, 0f) },
        { "KeyPiece2", new Vector3(0.2f, -0.2f, 0f) },
        { "KeyPiece3", new Vector3(0f, -0.4f, 0f) },
        { "KeyPiece4", new Vector3(0.2f, -0.2f, 0f) },
        { "KeyPiece5", new Vector3(-0.2f, 0f, 0f) }
    };
    Dictionary<string, Vector3> fixedPos = new Dictionary<string, Vector3> {
        { "KeyPiece1", new Vector3(-2.3f, -0.34f, 0f) },
        { "KeyPiece2", new Vector3(-1.25f, -0.483f, 0f) },
        { "KeyPiece3", new Vector3(0.349f, -0.403f, 0f) },
        { "KeyPiece4", new Vector3(1.846f, -0.36f, 0f) },
        { "KeyPiece5", new Vector3(2.66f, -0.125f, 0f) }
    };

    void Start()
    {
        KeyPuzzleObject = KeyPuzzle.GetComponent<KeyPuzzleScript>();
    }

    void Update()
    {
        if (!moving & !fixingDone & KeyPuzzleObject.fixing)
        {
            targetPos = fixedPos[gameObject.name];
            moving = true;
        }

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (isPicked)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, -0.7f) + mouseOffsets[gameObject.name];
        }

        if (moving & Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            moving = false;
            if (KeyPuzzleObject.fixing)
            {
                fixingDone = true;
            }
        }
    }

    void OnMouseDown()
    {
        isPicked = true;
    }

    void OnMouseUp()
    {
        isPicked = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!moving & !KeyPuzzleObject.fixing & collision.gameObject.name.Substring(0, 9) == "PuzzleBox")
        {
            currentPuzzleBox = int.Parse(collision.gameObject.name.Substring(9, 1));
            if (KeyPuzzleObject.keyPiecesPos[currentPuzzleBox - 1] == "0")
            {
                moving = true;
                KeyPuzzleObject.keyPiecesPos[currentPuzzleBox - 1] = gameObject.name.Substring(8, 1);
                targetPos = collision.gameObject.transform.position + targetOffsets[gameObject.name];
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!KeyPuzzleObject.fixing & collision.gameObject.name.Substring(0, 9) == "PuzzleBox")
        {
            if (gameObject.name.Substring(8, 1) == KeyPuzzleObject.keyPiecesPos[int.Parse(collision.gameObject.name.Substring(9, 1)) - 1])
            {
                KeyPuzzleObject.keyPiecesPos[int.Parse(collision.gameObject.name.Substring(9, 1)) - 1] = "0";
            }
        }
    }
}
