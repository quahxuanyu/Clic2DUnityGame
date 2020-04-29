using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public GameObject dialogBox;
    public Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        dialogBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //DISPLAY DIALOG
    public void DisplayDialog(bool state)
    {
        dialogBox.SetActive(state);
    }
}
