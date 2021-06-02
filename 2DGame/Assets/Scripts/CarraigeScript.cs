using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarraigeScript : MonoBehaviour
{
    public GameObject gameController;
    GameControllerScript gameControllerObject;

    //Text Variables
    public GameObject textObject;

    GameObject currentGO;

    List<string> listInstantiatedGO = new List<string>();
    string currentInstantiatedGO;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController");
        gameControllerObject = gameController.GetComponent<GameControllerScript>();
        textObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Compass
        if (gameControllerObject.currentText == "This torch!" && currentInstantiatedGO != "Torch")
        {
            Debug.Log(gameControllerObject.currentText);
            currentInstantiatedGO = "Torch";
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Torch", typeof(GameObject)), new Vector2(gameObject.transform.position.x - 1f, gameObject.transform.position.y + 1f), Quaternion.identity);
            currentGO.name = "Torch";
        }
    }
}
