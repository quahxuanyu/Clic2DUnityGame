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
    string currentInstantiatedGO;

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
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!" && currentInstantiatedGO != "Piano")
        {
            currentInstantiatedGO = "Piano";
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Piano", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "Piano";
        }

        //Map
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!" && currentInstantiatedGO != "MonaLisaPainting")
        {
            currentInstantiatedGO = "MonaLisaPainting";
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "MonaLisaPotrait", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "MonaLisaPainting";
        }

        //FixPart
        if (gameControllerObject.currentText == "SHOPKEPPER: Thank you!!!" && currentInstantiatedGO != "LastSupper")
        {
            currentInstantiatedGO = "LastSupper";
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Strawberry", typeof(GameObject)), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1f), Quaternion.identity);
            currentGO.name = "LastSupper";
        }
    }
}
