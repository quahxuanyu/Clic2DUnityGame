using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FreeSellerScript : MonoBehaviour
{
    public GameObject gameController;
    GameControllerScript gameControllerObject;

    //Text Variables
    public GameObject textObject;
    private TextScript textObjectScript;

    GameObject currentGO;

    List<string> listInstantiatedGO = new List<string>();

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
        //Compass
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!" && !listInstantiatedGO.Contains("Compass"))
        {
            listInstantiatedGO.Add("Compass");
            textObjectScript.changeTextByKey("FatMerchant3O14", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Compass", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Compass";
        }

        //Map
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!" && !listInstantiatedGO.Contains("Map"))
        {
            listInstantiatedGO.Add("Map");
            textObjectScript.changeTextByKey("FatMerchant3O24", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Map", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Map";
        }

        //CarriageWheel
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!!" && !listInstantiatedGO.Contains("Carriage Wheel"))
        {
            listInstantiatedGO.Add("Carriage Wheel");
            textObjectScript.changeTextByKey("FatMerchant3O34", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Carriage Wheel", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Carriage Wheel";
        }

        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O15", "FatMerchant3O16", "FatMerchant3O17" }, "HAHA!!");
        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O25", "FatMerchant3O26", "FatMerchant3O27" }, "HAHA!!!");
        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O35", "FatMerchant3O36", "FatMerchant3O37" }, "HAHA!!!!");
    }
}
