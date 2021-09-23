using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;
using TMPro;
using System;

public class SceneVar
{
    public Vector3 Scale { get; set; }
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 Position { get; set; }
}

public class GameControllerScript : MonoBehaviour
{
    public GameObject canvas;
    public GameObject player;
    public GameObject backgroundMusic;
    public GameObject footSteps;
    public GameObject eventSystem;

    GameObject MarketMerchant;
    GameObject FatSeller;

    public PlayerController playerObject;
    public SpriteRenderer playerSpriteRenderer;
    public Rigidbody2D playerRigidBody2D;
    public Animator playerAnimator;
    public BoxCollider2D playerBoxCollider;

    GameObject DemonKing;
    DemonKingScript DemonKingObject;

    public TextScript textObjectScript;

    GameObject vCam;
    CinemachineVirtualCamera vCamObject;

    Dictionary<string, SceneVar> sceneTransitionVariables;
    SceneVar currentSceneVar;

    float playerOriginalSpeed;

    public string currentText;

    void Awake()
    {
        Application.targetFrameRate = 60;
        SceneManager.sceneLoaded += OnSceneLoaded;
        checkAudioListener();

        //player = GameObject.Find("Player");

        playerOriginalSpeed = playerObject.speed;
        currentText = "";

        sceneTransitionVariables = new Dictionary<string, SceneVar>() {
            { "DiningRoom", new SceneVar {
                Scale = new Vector3(1, 1, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(1, 0),
                Position = new Vector2(1.63f, -0.8f)
                }
            },
            { "Corridor", new SceneVar {
                Scale = new Vector3(1.34f, 1.34f, 1),
                Speed = playerOriginalSpeed * 1.34f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(-6, -2)
                }
            },
            { "PrincessChamber", new SceneVar {
                Scale = new Vector3(2, 2, 1),
                Speed = playerOriginalSpeed * 2,
                Direction = new Vector2(0, -1),
                Position = new Vector2(-7, 0)
                }
            },
            { "FarmHut", new SceneVar {
                Scale = new Vector3(1.34f, 1.34f, 1),
                Speed = playerOriginalSpeed * 1.34f,
                Direction = new Vector2(0, -1),
                Position = new Vector2(-3.7f, 0.6f)
                }
            },
            { "Farm", new SceneVar {
                Scale = new Vector3(0.8f, 0.8f, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, -1),
                Position = new Vector2(-11.4f, -7f)
                }
            },
            { "Forest", new SceneVar {
                Scale = new Vector3(1, 1, 1),
                Speed = playerOriginalSpeed * 1.34f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(-7.24f, -0.550f)
                }
            },
            { "PushingStonePuzzle", new SceneVar {
                Scale = new Vector3(1, 1, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, 1),
                Position = new Vector2(0, -4)
                }
            },
            { "StoneMaze", new SceneVar {
                Scale = new Vector3(1, 1, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, 1),
                Position = new Vector2(0, -5)
                }
            },
            { "Dilemma", new SceneVar {
                Scale = new Vector3(1.5f, 1.5f, 1),
                Speed = playerOriginalSpeed * 1.5f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(-5, -4)
                }
            },
            { "CropsPuzzleHouse", new SceneVar {
                Scale = new Vector3(1.34f, 1.34f, 1),
                Speed = playerOriginalSpeed * 1.34f,
                Direction = new Vector2(0, -1),
                Position = new Vector2(3.7f, 0.6f)
                }
            },
            { "WaterBucketPrototype", new SceneVar {
                Scale = new Vector3(1.34f, 1.34f, 1),
                Speed = playerOriginalSpeed * 1.34f,
                Direction = new Vector2(0, -1),
                Position = new Vector2(0f, 0f)
                }
            },
            { "CropsPuzzle", new SceneVar {
                Scale = new Vector3(0.8f, 0.8f, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, -1),
                // Position = new Vector2(5f, -8f)
                Position = new Vector2(7.1f, 2.9f)
                }
            },
            { "KeyPuzzle", new SceneVar {
                Scale = new Vector3(0.8f, 0.8f, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, -1),
                Position = new Vector2(10f, 10f)
                }
            },
            { "Beach", new SceneVar {
                Scale = new Vector3(1.5f, 1.5f, 1),
                Speed = playerOriginalSpeed * 1.5f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(3.646802f, -2.34017f)
                }
            },
            { "Ending", new SceneVar {
                Scale = new Vector3(1.5f, 1.5f, 1),
                Speed = playerOriginalSpeed * 1.5f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(-1.3f, -1.81f)
                }
            },
            { "EndingTwo", new SceneVar {
                Scale = new Vector3(1.5f, 1.5f, 1),
                Speed = playerOriginalSpeed * 1.5f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(-1.3f, -1.81f)
                }
            },
            { "Sunset", new SceneVar {
                Scale = new Vector3(1.5f, 1.5f, 1),
                Speed = playerOriginalSpeed * 1.5f,
                Direction = new Vector2(1, 0),
                Position = new Vector2(0, 0)
                }
            },
            { "DiningRoomFinale", new SceneVar {
                Scale = new Vector3(1, 1, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(1, 0),
                Position = new Vector2(1.40f, -0.8f)
                }
            },
        };

        //Keep the objects regardless of scene change
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(backgroundMusic);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(footSteps);
    }

    private void checkAudioListener()
    {
        AudioListener[] aL = FindObjectsOfType<AudioListener>();
        for (int i = 0; i < aL.Length; i++)
        {
            //Ignore the first AudioListener in the array 
            if (i == 0)
                continue;
            //Destroy 
            DestroyImmediate(aL[i]);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Make protagonist appear at the right places when transitioning to new scenes
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        vCam = GameObject.Find("CM vcam1");
        vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
        vCamObject.m_Follow = playerObject.transform;

        currentSceneVar = sceneTransitionVariables[scene.name];
        playerObject.transform.localScale = currentSceneVar.Scale;
        playerObject.speed = currentSceneVar.Speed;

        playerObject.lookDirection = currentSceneVar.Direction;
        playerRigidBody2D.position = currentSceneVar.Position;
        playerObject.inTransition = false;
        playerObject.lockedMovement = false;

        canvas.transform.GetChild(5).GetComponent<FadingScript>().continueMusic = false;

        switch (scene.name)
        {
            case "PrincessChamber":
                textObjectScript.virtualActivationFuntion("Chamber", playerRigidBody2D.position);
                //playerObject.textState = true;

                DemonKing = GameObject.Find("DemonKing");
                DemonKingObject = DemonKing.GetComponent<DemonKingScript>();
                DemonKingObject.sceneLoaded = "PrincessChamber";
                break;

            case "DiningRoom":
                playerObject.lockedMovement = true;
                StartCoroutine(displayDialogueWithDelay(5f, "DiningTable"));
                break;

            case "FarmHut":
                //Debug.Log("Running FarmHut case");
                //Debug.Log(canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

                currentSceneVar.Direction = new Vector2(0, -1);
                currentSceneVar.Position = new Vector2(-3.7f, 0.6f);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                //Change letter name when boar food is done when entering the farm hut second time
                if (playerObject.hasFedPig)
                {
                    GameObject.Find("CabinetWithLetter").name = "CabinetWithLetter2";
                }

                //Change letter name after day two
                if (currentText == "I’ll leave first thing tomorrow. ")
                {
                    GameObject.Find("CabinetWithLetter2").name = "CabinetWithLetterDone";
                }

                //If it is not day two, change music
                if (currentText != "I’ll leave first thing tomorrow. ")
                {
                    backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Overworld_town_music") as AudioClip;
                    backgroundMusic.GetComponent<AudioSource>().Play();
                }
                break;

            case "Farm":
                sceneTransitionVariables["FarmHut"].Direction = new Vector2(0, 1);
                sceneTransitionVariables["FarmHut"].Position = new Vector2(4.3f, -4.8f);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                FatSeller = GameObject.Find("FatMerchant");
                FatSeller.SetActive(false);

                //Change market and carriage name when day two
                if (currentText == "I’ll leave first thing tomorrow. ")
                {
                    Debug.Log("MarketChanged");
                    GameObject.Find("MarketStandRed").name = "MarketStandRed2";
                    GameObject.Find("CarriageNotNextDayYet").name = "Carriage";
                    GameObject.Find("MarketMerchant").SetActive(false);
                    FatSeller.SetActive(true);
                }
                break;

            case "Forest":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/The_forest") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                DemonKing = GameObject.Find("DemonKing");
                DemonKingObject = DemonKing.GetComponent<DemonKingScript>();
                DemonKingObject.playerObject = player;
                DemonKingObject.textObject = canvas.transform.GetChild(1).gameObject;
                DemonKingObject.sceneLoaded = "Forest";
                break;

            case "PushingStonePuzzle":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Cave_Music") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                playerObject.LightObject.SetActive(true);
                break;

            case "StoneMaze":
                playerObject.LightObject.SetActive(true);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);
                break;

            case "Dilemma":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Game_Show") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                playerObject.LightObject.SetActive(false);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                //var transformParticles = GameObject.Find("TransformParticles").GetComponent<ParticleSystem>();
                //transformParticles.Play();
                StartCoroutine(displayDialogueWithDelay(2.5f, "QuizDemon"));
                break;

            case "CropsPuzzleHouse":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Crops_Puzzle_Music_2") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                currentSceneVar.Direction = new Vector2(0, -1);
                currentSceneVar.Position = new Vector2(3.7f, 0.6f);

                if (playerObject.inventoryAmount.ContainsKey("FullKey"))
                {
                    playerObject.changeTagOnDialogue("CropsPuzzleHouse", "CabinetWithKey");
                }

                playerObject.LightObject.SetActive(false);
                canvas.transform.GetChild(0).gameObject.SetActive(true);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);
                break;

            case "KeyPuzzle":
                sceneTransitionVariables["CropsPuzzleHouse"].Direction = new Vector2(0, 1);
                sceneTransitionVariables["CropsPuzzleHouse"].Position = new Vector2(1.07f, 1.47f);

                canvas.transform.GetChild(0).gameObject.SetActive(false);
                break;

            case "CropsPuzzle":
                sceneTransitionVariables["CropsPuzzleHouse"].Direction = new Vector2(0, 1);
                sceneTransitionVariables["CropsPuzzleHouse"].Position = new Vector2(-4.3f, -4.8f);

                playerObject.LightObject.SetActive(false);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Crops_Puzzle_Music_2") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();
                break;

            case "Beach":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/PrincessDemonKing") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                GameObject.Find("InventoryBar").SetActive(false);

                playerObject.LightObject.SetActive(false);

                canvas.transform.GetChild(5).GetComponent<FadingScript>().continueMusic = true;

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);
                StartCoroutine(displayDialogueWithDelay(2.5f, "1DemonKingBeach"));
                break;

            case "DiningRoomFinale":
                if (currentText.Contains("Wait here until I return."))
                {
                    StartCoroutine(displayDialogueWithDelay(2.5f, "EndingOne"));
                    playerObject.lockedMovement = true;
                }
                else
                {
                    StartCoroutine(displayDialogueWithDelay(2.5f, "EndingTwo"));
                    playerObject.lockedMovement = true;
                }
                playerSpriteRenderer.sortingLayerName = "Default";

                break;

            case "Sunset":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Epilogue") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();

                canvas.transform.GetChild(5).GetComponent<FadingScript>().continueMusic = true;

                playerObject.lockedMovement = true;
                StartCoroutine(transitionToScene(8, "Ending"));
                break;

            case "Ending":
                playerObject.lockedMovement = true;
                break;

            case "EndingTwo":
                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/Epilogue") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();
                playerObject.lockedMovement = true;
                break;
        }
    }

    IEnumerator displayDialogueWithDelay(float time, string text)
    {
        // Wait for an amount of time before displaying next dialogue
        yield return new WaitForSeconds(time);
        // Debug.Log("Wait Function ACTIVE");
        textObjectScript.virtualActivationFuntion(text, playerObject.transform.position);
    }

    IEnumerator transitionToScene(float time, string scene)
    {
        // Wait for an amount of time before displaying next dialogue
        yield return new WaitForSeconds(time);
        // Debug.Log("Wait Function ACTIVE");

        StartCoroutine(playerObject.TransitionToScene(scene, 5f, 2.5f));
    }
}
