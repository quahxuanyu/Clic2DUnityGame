using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    Rigidbody2D rigidBody;
    RectTransform rectTransform;
    bool state = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);
        if (state == true)
        {
            if (rectTransform.localPosition.y <= 30)
            {
                Debug.Log(rigidBody.position.y);
                rigidBody.position += new Vector2(0, 1750) * Time.deltaTime;
            }
        }
        else
        {
            if (rectTransform.localPosition.y >= -720)
            {
                Debug.Log(rigidBody.position.y);
                rigidBody.position += new Vector2(0, -1750) * Time.deltaTime;
            }
        }
    }

    public void MiniMap(bool boolean)
    {
        state = boolean;
    }
}
