using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public GameObject InventoryTextObject;
    TextMeshProUGUI InventoryText;
    bool displayTextState = false;
    float alpha = 1f;
    float targetAlpha = 1f;

    public GameObject player;
    PlayerController playerObject;
    GameObject currentGO;
    Text currentGOText;

    private int numOfSpace = 8;
    public List<string> availableSpaces = new List<string>();
    public GameObject ImagePrefab;
    Image imageComponent;

    int currentSelectorIndex = 0;

    //Image offset
    int x = 54;
    int y = -45;
    //Selector offset
    int selectorX = 50;
    int selectorY = -50;

    GameObject SelectorObject;
    // Start is called before the first frame update

    // Keycodes for inventory input
    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    void Start()
    {
        InventoryText = InventoryTextObject.GetComponent<TextMeshProUGUI>();
        SelectorObject = gameObject.transform.GetChild(0).gameObject;
        playerObject = player.GetComponent<PlayerController>();
        //fill list of string for storing which collum of inventory is open
        for (int i = 0; i < numOfSpace; i++)
        {
            availableSpaces.Add("empty");
        }
    }

    void Update()
    {
        float scrolWheel = Input.mouseScrollDelta.y;
        if (InventoryText.color.a > targetAlpha)
        {
            Color alphaChanged = InventoryText.color;
            alpha = InventoryText.color.a - 3f * Time.deltaTime;
            alphaChanged.a = alpha;
            InventoryText.color = alphaChanged;
            //Debug.Log("MINUSInG: " + InventoryText.color.a + " Target: " + targetAlpha);
        }
        else if (InventoryText.color.a < 0f)
        {
            InventoryTextObject.SetActive(false);
        }

        combineBucket(playerObject.inventoryAmount);

        //Debug.Log(i);
        //check if any of the number keys are pressed
        if (scrolWheel != 0)
        {
            if (scrolWheel > 0)
            {
                currentSelectorIndex -= 1;
            }
            else
            {
                currentSelectorIndex += 1;
            }
            currentSelectorIndex = currentSelectorIndex % 8;
            if (currentSelectorIndex < 0)
                currentSelectorIndex = 7;
            //Check if the collum selected is empty or not
            //if it's fill, the item selected will be that
            if (availableSpaces[currentSelectorIndex] != "empty")
            {
                playerObject.currentSelectedItem = availableSpaces[currentSelectorIndex];
                //Move selector image
                SelectorObject.transform.localPosition = new Vector2(selectorX, selectorY - (currentSelectorIndex * 100));
                //Display Item Name
                displayTextState = true;
            }
            //if it's empty, the item selected remain empty
            else
            {
                playerObject.currentSelectedItem = "";
                SelectorObject.transform.localPosition = new Vector2(selectorX, selectorY - (currentSelectorIndex * 100));
            }

            //display item name
            if (displayTextState)
            {
                StartCoroutine(FadeInventoryText(availableSpaces[currentSelectorIndex]));
            }
        }

        for (int i = 0; i < numOfSpace; i++)
        {
            //Debug.Log(i);
            //check if any of the number keys are pressed
            if (Input.GetKeyDown(keyCodes[i]))
            {
                //Check if the collum selected is empty or not
                //if it's fill, the item selected will be that
                Debug.Log(availableSpaces[i]);
                Debug.Log(i);
                if (availableSpaces[i] != "empty")
                {
                    Debug.Log("KEycode Inventory: " + i);
                    playerObject.currentSelectedItem = availableSpaces[i];
                    //Get selector image
                    GameObject SelectorObject = gameObject.transform.GetChild(0).gameObject;
                    //Move selector image
                    SelectorObject.transform.localPosition = new Vector2(selectorX, selectorY - (i * 100));
                    //Display Item Name
                    displayTextState = true;
                }
                //if it's empty, the item selected remain empty
                else
                {
                    Debug.Log("KEycode Inventory: " + i);
                    playerObject.currentSelectedItem = "";
                    SelectorObject.transform.localPosition = new Vector2(selectorX, selectorY - (i * 100));
                }

                //display item name
                if (displayTextState)
                {
                    StartCoroutine(FadeInventoryText(availableSpaces[i]));
                }
            }
        }
    }

    //If function called, update inventory display
    public void InventoryUpdate()
    {
        //go through the dictionary of pickable objects
        foreach (KeyValuePair<string, GameObject> elements in playerObject.pickableGameObjects)
        {
            //check if the current object exist already as a child
            //if yes, it will be a positive number
            //if no, it will be -1
            int haveChild = ContainsChild(elements.Key);

            //if current object exist as child
            if (haveChild != -1)
            {
                //if number equals zero, delete object on display and set it's current space to "empty"
                if (playerObject.inventoryAmount[elements.Key] == 0)
                {
                    availableSpaces[haveChild - 1] = "empty";
                    if (playerObject.currentSelectedItem == elements.Key)
                        playerObject.currentSelectedItem = "";
                    Destroy(gameObject.transform.GetChild(haveChild).gameObject);
                    continue;
                }
                //get number next to the object and update it
                currentGOText = gameObject.transform.GetChild(haveChild).GetChild(0).GetComponent<Text>();
                currentGOText.text = Convert.ToString(playerObject.inventoryAmount[elements.Key]);
                fadeInventoryText(elements.Key);
            }
            //else if object does not exist as child
            else
            {
                if (playerObject.inventoryAmount[elements.Key] == 0)
                {
                    continue;
                }
                fadeInventoryText(elements.Key);
                //go through all collums
                for (int b = 0; b < numOfSpace; b++)
                {
                    //check for the first one that is empty
                    if (availableSpaces[b] == "empty")
                    {
                        //create current game object
                        currentGO = Instantiate(ImagePrefab, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
                        currentGO.name = elements.Value.name;
                        playerObject.currentSelectedItem = elements.Value.name;
                        currentGO.transform.localPosition = new Vector2(x, y + -(b * 100));
                        imageComponent = currentGO.GetComponent<Image>();
                        imageComponent.sprite = elements.Value.GetComponent<SpriteRenderer>().sprite;
                        availableSpaces[b] = currentGO.name;
                        break;
                    }
                } 
            }
        }
        
        /*foreach (var space in availableSpaces)
        {
            if (space == "empty" && playerObject.pickableGameObjects.ContainsKey(space))
            {
                playerObject.pickableGameObjects.Remove(space);
            }
        }*/
    }

    void fadeInventoryText(String text)
    {
        if (GameObject.Find("InventoryBar") == null)
        {
            return;
        }
        StartCoroutine(FadeInventoryText(text));
    }

    void combineBucket(Dictionary<string, int> inventoryAmount)
    {
        int checkForBucketPieces = 0; 
        foreach (KeyValuePair<string, int> elements in inventoryAmount)
        {
            if (elements.Key.Contains("Bucket ") || elements.Key  == "Chewed Bubblegum")
            {
                if (elements.Value > 0)
                {
                    checkForBucketPieces += 1;
                }
            }
        }
        if (checkForBucketPieces == 5)
        {
            playerObject.inventoryAmount["Bucket Rim"] = 0;
            playerObject.inventoryAmount["Bucket Wooden Parts"] = 0;
            playerObject.inventoryAmount["Bucket Plug"] = 0;
            playerObject.inventoryAmount["Bucket Handle"] = 0;
            playerObject.inventoryAmount["Chewed Bubblegum"] = 0;
            //InventoryUpdate();
            playerObject.addItemToInventory((GameObject)Resources.Load("Prefabs/" + "Empty Bucket", typeof(GameObject)));
        }
    }

    IEnumerator FadeInventoryText(string name)
    {
        Debug.Log("FadeInventoryText" + GameObject.Find("InventoryBar"));

        targetAlpha = 1f;
        InventoryTextObject.SetActive(true);
        displayTextState = false;
        InventoryText.text = name;
        Color alphaChanged = InventoryText.color;
        alphaChanged.a = 1f;
        InventoryText.color = alphaChanged;
        yield return new WaitForSeconds(2);
        targetAlpha = 0f;
    }

    //function to check if object exist as child
    public int ContainsChild(string name)
    {
        int answer = -1;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            //Debug.Log("name: " + name);
            //Debug.Log("child: " + gameObject.transform.GetChild(i).name);
            if (gameObject.transform.GetChild(i).name == name)
            {
                answer = i;
                break;
            }
        }
        return answer;
    }
}
