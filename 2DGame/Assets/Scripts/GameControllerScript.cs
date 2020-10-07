using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject Protagonist;
    public GameObject BackgroundMusic;

    RubyController ProtagonistObject;
    Rigidbody2D rigidBody2D;
    Animator animator;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ProtagonistObject = Protagonist.GetComponent<RubyController>();
        rigidBody2D = Protagonist.GetComponent<Rigidbody2D>();
        animator = Protagonist.GetComponent<Animator>();

        //Keep the objects regardless of scene change
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(Canvas);
        DontDestroyOnLoad(Protagonist);
        DontDestroyOnLoad(BackgroundMusic);
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
                ProtagonistObject.lookDirection = new Vector2(1, 0);
                rigidBody2D.MovePosition(new Vector2(-6, -2));
                ProtagonistObject.inTransition = false;
                break;

            case "PrincessChamber":
                ProtagonistObject.lookDirection = new Vector2(0, -1);
                rigidBody2D.MovePosition(new Vector2(-7, 0));
                ProtagonistObject.inTransition = false;
                break;

            case "FarmHut":
                ProtagonistObject.lookDirection = new Vector2(0, -1);
                rigidBody2D.MovePosition(new Vector2(-4, 0));
                ProtagonistObject.inTransition = false;
                break;

            case "Farm":
                ProtagonistObject.lookDirection = new Vector2(0, -1);
                rigidBody2D.MovePosition(new Vector2(-0.4f, -2.75f));
                ProtagonistObject.inTransition = false;
                break;
        }
    }
}
