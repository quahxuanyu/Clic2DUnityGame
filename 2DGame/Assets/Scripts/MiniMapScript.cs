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
        if (state == true)
        {
            //gameObject.GetComponent<RectTransform>().position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(614, 432, 0), 200 * Time.deltaTime); 
            if (rectTransform.localPosition.y <= -30)
            {
                rigidBody.position += new Vector2(0, 1750) * Time.deltaTime;
            }
        }
        else
        {
            if (rectTransform.localPosition.y >= -720)
            {
                rigidBody.position += new Vector2(0, -1750) * Time.deltaTime;
            }
        }
    }

    public void MiniMap(bool boolean)
    {
        state = boolean;
    }
}
