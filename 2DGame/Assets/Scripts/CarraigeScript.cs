using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarraigeScript : MonoBehaviour
{
    //Text Variables
    public GameObject textObject;

    GameObject currentGO;

    string currentText;

    List<string> listInstantiatedGO = new List<string>();
    string currentInstantiatedGO;

    // Start is called before the first frame update
    void Start()
    {
        textObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        currentText = textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;        
        //Compass
        if (currentText == "This torch!" && currentInstantiatedGO != "Torch")
        {
            Debug.Log(currentText);
            currentInstantiatedGO = "Torch";
            currentGO = Instantiate((GameObject)Resources.Load("Prefabs/" + "Torch", typeof(GameObject)), new Vector2(gameObject.transform.position.x - 1f, gameObject.transform.position.y + 1f), Quaternion.identity);
            currentGO.name = "Torch";
        }
    }
}
