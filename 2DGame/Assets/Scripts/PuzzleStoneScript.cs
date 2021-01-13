using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleStoneScript : MonoBehaviour
{
    Rigidbody2D rigidBody2D;

    Vector2 RigidbodyPosition;
    Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        RigidbodyPosition = rigidBody2D.position;
        targetPosition = new Vector2(999, 999);
    }

    // Update is called once per frame
    void Update()
    {
        RigidbodyPosition = new Vector2(Mathf.Round(rigidBody2D.position.x * 10f) / 10f, Mathf.Round(rigidBody2D.position.y * 10f) / 10f);
        if (RigidbodyPosition == targetPosition)
        {
            Debug.Log("stop");
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            targetPosition = new Vector2(999, 999);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collide");
            rigidBody2D.constraints = RigidbodyConstraints2D.None;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            targetPosition = RigidbodyPosition + collision.gameObject.GetComponent<PlayerController>().lookDirection;
            rigidBody2D.MovePosition(rigidBody2D.position + collision.gameObject.GetComponent<PlayerController>().lookDirection * 0.1f * Time.deltaTime);
        }
    }
}
