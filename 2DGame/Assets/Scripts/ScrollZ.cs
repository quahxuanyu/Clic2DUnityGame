using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollZ : MonoBehaviour
{
    public float endOfText = 0;
    private static bool isScrolling = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isScrolling) {
            return;
        }

        Vector3 pos = transform.position;

        Vector3 localVectorUp = transform.TransformDirection(0, 0.25f, 0);
        pos += localVectorUp;

        transform.position = pos;

        if (transform.position.y > endOfText)
        {
          isScrolling = false;
        }
    }
}
