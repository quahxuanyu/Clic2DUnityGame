using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    public Vector2 lookDirection = new Vector2(1, 0);
    public float speed = 3.0f;
    public float raycastDistance = 2.5f;
    public float raycastLimitDistance = 1.5f;
    private bool textState = false;

    public GameObject textBox;
    TextScript textObject;

    // Start is called before the first frame update
    void Start()
    {
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
                if (hit.collider.gameObject.tag == "TextInteract")
                {
                    if (textState == false)
                    {
                        textState = !textState;
                        textObject.interactablePos = hit.collider.gameObject.GetComponent<Rigidbody2D>().position;
                        textObject.currentText = hit.collider.name;
                    }
                    else
                    {
                        textState = !textState;
                    }
                    textObject.DisplayDialog(textState);
                }
            }
        }
       
        //Check distance between Player and Object, if it's more than "raycastLimitDistance" dialog turn off
        if (textState == true && Vector2.Distance(textObject.interactablePos, rigidBody2D.position) > raycastLimitDistance)
        {
            textState = !textState;
            textObject.DisplayDialog(textState);
        }
    }
}