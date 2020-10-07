using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RubyController : MonoBehaviour
{
    //Movement Variables
    Rigidbody2D rigidBody2D;
    float horizontal;
    float vertical;
    public Vector2 lookDirection = new Vector2(1, 0);
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
    private bool textState = false;
    public GameObject textBox;
    public float raycastDistance = 50f;
    //Walk away text limit
    public float raycastLimitDistance = 1.5f;
    TextScript textObject;

    Animator animator;

    //Scene Transition Variable
    string nextScene;
    public bool inTransition = false;

    //Fading variables
    FadingScript fadeScriptObject;
    public float fadeDuration = 1f;
    public float timeBeforeFadeIn = 0.5f;
    
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
        fadeScriptObject = GameObject.Find("FadingScreen").GetComponent<FadingScript>();
    }

    // Update is called once per frame
    void Update()
    {
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

        //Don't update movement variables uring scene transition
        if (!inTransition)
        {
            Vector2 move = new Vector2(horizontal, vertical);
            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            Vector2 position = rigidBody2D.position;
            position += move * speed * Time.deltaTime;
            rigidBody2D.MovePosition(position);
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
                        textState = true;
                        textObject.interactablePos = hit.collider.gameObject.GetComponent<Rigidbody2D>().position;
                        textObject.currentTextObjectName = hit.collider.name;
                    }
                    else if (!textObject.hasNextPage)
                    {
                        textState = false;
                    }
                    textObject.DisplayDialog(textState);
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
                Debug.Log("IT's NOTHING");
            }
        }

    //Check distance between Player and Object, if it's more than "raycastLimitDistance"  ALL dialog turn off
    if (textState == true && Vector2.Distance(textObject.interactablePos, rigidBody2D.position) > raycastLimitDistance)
    {
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
        textObject.notOption = true;
        textObject.currentPage = 0;
        textObject.DisplayDialog(textState);
        }
    }

    //Check if Player Collide with Object
    void OnCollisionEnter2D(Collision2D collision)
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

        if (collision.gameObject.tag == "SceneTransition")
        {
            //Get the name of the scene
            nextScene = collision.gameObject.name.Remove(0, 17);

            //Call scene transition function (which is a coroutine that allows the code to pause)
            StartCoroutine(TransitionToScene(nextScene));
        }
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        //Make sure movement variables aren't updated
        //Start fading in
        inTransition = true;
        fadeScriptObject.BeginFade(1, fadeDuration);

        //Don't transition to new scene until fully faded in and waited for an amount of time
        yield return new WaitForSeconds(fadeDuration + timeBeforeFadeIn);
        SceneManager.LoadScene(sceneName);
    }
}