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
        { "KeyPiece1", new Vector3(0.26f, -0.26f, 0f) },
        { "KeyPiece2", new Vector3(0.22f, -0.42f, 0f) },
        { "KeyPiece3", new Vector3(0.13f, -0.36f, 0f) },
        { "KeyPiece4", new Vector3(-0.03f, -0.3f, 0f) },
        { "KeyPiece5", new Vector3(-0.3f, -0.04f, 0f) }
    };
    Dictionary<string, Vector3> mouseOffsets = new Dictionary<string, Vector3> {
        { "KeyPiece1", new Vector3(0.3f, -0.3f, 0f) },
        { "KeyPiece2", new Vector3(0.2f, -0.2f, 0f) },
        { "KeyPiece3", new Vector3(0f, -0.4f, 0f) },
        { "KeyPiece4", new Vector3(0.2f, -0.2f, 0f) },
        { "KeyPiece5", new Vector3(-0.2f, 0f, 0f) }
    };
    Dictionary<string, Vector3> fixedPos = new Dictionary<string, Vector3> {
        { "KeyPiece1", new Vector3(-2.213f, -0.117f, 0f) },
        { "KeyPiece2", new Vector3(-1.066f, -0.289f, 0f) },
        { "KeyPiece3", new Vector3(0.605f, -0.061f, 0f) },
        { "KeyPiece4", new Vector3(2.016f, -0.09f, 0f) },
        { "KeyPiece5", new Vector3(3.241f, -0.163f, 0f) }
    };
    Dictionary<string, float[]> edgeOffset = new Dictionary<string, float[]> {
        { "KeyPiece1", new float[] { -6f, 5.8f , -2.3f, 4.3f } },
        { "KeyPiece2", new float[] { -5.5f, 5.48f, -2.1f, 4.1f } },
        { "KeyPiece3", new float[] { -5.55f, 5.6f, -2.4f, 4.3f } },
        { "KeyPiece4", new float[] { -5.57f, 5.4f, -1.7f, 3.7f } },
        { "KeyPiece5", new float[] { -5.8f, 6.4f, -2f, 3.84f } },
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
            if (mousePos.x > edgeOffset[gameObject.name][0] && mousePos.x < edgeOffset[gameObject.name][1] && 
                mousePos.y > edgeOffset[gameObject.name][2] && mousePos.y < edgeOffset[gameObject.name][3])
            {
                transform.position = new Vector3(mousePos.x, mousePos.y, -0.7f);
            }
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
                Debug.Log(gameObject.name + " On Collision");
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
                Debug.Log(gameObject.name + " Exit Collsion");
                KeyPuzzleObject.keyPiecesPos[int.Parse(collision.gameObject.name.Substring(9, 1)) - 1] = "0";
            }
        }
    }
}
