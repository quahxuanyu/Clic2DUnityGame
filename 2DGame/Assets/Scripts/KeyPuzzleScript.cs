using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPuzzleScript : MonoBehaviour
{
    public GameObject[] keyPieces = new GameObject[5]; // Set in unity
    public GameObject[] puzzleBoxes = new GameObject[5]; // Set in unity

    GameObject player;
    PlayerController playerObject;
    GameObject canvas;

    Bounds[] puzzleBoxBounds = new Bounds[5];
    Vector3[] puzzleBoxKeyPositions = new Vector3[5];

    protected static Vector3[] fixedPos = {
        new Vector3(-2.213f, -0.117f, 0f),
        new Vector3(-1.066f, -0.289f, 0f),
        new Vector3(0.605f, -0.061f, 0f),
        new Vector3(2.016f, -0.09f, 0f),
        new Vector3(3.241f, -0.163f, 0f)
    };

    bool fixing = false;
    float speed = 1.5f;

    string currentKeyPiece;
    string currentPuzzleBox;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        player = GameObject.Find("Player");
        playerObject = player.GetComponent<PlayerController>();

        for (int i = 0; i < 5; i++)
        {
            puzzleBoxBounds[i] = puzzleBoxes[i].GetComponent<BoxCollider2D>().bounds;
            puzzleBoxKeyPositions[i] = new Vector3(
                puzzleBoxBounds[i].center.x, puzzleBoxBounds[i].center.y, -0.7f);
        }
    }

    void Update()
    {
        if (!fixing)
        {
            bool solved = true;
            foreach (GameObject keyPiece in keyPieces)
            {
                // Keys have to be NOT being moved around AND in the right place
                if (!keyPiece.GetComponent<KeyPieceScript>().isCorrect())
                {
                    solved = false;
                    break;
                }
            }
            if (solved)
            {
                fixing = true;
            }
        }
        else
        {
            bool fixingDone = true;
            // Move each key piece into positio 
            for (int i = 0; i < 5; i++)
            {
                if (!(Vector3.Distance(keyPieces[i].transform.position, fixedPos[i]) < 0.001f))
                {
                    fixingDone = false;
                    keyPieces[i].transform.position =
                        Vector3.MoveTowards(keyPieces[i].transform.position, fixedPos[i], speed * Time.deltaTime);
                }
            }
            // If all keys pieces have been moved into a key, the puzzle is done
            if (fixingDone)
            {
                playerObject.addItemToInventory((GameObject)Resources.Load("Prefabs/" + "Key", typeof(GameObject)));
                StartCoroutine(playerObject.TransitionToScene("CropsPuzzleHouse",
                    playerObject.fadeDuration, playerObject.timeBeforeFadeIn));
                fixing = false;
            }
        }
    }

    public int getPuzzleBox(Vector2 pos)
    {
        for (int i = 0; i < 5; i++)
        {
            if (puzzleBoxBounds[i].Contains(
                new Vector3(pos.x, pos.y, puzzleBoxBounds[i].center.z)))
            {
                return i + 1;
            }
        }
        return 0;
    }

    public bool getFixing()
    {
        return fixing;
    }

    public Vector3 getPuzzleBoxKeyPosition(int i)
    {
        return puzzleBoxKeyPositions[i - 1];
    }

    public float getSpeed() {
        return speed;
    }
}

