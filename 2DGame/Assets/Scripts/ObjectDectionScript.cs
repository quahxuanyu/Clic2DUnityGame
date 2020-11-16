using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectDectionScript : MonoBehaviour
{
    GameObject BoarFood;
    GameObject FixPart;
    GameObject Letter;

    GameObject playerObject;
    GameObject textObject;
    TextScript textObjectScript;

    GameObject fadeObject;
    FadingScript fadeObjectScript;
    //the area of Detection from the position
    Vector2 areaOfDetection = new Vector2(2f, 1.5f);

    // Start is called before the first frame update
    void Start()
    {
        fadeObject = GameObject.Find("FadingScreen");
        fadeObjectScript = fadeObject.GetComponent<FadingScript>();

        playerObject = GameObject.Find("Player");
        textObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        textObjectScript = textObject.GetComponent<TextScript>();
    }

    // Update is called once per frame
    void Update()
    {
        BoarFood = GameObject.Find("PacketOfPigFood");
        FixPart = GameObject.Find("Strawberry");
        Letter = GameObject.Find("Letter");

        //check if boardfood is in the hierarchy
        if (BoarFood != null)
        {
            //check if boarfood is in the range of the detection
            if (whithinRange(BoarFood, areaOfDetection, new Vector2(-19.69f, -2.65f)))
            {
                Debug.Log("Object Detect");
                textObjectScript.virtualActivationFuntion("Pigsties", playerObject.transform.position);
                BoarFood.name = "PacketOfPigFoodChange";
                BoarFood = null;
            }
        }

        if (FixPart != null)
        {
            //check if boarfood is in the range of the detection
            if (whithinRange(FixPart, areaOfDetection, new Vector2(6.18f, 0.2f)))
            {
                Debug.Log("Object Detect");
                textObjectScript.virtualActivationFuntion("carraigeFix", playerObject.transform.position);
                FixPart.name = "FixPartChange";
                FixPart.active = false;
                FixPart = null;
            }
        }

        if (Letter != null)
        {
            //check if boarfood is in the range of the detection
            if (whithinRange(Letter, areaOfDetection, new Vector2(-15.723f, -0.666f)))
            {
                Debug.Log("Object Detect");
                textObjectScript.virtualActivationFuntion("mailBoxLetterText", playerObject.transform.position);
                Letter.name = "FixPartChange";
                Letter.active = false;
                Letter = null;
            }
        }



        if (textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "GUY: I gotta go now, wish you luck on your journy to rescue the princess")
        {
            textObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "GUY: I gotta go now, wish you luck on your journy to rescue the princess ";
            StartCoroutine(TransitionToScene(4f, 2f));
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

    IEnumerator TransitionToScene(float duration, float timeBefore)
    {
        //Make sure movement variables aren't updated
        //Start fading in
        playerObject.GetComponent<PlayerController>().inTransition = true;
        fadeObjectScript.BeginFade(1, duration);
        //Don't transition to new scene until fully faded in and waited for an amount of time
        yield return new WaitForSeconds(duration + timeBefore);
        GameObject.Find("carriage").SetActive(false);
        textObjectScript.optionTree = "";
        textObjectScript.hasNextOption = false;
        textObjectScript.hasNextPage = false;
        textObjectScript.virtualActivation = false;
        textObjectScript.notOption = true;
        textObjectScript.currentPage = 0;
        textObjectScript.DisplayDialog(false);
        playerObject.GetComponent<PlayerController>().inTransition = false;
        fadeObjectScript.BeginFade(-1, 1);
    }
}
