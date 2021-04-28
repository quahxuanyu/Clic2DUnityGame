using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CompassScript : MonoBehaviour
{
    Dictionary<string, Vector2> targetPositions = new Dictionary<string, Vector2>() {
        {"DiningRoom", new Vector2(-5.6f, 6.1f)},
        {"Corridor", new Vector2(0f, 0f)},
        {"PrincessChamber", new Vector2(0f, 0f)},
        {"FarmHut", new Vector2(32.04f, 0f)},
        {"Farm", new Vector2(32.04f, 0f)},
        {"Forest", new Vector2(8.59f, 8.1f)},
        {"PushingStonePuzzle", new Vector2(0f, 3.7f)},
        {"StoneMaze", new Vector2(0f, 0f)},
        {"Dilemma", new Vector2(0f, 0f)},
        {"CropsPuzzleHouse", new Vector2(0f, 0f)},
        {"KeyPuzzle", new Vector2(0f, 0f)},
        {"CropsPuzzle", new Vector2(0f, 0f)},
        {"Beach", new Vector2(0f, 0f)},
        {"WaterBucketPrototype", new Vector2(0f, 0f)}
    };

    GameObject playerObejct;
    GameObject compassNeedle;

    Rigidbody2D rigidBody;
    RectTransform rectTransform;
    bool state = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        playerObejct = GameObject.Find("Player");
        compassNeedle = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (state == true)
        {
            if (rectTransform.localPosition.y <= 30)
            {
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

        //Needle angle
        float playerX = playerObejct.transform.position.x - targetPositions[SceneManager.GetActiveScene().name].x;
        float playerY = playerObejct.transform.position.y - targetPositions[SceneManager.GetActiveScene().name].y;

        if (playerX > 0 && playerY > 0 || playerX > 0 && playerY < 0)
        {
            float angle = 90f + (Mathf.Rad2Deg * Mathf.Atan(playerY / playerX));
            //Debug.Log   (Mathf.Rad2Deg * Mathf.Atan(playerY / playerX));
            compassNeedle.GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            float angle = 270f + (Mathf.Rad2Deg * Mathf.Atan(playerY / playerX));
            compassNeedle.GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

    }

    public void Compass(bool boolean)
    {
        state = boolean;
    }
}
