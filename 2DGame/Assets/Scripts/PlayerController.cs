using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerController : MonoBehaviour
{
    //Movement Variables
    Rigidbody2D rigidBody2D;
    float horizontal;
    float vertical;
    public Vector2 lookDirection;
    public float speed = 2f;

    //Inventory Variables
    public GameObject Inventory;
    DisplayInventory InventoryScript;
    public Dictionary<string, int> inventoryAmount = new Dictionary<string, int>();
    public Dictionary<string, GameObject> pickableGameObjects = new Dictionary<string, GameObject>();
    public GameObject currentPickableItem;
    GameObject currentDroppedItem;
    public string currentSelectedItem;

    //Text Variables
    public bool textState = false;
    public GameObject textBox;
    public float raycastDistance = 50f;
    //Walk away text limit
    private float raycastLimitDistance = 2f;
    TextScript textObject;
    string currentText;

    //MiniMap
    public GameObject MiniMapObject;
    MiniMapScript miniMapObjectScript;

    //Compass
    public GameObject CompassObject;
    CompassScript compassObjectScript;

    Animator animator;

    //Scene Transition Variable
    string nextScene;
    public bool inTransition = false;
    public bool lockedMovement;

    //Fading variables
    public GameObject fadeScreen;
    public float fadeDuration = 1f;
    public float timeBeforeFadeIn = 0.5f;
    FadingScript fadeScriptObject;

    // Start is called before the first frame update
    void Start()
    {
        //NOTE: IF ERRORS LIKE "OBJECT REFERENCE NOT SET TO AN INSTANCE OF AN OBJECT"
        //WHEN YOU CHANGE TO A DIFFERENT SCENE. IT IS DUE TO THE FACT THAT IF ONE OF
        //THESE OBJECTS WERE NOT ATTACHED, IT GIVES OUT AN ERROR, AND
        //EVERYTHING ELSE AFTER THAT LINE DOSEN'T RUN
        currentPickableItem = null;
        rigidBody2D = GetComponent<Rigidbody2D>();
        InventoryScript = Inventory.GetComponent<DisplayInventory>();
        animator = GetComponent<Animator>();
        textObject = textBox.GetComponent<TextScript>();
        fadeScriptObject = fadeScreen.GetComponent<FadingScript>();
        miniMapObjectScript = MiniMapObject.GetComponent<MiniMapScript>();
        compassObjectScript = CompassObject.GetComponent<CompassScript>();
        lockedMovement = true;
        lookDirection = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        currentText = textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        //MOVEMENT
        //Debug.Log(horizontal.ToString());
        //Debug.Log(Input.GetAxis("Horizontal").ToString());
        if (horizontal > 0 && Input.GetAxis("Horizontal") >= horizontal)
        {
            horizontal = 1;
        }
        else if (horizontal < 0 && Input.GetAxis("Horizontal") <= horizontal)
        {
            horizontal = -1;
        }
        else
        {
            horizontal = 0;
        }

        if (vertical > 0 && Input.GetAxis("Vertical") >= vertical)
        {
            vertical = 1;
        }
        else if (vertical < 0 && Input.GetAxis("Vertical") <= vertical)
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }

        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);

        //Don't update movement variables during scene transition
        if (!inTransition & !lockedMovement)
        {
            Vector2 position = rigidBody2D.position;
            position += move * speed * Time.deltaTime;
            rigidBody2D.MovePosition(position);
            animator.SetFloat("Speed", move.magnitude);
        }
        else
        {
            Debug.Log("Not Moving  " + "In transition: " + inTransition + lockedMovement)
;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //RAYCAST
        //Check If Raycast hit anything when "X" is pressed, if yes turn on the dialog OR if mouse left click is pressed, change the dialogue page
        if (Input.GetKeyDown(KeyCode.X) && textBox.activeSelf == false || Input.GetKeyDown(KeyCode.Mouse0) && textBox.activeSelf == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidBody2D.position + Vector2.up * 0.2f, lookDirection, raycastDistance, LayerMask.GetMask("NonPlayerCharacter"));
            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.gameObject.name);
                //Debug.Log(textObject.notOption);
                if (hit.collider.gameObject.tag == "TextInteract" && textObject.notOption)
                {
                    // REVIEW LATER "if (textObject.hasNextPage == false)" AND "else if (!textObject.hasNextPage)"
                    if (textObject.hasNextPage == false)
                    {
                        Debug.Log("First");
                        textState = true;
                        textObject.interactablePos = gameObject.transform.position;
                        textObject.currentTextObjectName = hit.collider.name;
                    }
                    else if (textObject.virtualActivation == true)
                    {
                        Debug.Log("Second");
                        textState = true;
                    }
                    else if (!textObject.hasNextPage)
                    {
                        Debug.Log("Third");
                        textState = false;
                    }
                    textObject.DisplayDialog(textState);
                }
            }

            else if (textBox.activeSelf == true && Input.GetKeyDown(KeyCode.Mouse0) && textObject.virtualActivation == true && textObject.notOption)
            {
                //Debug.Log("Yep, Here is the problem");
                if (textObject.hasNextPage)
                {
                    textState = true;
                }
                if (textObject.hasNextPage == false)
                {
                    textState = true;
                    textObject.interactablePos = gameObject.transform.position;
                }
                else if (!textObject.hasNextPage)
                {
                    textState = false;
                }
                textObject.interactablePos = gameObject.transform.position;
                //Debug.Log("possibility problem 1: " + textState);
                Debug.Log("Hmmm: " + textState);
                textObject.DisplayDialog(textState);
            }
        }

        //Increament Item amount
        //if colidded with a new object
        if (currentPickableItem != null)
        {
            //check if dictionary contains this objectS
            if (inventoryAmount.ContainsKey(currentPickableItem.name))
            {
                //increment by 1, and update the inventory display
                inventoryAmount[currentPickableItem.name] += 1;
                InventoryScript.InventoryUpdate();
            }
            else
            {
                //else, add a new one
                inventoryAmount.Add(currentPickableItem.name, 1);
                InventoryScript.InventoryUpdate();
            }
            //if currently no item is selected
            if (currentSelectedItem == "")
            {
                //the current object will be selected
                currentSelectedItem = currentPickableItem.name;
                InventoryScript.InventoryUpdate();
            }
            //destroy the obejct
            Destroy(currentPickableItem);
            currentPickableItem = null;
        }

        //Drop Item
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //if an item is selected and its amount is not equals to zero
            if (currentSelectedItem != "" && inventoryAmount[currentSelectedItem] > 0)
            {
                //create it, decrese the amount by 1 and update the inventory display
                currentDroppedItem = Instantiate(pickableGameObjects[currentSelectedItem], rigidBody2D.position + lookDirection * 1.1f, Quaternion.identity);
                currentDroppedItem.name = pickableGameObjects[currentSelectedItem].name;
                inventoryAmount[currentDroppedItem.name] -= 1;
                Debug.Log(currentSelectedItem);
                Debug.Log(inventoryAmount[currentSelectedItem]);
                InventoryScript.InventoryUpdate();
                Debug.Log(currentSelectedItem);
                Debug.Log(inventoryAmount[currentSelectedItem]);
            }

            if (currentSelectedItem != "" && inventoryAmount[currentSelectedItem] == 0)
            {

                //Debug.Log(currentSelectedItem);
                //Debug.Log(inventoryAmount[currentSelectedItem]);
                currentSelectedItem = "";
                //InventoryScript.InventoryUpdate();
                //Debug.Log(currentSelectedItem);
                //Debug.Log("IT's NOTHING");
            }
        }

        //change Scene short cut
        if (Input.GetKeyDown(KeyCode.R))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Farm", fadeDuration, timeBeforeFadeIn));
        }

        //Item UI pop-up
        //MINI MAP
        if (currentSelectedItem == "Strawberry")
        {
            miniMapObjectScript.MiniMap(true);
        }
        else
        {
            miniMapObjectScript.MiniMap(false);
        }

        //Compass
        if (currentSelectedItem == "Piano")
        {
            compassObjectScript.Compass(true);
        }
        else
        {
            compassObjectScript.Compass(false);
        }



        //Check distance between Player and Object, if it's more than "raycastLimitDistance"  ALL dialog turn off
        if (textState == true && Vector2.Distance(textObject.interactablePos, rigidBody2D.position) > raycastLimitDistance)
        {
            //Debug.Log("WHAT????: " + Vector2.Distance(textObject.interactablePos, rigidBody2D.position) + " > " + raycastLimitDistance);
            textState = !textState;
            if (textObject.notOption == false)
            {
                for (int i = 1; i <= textObject.optionObject.numOfButtons; i++)
                {
                    Destroy(textObject.optionObject.gameObject.transform.GetChild(i).gameObject);
                }
            }
            textObject.optionTree = "";
            textObject.hasNextOption = false;
            textObject.hasNextPage = false;
            textObject.virtualActivation = false;
            textObject.notOption = true;
            textObject.currentPage = 0;
            //Debug.Log("possibility problem 2: " + textState);
            //Debug.Log("Distance over limit: " + Vector2.Distance(textObject.interactablePos, rigidBody2D.position));
            Debug.Log("Hmmm: " + textState);
            textObject.DisplayDialog(textState);
        }

        //Lock movement during certain dialogues
        lockMovementFunction(textToUnlock: "KING:What is going on? I wonder...\n \n (use W, A, S, D to move)");
        lockMovementFunction("KING: The entire chamber is in ruins!", "Leave, servant.");
        lockMovementFunction("DEMON: HA! Cannot find your dearest princess? Hmmm?", "By the summer solstice. I am waiting.");
        lockMovementFunction("KING: The summer soltace... That is in a week!", "Servant!!");
        lockMovementFunction("PLAYER: This is weird. There is no wall on the map.", "Good choice! Now we can properly play the game.");
        lockMovementFunction(textToUnlock: "PLAYER: I can't turn back. If I did, there would be no game.");

        //Check if it's the dialogue for changing scene
        if (currentText == "Fifty thousand pounds of gold! Now, begone!")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fifty thousand pounds of gold! Now, begone! ";
            StartCoroutine(TransitionToScene("FarmHut", 5f, 2.5f));
        }

        if (currentText == "Good choice! Now we can properly play the game.")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Good choice! Now we can properly play the game. ";
            StartCoroutine(TransitionToScene("PushingStonePuzzle", 4f, 2f));
        }

        if (currentText == "PLAYER: I can't turn back. If I did, there would be no game.")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "PLAYER: I can't turn back. If I did, there would be no game. ";
            StartCoroutine(TransitionToScene("PushingStonePuzzle", 4f, 2f));
        }

        if (currentText == "I’ll leave first thing tomorrow.")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "I’ll leave first thing tomorrow. ";
            StartCoroutine(TransitionToScene("FarmHut", 4f, 2f));
        }

        if (currentText == "*Sigh.*")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "*Sigh.* ";
            //Debug.Log("Found cabinetwithletter: " + GameObject.Find("CabinetWithLetter"));
            GameObject.Find("CabinetWithLetter").name = "CabinetWithLetterDone";
        }

    }

    //Check if Player Collide with Object
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Call the Invetory function
        inventoryFuction(collision);

        if (collision.gameObject.tag == "SceneTransition")
        {
            //Get the name of the scene
            nextScene = collision.gameObject.name.Remove(0, 17);

            //Call scene transition function (which is a coroutine that allows the code to pause)
            StartCoroutine(TransitionToScene(nextScene, fadeDuration, timeBeforeFadeIn));
        }
    }

    void inventoryFuction(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pickable")
        {
            currentPickableItem = collision.gameObject;
            //Check if object dosen't exist in dictionary
            if (pickableGameObjects.ContainsKey(currentPickableItem.name) == false)
            {
                //Go the prefab form the prefab folder by it's name and adds to the dictionary
                pickableGameObjects.Add(currentPickableItem.name, (GameObject)Resources.Load("Prefabs/" + currentPickableItem.name, typeof(GameObject)));
            }
        }
    }

    void lockMovementFunction(string textToLock = "defaultParameter", string textToUnlock = "defaultParameter")
    {
        if (currentText == textToLock)
        {
            Debug.Log("Lock Movement");
            lockedMovement = true;
        }
        if (currentText == textToUnlock)
        {
            Debug.Log("Unlock Movement");
            lockedMovement = false;
        }
    }

    IEnumerator TransitionToScene(string sceneName, float duration, float timeBefore)
    {
        //Make sure movement variables aren't updated
        //Start fading in
        inTransition = true;
        fadeScriptObject.BeginFade(1, duration);
        //Don't transition to new scene until fully faded in and waited for an amount of time
        yield return new WaitForSeconds(duration + timeBefore);
        textObject.optionTree = "";
        textObject.hasNextOption = false;
        textObject.hasNextPage = false;
        textObject.virtualActivation = false;
        textObject.notOption = true;
        textObject.currentPage = 0;
        Debug.Log("Hmmm: " + textState);
        textObject.DisplayDialog(false);
        SceneManager.LoadScene(sceneName);
    }
}