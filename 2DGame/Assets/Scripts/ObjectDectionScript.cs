using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDectionScript : MonoBehaviour
{
    GameObject BoarFood;
    GameObject playerObject;
    GameObject textObject;
    TextScript textObjectScript;

    //Offset the starting position of checking
    Vector3 offset = new Vector3(-0.561f, -2.676f, 0);

    //the area of Detection from the position
    Vector2 areaOfDetection = new Vector2(2f, 1.5f);

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");
        textObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        textObjectScript = textObject.GetComponent<TextScript>();
    }

    // Update is called once per frame
    void Update()
    {        
        BoarFood = GameObject.Find("PacketOfPigFood");

        //check if boardfood is in the hierarchy
        if (BoarFood != null)
        {
            //check if boarfood is in the range of the detection
            if (whithinRange(BoarFood, areaOfDetection, gameObject.transform.position - offset))
            {
                Debug.Log("Object Detect");
                textObjectScript.virtualActivationFuntion("Pigsties", playerObject.transform.position);
                BoarFood.name = "PacketOfPigFoodChange";
                BoarFood = null;
            }
        }
        
    }

    //whithingRange(Object to check, the area of detection, the middle point of the area of detection)
    bool whithinRange(GameObject obj, Vector2 area, Vector2 position)
    {
        if (obj.transform.position.x < (position.x + area.x) && obj.transform.position.x > (position.x - area.x) && obj.transform.position.y < (position.y + area.y) && obj.transform.position.y > (position.y - area.y))
        {
            return true;
        }
        return false;
    }
}
