using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public TextScript textObject;
    public Vector2 lookDirection = new Vector2(1, 0);
    public float speed = 3.0f;
    public float raycastDistance = 2.5f;
    public float raycastLimitDistance = 1.5f;
    private bool textState = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {   
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        Vector2 position = rigidbody2D.position;

        position = position + move * speed * Time.deltaTime;

        rigidbody2D.MovePosition(position);

        if (horizontal != 0 || vertical != 0)
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        //RAYCAST
        //Check If Raycast hit anything when "X" is pressed, if yes turn on the dialog
        if (Input.GetKeyDown(KeyCode.X) && textState == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2D.position + Vector2.up * 0.2f, lookDirection, raycastDistance, LayerMask.GetMask("NonPlayerCharecter"));
            if (hit.collider != null)
            {
                textObject = hit.collider.GetComponent<TextScript>();
                if (textObject != null)
                {
                    textState = true;
                    textObject.DisplayDialog(true);
                }
            }

        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            textState = false;
            textObject.DisplayDialog(false);
        }
       
        //Check distance between Player and Object, if it's more than "raycastLimitDistance" dialog turn off
        if (textState == true && Vector2.Distance(textObject.rigidbody2D.position, rigidbody2D.position) > raycastLimitDistance)
        {
            textState = false;
            textObject.DisplayDialog(false);
        }
    }
}