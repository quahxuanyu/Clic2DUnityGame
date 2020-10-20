using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public GameObject canvas;
    public GameObject player;
    public GameObject backgroundMusic;
    public GameObject eventSystem;

    RubyController playerObject;
    SpriteRenderer playerSpriteRenderer;
    Rigidbody2D playerRigidBody2D;
    Animator playerAnimator;
    BoxCollider2D playerBoxCollider;

    TextScript textObjectScript;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerObject = player.GetComponent<RubyController>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerRigidBody2D = player.GetComponent<Rigidbody2D>();
        playerAnimator = player.GetComponent<Animator>();
        playerBoxCollider = player.GetComponent<BoxCollider2D>();

        textObjectScript = canvas.transform.GetChild(1).GetComponent<TextScript>();

        //Keep the objects regardless of scene change
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(backgroundMusic);
        DontDestroyOnLoad(eventSystem);
        /*
         //Old code for background music
         GameObject[] GOs = GameObject.FindGameObjectsWithTag("Music");
         if (GOs.Length > 1)
         {
            Destroy(this.gameObject);
         }
         */
    }

    //Make protagonist appear at the right places when transitioning to new scenes
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Corridor":
                playerObject.lookDirection = new Vector2(1, 0);
                playerRigidBody2D.MovePosition(new Vector2(-6, -2));
                playerObject.inTransition = false;
                break;

            case "PrincessChamber":
                textObjectScript.interactablePos = playerObject.transform.position;
                textObjectScript.currentTextObjectName = "Chamber";
                textObjectScript.virtualActivation = true;
                textObjectScript.DisplayDialog(true);
                //playerObject.textState = true;

                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.MovePosition(new Vector2(-7, 0));
                playerObject.inTransition = false;
                break;

            case "FarmHut":
                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.MovePosition(new Vector2(-4, 0));
                playerObject.inTransition = false;
                break;

            case "Farm":
                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.MovePosition(new Vector2(-0.4f, -2.75f));
                playerObject.inTransition = false;
                break;
        }
    }
}
