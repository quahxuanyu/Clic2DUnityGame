using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
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
    public Dictionary<string, GameObject> pickableGameObjects = new Dictionary<string, GameObject>(); //Saving picked up item Prefab, used when throwing out item
    public GameObject currentPickableItem;
    GameObject currentDroppedItem;
    public string currentSelectedItem;

    //Text Variables
    public bool textState = false;
    public GameObject textBox;
    public float raycastDistance = 50f;
    //Walk away text limit
    private float raycastLimitDistance = 2f;
    public TextScript textObject;
    string currentText;

    //MiniMap
    public GameObject MiniMapObject;
    MiniMapScript miniMapObjectScript;

    //Compass
    public GameObject CompassObject;
    CompassScript compassObjectScript;

    //Lighting variables
    public GameObject LightObject;
    Light2D lightScript;

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

    //Audio Variables
    public GameObject footSteps;

    //List of completed quiz boxes
    HashSet<string> quizBoxes = new HashSet<string>();


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
        lightScript = LightObject.GetComponent<Light2D>();
        lockedMovement = true;
        lookDirection = new Vector2(0, -1);

        if (SceneManager.GetActiveScene().name == "DiningRoomFinale")
        {
            var transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
            transformParticles.Stop();
        }
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

            //Foot Steps Audio
            if (move.x != 0 || move.y != 0)
            {
                footSteps.GetComponent<AudioSource>().volume = 0.25f;
            }
            else
            {
                footSteps.GetComponent<AudioSource>().volume = 0;
            }
        }
        else
        {
            //Debug.Log("Not Moving  " + "In transition: " + inTransition + lockedMovement);
            footSteps.GetComponent<AudioSource>().volume = 0;
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
                Debug.Log("hit.collider.gameObject.name:" + hit.collider.gameObject.name + " textObject.notOption:" + textObject.notOption);

                if (hit.collider.gameObject.tag == "TextInteract" && textObject.notOption)
                {
                    // REVIEW LATER "if (textObject.hasNextPage == false)" AND "else if (!textObject.hasNextPage)"
                    if (textObject.hasNextPage == false)
                    {
                        Debug.Log("First" + hit.collider.name);
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

                if (hit.collider.gameObject.tag == "SceneTransitionInteract")
                {
                    //Get the name of the scene
                    if (hit.collider.gameObject.name == "CabinetWithKey")
                    {
                        nextScene = "KeyPuzzle";
                    }

                    //Call scene transition function (which is a coroutine that allows the code to pause)
                    StartCoroutine(TransitionToScene(nextScene, fadeDuration, timeBeforeFadeIn));
                }

                if (hit.collider.gameObject.tag == "ItemInteract")
                {
                    if (hit.collider.gameObject.name == "CropsPuzzleShed")
                    {
                        if (inventoryAmount["FullKey"] == 1)
                        {
                            inventoryAmount["FullKey"] = 0;
                            addItemToInventory((GameObject)Resources.Load("Prefabs/" + "Part2WoodenParts", typeof(GameObject)));
                        }
                        else
                        {
                            // Display dialogue
                        }
                    }

                    if (hit.collider.gameObject.name == "Lake")
                    {
                        if (inventoryAmount["bucketEmpty"] == 1)
                        {
                            inventoryAmount["bucketEmpty"] = 0;
                            addItemToInventory((GameObject)Resources.Load("Prefabs/" + "bucketFull", typeof(GameObject)));
                        }
                        else
                        {

                        }
                    }

                    if (hit.collider.gameObject.name == "BadFarm")
                    {
                        if (inventoryAmount["bucketFull"] == 1)
                        {
                            inventoryAmount["bucketFull"] = 0;
                            inventoryAmount["bucketEmpty"] = 1;
                            InventoryScript.InventoryUpdate();
                            hit.collider.gameObject.SetActive(false);
                            textObject.virtualActivationFuntion("OldManFarmer2", gameObject.transform.position);
                        }
                        else
                        {

                        }
                    }

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
            StartCoroutine(TransitionToScene("Dilemma", fadeDuration, timeBeforeFadeIn));
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

        //Torch
        if (currentSelectedItem == "Torch")
        {
            lightScript.pointLightOuterRadius = 8f;
        }
        else
        {
            lightScript.pointLightOuterRadius = 4f;
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
        lockMovementFunction("Your Majesty.");
        lockMovementFunction(textToUnlock: "PLAYER: I can't turn back. If I did, there would be no game.");

        lockMovementDilemma("First question:", "The kingdom is destroyed because of your selfish actions.");
        lockMovementDilemma(textToUnlock: "You dearest is dead because you tried to be so noble!");
        lockMovementDilemma("Your dearest is dying, and you have the power to save her!", "Are you willing to subject your dearest to joyless life");
        lockMovementDilemma(textToUnlock: "Are you willing to subject your dearest to a painful death");
        lockMovementDilemma("You have the power to end all of the world's problems!", "Now the world is going to end!");
        lockMovementDilemma(textToUnlock: "You had the power to make the world a better place and you didn't do it!");
        lockMovementDilemma("You are tied to a tree and you're going to starve to death", "You selfish bastard! That peasant was innocent!");
        lockMovementDilemma(textToUnlock: "Now you're going to die, and leave your dearest to grieve!");


        //Check if it's the dialogue for changing scene
        fadeOnDialogue("Fifty thousand pounds of gold! Now, begone!", "FarmHut", 5f, 2.5f);
        fadeOnDialogue("I’ll leave first thing tomorrow.", "FarmHut", 4f, 2f);
        fadeOnDialogue("Good choice! Now we can properly play the game.", "PushingStonePuzzle", 4f, 2f);
        fadeOnDialogue("PLAYER: I can't turn back. If I did, there would be no game.", "PushingStonePuzzle", 4f, 2f);
        fadeOnDialogue("OLD MAN: Thank you so much! You are free to go now.", "Beach", 4f, 2f);
        fadeOnDialogue("Wait here until I return. ", "DiningRoomFinale", 4f, 2f);
        fadeOnDialogue("I will do what I want. That is none of your business. ", "DiningRoomFinale", 4f, 2f);
        fadeOnDialogue("What hell have I willingly stepped into?", "Ending", 4f, 2f);

        //Cabinet Letter Change Name
        if (currentText == "*Sigh.*")
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "*Sigh.* ";
            GameObject.Find("CabinetWithLetter").name = "CabinetWithLetterDone";
        }

        // Change the quiz box if we've done the question
        if (SceneManager.GetActiveScene().name == "Dilemma")
        {
            ChangeQuizBoxToDone(textObject, "QuizBoxA");
            ChangeQuizBoxToDone(textObject, "QuizBoxB");
            ChangeQuizBoxToDone(textObject, "QuizBoxC");
            ChangeQuizBoxToDone(textObject, "QuizBoxD");
        }
    }


    void ChangeQuizBoxToDone(TextScript textObject, String quizBoxName)
    {
        if (textObject.currentTextObjectName == quizBoxName)
        {
            var quizBox = GameObject.Find(quizBoxName);
            var quizBoxDone = (Sprite)Resources.Load("Art/Sprites/Environment/Other/questionBoxDone",
                typeof(Sprite));
            var quizBoxSpriteRenderer = quizBox.GetComponent<SpriteRenderer>();
            quizBoxSpriteRenderer.sprite = quizBoxDone;
            quizBoxes.Add(quizBoxName);
        }
    }

    //Check if Player Collide with Object
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pickable")
        {
            //Call the Invetory function
            addItemToInventory(collision.gameObject);
        }

        if (collision.gameObject.tag == "SceneTransition")
        {
            //Get the name of the scene
            nextScene = collision.gameObject.name.Remove(0, 17);

            //Call scene transition function (which is a coroutine that allows the code to pause)
            StartCoroutine(TransitionToScene(nextScene, fadeDuration, timeBeforeFadeIn));
        }
    }

    public void addItemToInventory(GameObject item)
    {
        if (item.tag == "Pickable")
        {
            currentPickableItem = item;
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
        if (currentText.Contains(textToLock))
        {
            Debug.Log("Lock Movement");
            lockedMovement = true;
        }
        if (currentText.Contains(textToUnlock))
        {
            Debug.Log("Unlock Movement");
            lockedMovement = false;
        }
    }

    
    void lockMovementDilemma(string textToLock = "defaultParameter", string textToUnlock = "defaultParameter")
    {
        if (currentText.Contains(textToLock))
        {
            Debug.Log("Lock Movement");
            lockedMovement = true;
        }
        if (currentText.Contains(textToUnlock))
        {
            Debug.Log("Unlock Movement");
            lockedMovement = false;
            if (quizBoxes.Count == 4)
            {
                StartCoroutine(TransitionToScene("CropsPuzzleHouse", 4f, 2f));
            }
        }
    }

    public void fadeOnDialogue(string dialogue, string scene, float duration, float timeBefore)
    {
        if (currentText == dialogue)
        {
            textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogue + " ";
            StartCoroutine(TransitionToScene(scene, duration, timeBefore));
        }
    }

    public IEnumerator TransitionToScene(string sceneName, float duration, float timeBefore)
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
        if (sceneName == "Menu")
        {
            foreach (var root in gameObject.scene.GetRootGameObjects())
                Destroy(root);
        }
        SceneManager.LoadScene(sceneName);
    }
}