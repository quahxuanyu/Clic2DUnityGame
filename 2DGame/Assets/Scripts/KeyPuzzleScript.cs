using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPuzzleScript : MonoBehaviour
{
    public GameObject[] keyPieces = new GameObject[5];
    public string[] keyPiecesPos = { "0", "0", "0", "0", "0" };

    GameObject player;
    PlayerController playerObject;
    GameObject canvas;

    public bool fixing = false;
    bool fixingDone = false;

    string currentKeyPiece;
    string currentPuzzleBox;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        player = GameObject.Find("Player");
        playerObject = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        foreach (GameObject keyPiece in keyPieces)
        {
            currentKeyPiece = keyPiece.name.Substring(8, 1);
            //Debug.Log(keyPiece.name + keyPiecesPos[int.Parse(currentKeyPiece) - 1]);
            if (fixing)
            {
                if (!keyPiece.GetComponent<KeyPieceScript>().fixingDone)
                {
                    break;
                }

                if (currentKeyPiece == "5")
                {
                    fixing = false;
                    fixingDone = true;
                    playerObject.addItemToInventory((GameObject)Resources.Load("Prefabs/" + "Key", typeof(GameObject)));
                    StartCoroutine(playerObject.TransitionToScene("CropsPuzzleHouse", playerObject.fadeDuration, playerObject.timeBeforeFadeIn));
                }
            }
            else if (!fixingDone)
            {
                if (keyPiecesPos[int.Parse(currentKeyPiece) - 1] != currentKeyPiece | keyPiece.GetComponent<KeyPieceScript>().moving)
                {
                    break;
                }

                if (currentKeyPiece == "5")
                {
                    fixing = true;
                }
            }
        }
    }
}
