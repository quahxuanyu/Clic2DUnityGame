using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollZ : MonoBehaviour
{
    float endOfText = 7f;
    private static bool isScrolling = true;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer player = GameObject.Find("Player").GetComponent<SpriteRenderer>();
        var color = player.color;
        color.a = 0;
        player.color = color;
        GameObject.Find("InventoryBar").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isScrolling) {
            return;
        }

        Vector3 pos = transform.position;

        Vector3 localVectorUp = transform.TransformDirection(0, 0.50f, 0);
        pos += localVectorUp * Time.deltaTime;

        transform.position = pos;


        if (gameObject.transform.localPosition.y > endOfText && isScrolling == true)
        { 
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerController>().TransitionToScene("Menu", 10, 0.1f));
        }
    }
}
