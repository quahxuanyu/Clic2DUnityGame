using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    public Vector2 lookDirection = new Vector2(1, 0);
    public float speed = 3.0f;
    public float raycastDistance = 2.5f;
    public float raycastLimitDistance = 1.5f;
    private bool textState = false;

    public GameObject strawberry;
    GameObject[] GOs;
    public Dictionary<string, int> inventoryAmount = new Dictionary<string, int>();
    public Dictionary<string, GameObject> pickableGameObjects;
    GameObject currentPickableItem;
    GameObject currentDroppedItem;
    string currentSelectedItem;

    public GameObject textBox;
    TextScript textObject;

    GameObject emptyGO;

    // Start is called before the first frame update
    void Start()
    {
        emptyGO = new GameObject();
        GOs = new GameObject[] { strawberry };
        pickableGameObjects = GOs.ToDictionary(GO => GO.name, GO => GO);
        currentPickableItem = emptyGO;
        rigidBody2D = GetComponent<Rigidbody2D>();
        textObject = textBox.GetComponent<TextScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        Vector2 position = rigidBody2D.position;

        position = position + move * speed * Time.deltaTime;

        rigidBody2D.MovePosition(position);

        if (horizontal != 0 || vertical != 0)
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        //RAYCAST
        //Check If Raycast hit anything when "X" is pressed, if yes turn on the dialog
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidBody2D.position + Vector2.up * 0.2f, lookDirection, raycastDistance, LayerMask.GetMask("NonPlayerCharecter"));
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "TextInteract" && textObject.notOption)
                {
                    if (textObject.hasNextPage == false)
                    {
                        textState = true;
                        textObject.interactablePos = hit.collider.gameObject.GetComponent<Rigidbody2D>().position;
                        textObject.currentText = hit.collider.name;
                    }
                    else if (!textObject.hasNextPage)
                    { 
                        textState = false;
                    }
                    textObject.DisplayDialog(textState);
                }
            }
        }

        if (currentPickableItem.GetComponents<Component>().Length > 1)
        {
            if (inventoryAmount.ContainsKey(currentPickableItem.name))
            {
                inventoryAmount[currentPickableItem.name] += 1;
            }
            else
            {
                inventoryAmount[currentPickableItem.name] = 1;
            }
            currentSelectedItem = currentPickableItem.name;
            Destroy(currentPickableItem);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inventoryAmount[currentSelectedItem] > 0)
            {
                currentDroppedItem = Instantiate(pickableGameObjects[currentSelectedItem], rigidBody2D.position + lookDirection * 1.1f, Quaternion.identity);
                currentDroppedItem.name = pickableGameObjects[currentSelectedItem].name;
                inventoryAmount[currentDroppedItem.name] -= 1;
            }

            if (inventoryAmount[currentSelectedItem] == 0)
            {
                currentSelectedItem = "";
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pickable")
        {
            currentPickableItem = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pickable")
        {
            currentPickableItem = emptyGO;
        }
    }
}