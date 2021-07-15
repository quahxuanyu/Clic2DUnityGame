using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellerScirpt : MonoBehaviour
{
    public GameObject gameController;
    GameControllerScript gameControllerObject;

    //Text Variables
    public GameObject textObject;
    private TextScript textObjectScript;

    GameObject currentGO;

    List<string> instantiatedGO = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController");
        gameControllerObject = gameController.GetComponent<GameControllerScript>();
        textObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        textObjectScript = textObject.GetComponent<TextScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "MarketStandRed" && gameControllerObject.currentText == "SELLER: Here ya go!" && instantiatedGO.Contains("Sack of Boar Food") == false)
        {
            instantiatedGO.Add("Sack of Boar Food");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Sack of Boar Food", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Sack of Boar Food";
            gameObject.name = "MarketStandRedDone";
        }

        if (gameObject.name == "CabinetWithLetterDone" && gameControllerObject.currentText.Contains("*Sigh.*") && instantiatedGO.Contains("Letter") == false)
        {
            instantiatedGO.Add("Letter");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Letter", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 0.7f), Quaternion.identity);
            currentGO.name = "Letter";
        }
    }
}
