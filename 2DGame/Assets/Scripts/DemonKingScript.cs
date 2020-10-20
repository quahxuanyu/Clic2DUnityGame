using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKingScript : MonoBehaviour
{
    GameObject playerObject;
    Rigidbody2D demonKingRigidbody2D;

    GameObject textObject;
    TextScript textObjectScript;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");
        demonKingRigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        textObject = GameObject.Find("TextBox");
        textObjectScript = textObject.GetComponent<TextScript>();
    }

    public void StartAppear()
    {
        StartCoroutine(AppearAfter(2.5f));
    }

    IEnumerator AppearAfter(float time)
    {
        // Wait for an amount of time before appearing and displaying next dialogue
        yield return new WaitForSeconds(time);
        textObjectScript.interactablePos = playerObject.transform.position;
        textObjectScript.currentTextObjectName = gameObject.name;
        textObjectScript.virtualActivation = true;
        textObjectScript.DisplayDialog(true);
        demonKingRigidbody2D.MovePosition(new Vector2(3, -3));
        yield return new WaitForSeconds(0.1f);
        demonKingRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
