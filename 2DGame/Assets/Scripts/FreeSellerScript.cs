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
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!" && !listInstantiatedGO.Contains("Piano"))
        {
            listInstantiatedGO.Add("Piano");
            textObjectScript.changeTextByKey("FatMerchant3O14", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Piano", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Piano";
        }

        //Map
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!" && !listInstantiatedGO.Contains("MonaLisaPainting"))
        {
            listInstantiatedGO.Add("MonaLisaPainting");
            textObjectScript.changeTextByKey("FatMerchant3O24", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "MonaLisaPotrait", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "MonaLisaPainting";
        }

        //FixPart
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!!" && !listInstantiatedGO.Contains("LastSupper"))
        {
            listInstantiatedGO.Add("LastSupper");
            textObjectScript.changeTextByKey("FatMerchant3O34", "SHOPKEPPER: Hey you greedy punk! \n I gave you everything, if you want more you gotta pay!");
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Strawberry", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "LastSupper";
        }

        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O15", "FatMerchant3O16", "FatMerchant3O17" }, "HAHA!!");
        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O25", "FatMerchant3O26", "FatMerchant3O27" }, "HAHA!!!");
        textObjectScript.removeTextsByKeyOnDialogue(new string[] { "FatMerchant3O35", "FatMerchant3O36", "FatMerchant3O37" }, "HAHA!!!!");
    }
}
