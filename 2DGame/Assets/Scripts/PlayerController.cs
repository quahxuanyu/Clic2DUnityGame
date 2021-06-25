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
    public Rigidbody2D rigidBody2D;
    float horizontal;
    float vertical;
    public Vector2 lookDirection;
    public float speed = 2f;

    //Inventory Variables
    public GameObject Inventory;
    public DisplayInventory InventoryScript;
    public Dictionary<string, int> inventoryAmount = new Dictionary<string, int>();
    public Dictionary<string, GameObject> pickableGameObjects = new Dictionary<string, GameObject>(); //Saving picked up item Prefab, used when throwing out item
    public GameObject currentPickableItem;
    GameObject currentDroppedItem;
    public string currentSelectedItem;

    //Text Variables
    public GameObject textBox;
    public float raycastDistance = 50f;

    //Walk away text limit
    private float raycastLimitDistance = 2f;
    public TextScript textObjectScript;

    //MiniMap
    public GameObject MiniMapObject;
    public MiniMapScript miniMapObjectScript;

    //Compass
    public GameObject CompassObject;
    public CompassScript compassObjectScript;

    //Lighting variables
    public GameObject LightObject;
    public Light2D lightScript;

    public Animator animator;

    //Scene Transition Variable
    string nextScene;
    public bool inTransition = false;
    public bool lockedMovement;
    public bool shiftKeyDown = false;


    //Fading variables
    public GameObject fadeScreen;
    public float fadeDuration = 1f;
    public float timeBeforeFadeIn = 0.5f;
    public FadingScript fadeObjectScript;

    //Audio Variables
    public GameObject footSteps;

    //List of completed quiz boxes
    HashSet<string> quizBoxes = new HashSet<string>();

    public GameObject gameController;
    public GameControllerScript gameControllerObject;

    //Item detection
    GameObject hitObject;
    static string[] altItems = { "chewed_bubblegum", "Part1Rim", "Part2WoodenParts", "Part3Plug", "Part4Handle", "bucketEmpty" };
    static Dictionary<string, string> altDialoguesBucket = new Dictionary<string, string>
                {
                    { "CropsPuzzleShed", "CropsPuzzleShedBroken" },
                    { "Lake", "CropsPuzzleLakeBroken" },
                    { "BadFarm", "CropsPuzzleFarmBroken" }
                };
    static Dictionary<string, Dictionary<string, string>> altDialogues = new Dictionary<string, Dictionary<string, string>>
                {
                    { "chewed_bubblegum", altDialoguesBucket},
                    { "Part1Rim", altDialoguesBucket},
                    { "Part2WoodenParts", altDialoguesBucket},
                    { "Part3Plug", altDialoguesBucket},
                    { "Part4Handle", altDialoguesBucket},
                    { "bucketEmpty", new Dictionary<string, string>
                        {
                            { "CropsPuzzleShed", "CropsPuzzleShedEmptyBucket"},
                            { "BadFarm", "CropsPuzzleFarmEmptyBucket" }
                        }
                    }
                };

    // Start is called before the first frame update
    void Start()
    {
        //NOTE: IF ERRORS LIKE "OBJECT REFERENCE NOT SET TO AN INSTANCE OF AN OBJECT"
        //WHEN YOU CHANGE TO A DIFFERENT SCENE. IT IS DUE TO THE FACT THAT IF ONE OF
        //THESE OBJECTS WERE NOT ATTACHED, IT GIVES OUT AN ERROR, AND
        //EVERYTHING ELSE AFTER THAT LINE DOSEN'T RUN
        currentPickableItem = null;
        lockedMovement = true;
        lookDirection = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        //MOVEMENT
        //Debug.Log(horizontal.ToString());
        //Debug.Log(Input.GetAxis("Horizontal").ToString());

        //Debug.Log("Game Controller Text: " + gameControllerObject.currentText);
        //Debug.Log("Text Object Text: " + textObjectScript.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        //Debug.Log(Input.mouseScrollDelta.y);

        //Debug.Log("GameController Player: " + gameControllerObject.player);
        //Debug.Log("GameController PlayerObject: " + gameControllerObject.playerObject);
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && textObjectScript.virtualActivation && textObjectScript.notOption)
            {
                textObjectScript.interactablePos = gameObject.transform.position;
                textObjectScript.DisplayDialog();
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidBody2D.position + Vector2.up * 0.2f, lookDirection, raycastDistance, LayerMask.GetMask("NonPlayerCharacter"));

                if (hit.collider != null)
                {
                    hitObject = hit.collider.gameObject;
                    //Debug.Log("hit.collider.gameObject.name:" + hit.collider.gameObject.name + " textObjectScript.notOption:" + textObjectScript.notOption);

                    if (hitObject.CompareTag("TextInteract") && textObjectScript.notOption)
                    {
                        // REVIEW LATER "if (textObjectScript.hasNextPage == false)" AND "else if (!textObjectScript.hasNextPage)"
                        textObjectScript.interactablePos = gameObject.transform.position;
                        textObjectScript.currentTextObjectName = hit.collider.name;
                        textObjectScript.DisplayDialog();
                    }

                    if (hitObject.CompareTag("SceneTransitionInteract"))
                    {
                        //Get the name of the scene
                        if (hitObject.name == "CabinetWithKey")
                        {
                            nextScene = "KeyPuzzle";
                        }

                        //Call scene transition function (which is a coroutine that allows the code to pause)
                        StartCoroutine(TransitionToScene(nextScene, fadeDuration, timeBeforeFadeIn));
                        nextScene = "";
                    }

                    if (hitObject.CompareTag("ItemInteract"))
                    {
                        checkItemInInventory("CropsPuzzleShed", "FullKey", "CropsPuzzleShedNoKey", "Part2WoodenParts");
                        checkItemInInventory("Lake", "bucketEmpty", "CropsPuzzleLakeNoBucket", "bucketFull");
                        checkItemInInventory("BadFarm", "bucketFull", "CropsPuzzleFarmNoBucket", "bucketEmpty", "OldManFarmer2");
                        checkItemInInventory("AllFence", "PacketOfPigFood", "FarmNoPigFood",  "Pigsties");
                        checkItemInInventory("TownMailbox", "Letter", "TownNoLetter", "mailBoxLetterText");
                        checkItemInInventory("Carriage", "LastSupper", "CarriageNoFixPart", "carraigeFix");
                    }
                }
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
            currentPickableItem.SetActive(false);
            currentPickableItem = null;
        }

        //Drop Item
        /*if (Input.GetKeyDown(KeyCode.Q))
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
        }*/

        //change Scene short cut
        changeSceneForTesting();

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
        if (textObjectScript.currentTextObjectName != "" && Vector2.Distance(textObjectScript.interactablePos, rigidBody2D.position) > raycastLimitDistance)
        {
            if (textObjectScript.notOption == false)
            {
                for (int i = 1; i <= textObjectScript.optionObject.numOfButtons; i++)
                {
                    Destroy(textObjectScript.optionObject.gameObject.transform.GetChild(i).gameObject);
                }
            }
            textObjectScript.currentTextObjectName = "";
            textObjectScript.optionTree = "";
            textObjectScript.hasNextOption = false;
            textObjectScript.hasNextPage = false;
            textObjectScript.virtualActivation = false;
            textObjectScript.notOption = true;
            textObjectScript.currentPage = 0;
            //Debug.Log("possibility problem 2: " + textState);
            //Debug.Log("Distance over limit: " + Vector2.Distance(textObjectScript.interactablePos, rigidBody2D.position));
            //Debug.Log("Hmmm: " + textState);
            textObjectScript.DisplayDialog();
        }

        //Lock movement during certain dialogues
        lockMovementFunction(textToUnlock: "KING:What is going on? I wonder...\n \n (use W, A, S, D to move)");
        lockMovementFunction("KING: The entire chamber is in ruins!", "Leave, servant.");
        lockMovementFunction("DEMON: HA! Cannot find your dearest princess? Hmmm?", "By the summer solstice. I am waiting.");
        lockMovementFunction("KING: The summer soltace... That is in a week!", "Servant!!");
        lockMovementFunction("PLAYER: This is weird. There is no wall on the map.", "Good choice! Now we can properly play the game.");
        lockMovementFunction("Your Majesty.");
        lockMovementFunction(textToUnlock: "PLAYER: I can't turn back. If I did, there would be no game.");
        lockMovementFunction("DEMON: MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!!\n\nIt is time for your painful undoing!", "MUAHAHAHAHAHAHAHAHA!!!!!!!!!!!!!!!");

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
        if (gameControllerObject.currentText == "*Sigh.*")
        {
            gameControllerObject.currentText = "*Sigh.* ";
            GameObject.Find("CabinetWithLetter").name = "CabinetWithLetterDone";
        }

        if (gameControllerObject.currentText == "GUY: I gotta go now, wish you luck on your journy to rescue the princess")
        {
            gameControllerObject.currentText = "GUY: I gotta go now, wish you luck on your journy to rescue the princess ";
            StartCoroutine(CarriageFade(4f, 2f));
        }

        // Change the quiz box if we've done the question
        if (SceneManager.GetActiveScene().name == "Dilemma")
        {
            ChangeQuizBoxToDone(textObjectScript, "QuizBoxA");
            ChangeQuizBoxToDone(textObjectScript, "QuizBoxB");
            ChangeQuizBoxToDone(textObjectScript, "QuizBoxC");
            ChangeQuizBoxToDone(textObjectScript, "QuizBoxD");
        }

        changeTagOnDialogue("DiningRoom", "DiningTable", "Servant!");
        changeTagOnDialogue("Corridor", "TransitionToScenePrincessChamber", "Follow me, your majesty...", "SceneTransition");
        changeTagOnDialogue("Farm", "Carriage", "A replacement is required.", "ItemInteract");
    }

    private void changeSceneForTesting()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            shiftKeyDown = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            shiftKeyDown = false;
        }
        if (!shiftKeyDown)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Corridor", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("PrincessChamber", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("FarmHut", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Farm", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Forest", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("PushingStonePuzzle", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("StoneMaze", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Dilemma", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("CropsPuzzleHouse", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("KeyPuzzle", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("CropsPuzzle", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Beach", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Ending", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("DiningRoomFinale", fadeDuration, timeBeforeFadeIn));
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            lockedMovement = false;
            StartCoroutine(TransitionToScene("Ending", fadeDuration, timeBeforeFadeIn));
        }
    }

    void ChangeQuizBoxToDone(TextScript textObjectScript, String quizBoxName)
    {
        if (textObjectScript.currentTextObjectName == quizBoxName)
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
        if (collision.gameObject.CompareTag("Pickable"))
        {
            //Call the Invetory function
            addItemToInventory(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("SceneTransition"))
        {
            //Get the name of the scene
            nextScene = collision.gameObject.name.Remove(0, 17);

            //Call scene transition function (which is a coroutine that allows the code to pause)
            StartCoroutine(TransitionToScene(nextScene, fadeDuration, timeBeforeFadeIn));
        }
    }

    public void addItemToInventory(GameObject item)
    {
        if (item.CompareTag("Pickable"))
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
        if (gameControllerObject.currentText.Contains(textToLock))
        {
            lockedMovement = true;
            animator.SetFloat("Speed", 0);
        }
        else if (gameControllerObject.currentText.Contains(textToUnlock))
        {
            lockedMovement = false;
        }
    }

    
    void lockMovementDilemma(string textToLock = "defaultParameter", string textToUnlock = "defaultParameter")
    {
        if (gameControllerObject.currentText.Contains(textToLock))
        {
            lockedMovement = true;
            animator.SetFloat("Speed", 0);
        }
        else if (gameControllerObject.currentText.Contains(textToUnlock))
        {
            lockedMovement = false;
            if (quizBoxes.Count == 4)
            {
                quizBoxes.Add("quizBoxDone");
                StartCoroutine(TransitionToScene("CropsPuzzleHouse", 4f, 2f));
            }
        }
    }

    public void fadeOnDialogue(string dialogue, string scene, float duration, float timeBefore)
    {
        if (gameControllerObject.currentText == dialogue)
        {
            gameControllerObject.currentText = dialogue + " ";
            StartCoroutine(TransitionToScene(scene, duration, timeBefore));
        }
    }

    void checkItemInInventory(string obj, string target, string notFoundDialogue, string newItem = "", string dialogue = "")
    {
        if (hitObject.name == obj)
        {
            Debug.Log("currentSelectedItem: " + currentSelectedItem);
            if (currentSelectedItem == target)
            {
                Debug.Log("found");
                if (currentSelectedItem == "bucketFull")
                {
                    hitObject.SetActive(false);
                }
                if (dialogue != "")
                {
                    textObjectScript.virtualActivationFuntion(dialogue, gameObject.transform.position);
                }
                inventoryAmount[currentSelectedItem] = 0;
                InventoryScript.InventoryUpdate();
                if (newItem != "")
                {
                    addItemToInventory((GameObject)Resources.Load("Prefabs/" + newItem, typeof(GameObject)));
                }
            }
            else
            {
                foreach (var i in altItems)
                {
                    if (currentSelectedItem == i)
                    {
                        textObjectScript.virtualActivationFuntion(altDialogues[currentSelectedItem][obj], gameObject.transform.position);
                        return;
                    }
                }
                textObjectScript.virtualActivationFuntion(notFoundDialogue, gameObject.transform.position);
            }
        }
    }

    public void changeTagOnDialogue(string scene, string obj, string dialogue = "", string newTag = "Untagged")
    {
        if (SceneManager.GetActiveScene().name == scene)
        {
            if (gameControllerObject.currentText == dialogue && textObjectScript.currentTextObjectName == "" || dialogue == "")
            {
                GameObject.Find(obj).tag = newTag;
            }
        }
    }
    IEnumerator CarriageFade(float duration, float timeBefore)
    {
        //Make sure movement variables aren't updated
        //Start fading in
        inTransition = true;
        fadeObjectScript.BeginFade(1, duration);
        //Don't transition to new scene until fully faded in and waited for an amount of time
        yield return new WaitForSeconds(duration + timeBefore);
        GameObject.Find("Carriage").SetActive(false);
        textObjectScript.optionTree = "";
        textObjectScript.hasNextOption = false;
        textObjectScript.hasNextPage = false;
        textObjectScript.virtualActivation = false;
        textObjectScript.notOption = true;
        textObjectScript.currentPage = 0;
        textObjectScript.DisplayDialog();
        inTransition = false;
        fadeObjectScript.BeginFade(-1, 1);
    }

    public IEnumerator TransitionToScene(string sceneName, float duration, float timeBefore)
    {
        //Make sure movement variables aren't updated
        //Start fading in
        inTransition = true;
        fadeObjectScript.BeginFade(1, duration);
        //Don't transition to new scene until fully faded in and waited for an amount of time
        yield return new WaitForSeconds(duration + timeBefore);
        textObjectScript.currentTextObjectName = "";
        textObjectScript.optionTree = "";
        textObjectScript.hasNextOption = false;
        textObjectScript.hasNextPage = false;
        textObjectScript.virtualActivation = false;
        textObjectScript.notOption = true;
        textObjectScript.currentPage = 0;
        textObjectScript.DisplayDialog();
        if (sceneName == "Menu")
        {
            foreach (var root in gameObject.scene.GetRootGameObjects())
                Destroy(root);
        }
        SceneManager.LoadScene(sceneName);
    }
}