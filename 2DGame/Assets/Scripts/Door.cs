using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public GameObject doorPair;
    public Vector3 offSet = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        Transform rubyTransform = other.collider.GetComponent<Transform>();

        rubyTransform.position = doorPair.transform.position - offSet;
    }
}
