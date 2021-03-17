using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;
using TMPro;

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
    public GameObject MiniMap;
    public GameObject Compass;

    GameObject MarketMerchant;
    GameObject FatSeller;

    PlayerController playerObject;
    SpriteRenderer playerSpriteRenderer;
    Rigidbody2D playerRigidBody2D;
    Animator playerAnimator;
    BoxCollider2D playerBoxCollider;

    GameObject DemonKing;
    DemonKingScript DemonKingObject;

    TextScript textObjectScript;
    string textObjectText;

    GameObject vCam;
    CinemachineVirtualCamera vCamObject;

    Dictionary<string, SceneVar> sceneTransitionVariables;
    SceneVar currentSceneVar;

    float playerOriginalSpeed;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerObject = player.GetComponent<PlayerController>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerRigidBody2D = player.GetComponent<Rigidbody2D>();
        playerAnimator = player.GetComponent<Animator>();
        playerBoxCollider = player.GetComponent<BoxCollider2D>();

        playerOriginalSpeed = playerObject.speed;

        textObjectScript = canvas.transform.GetChild(1).GetComponent<TextScript>();
        textObjectText = canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

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
                Position = new Vector2(-8.3f, -1.7f)
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
                Position = new Vector2(7.1f, 2.9f)
                }
            },
            { "KeyPuzzle", new SceneVar {
                Scale = new Vector3(0.8f, 0.8f, 1),
                Speed = playerOriginalSpeed,
                Direction = new Vector2(0, -1),
                Position = new Vector2(10f, 10f)
                }
            }
        };

        //Keep the objects regardless of scene change
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(backgroundMusic);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(MiniMap);
        DontDestroyOnLoad(footSteps);
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

        switch (scene.name)
        {
            case "PrincessChamber":
                textObjectScript.virtualActivationFuntion("Chamber", playerObject.transform.position);
                //playerObject.textState = true;

                DemonKing = GameObject.Find("DemonKing");
                DemonKingObject = DemonKing.GetComponent<DemonKingScript>();
                DemonKingObject.sceneLoaded = "PrincessChamber";
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
                if (textObjectText == "PLAYER: There you go, that's should last for a month or two...")
                {
                    GameObject.Find("CabinetWithLetter").name = "CabinetWithLetter2";
                }

                //Change letter name after day two
                if (textObjectText == "I’ll leave first thing tomorrow. ")
                {
                    GameObject.Find("CabinetWithLetter").name = "CabinetWithLetterDone";
                }

                //If it is not day two, change music
                if (textObjectText != "I’ll leave first thing tomorrow. ")
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

                GameObject.Find("carriage").name = "carriageNotNextDayYet";
                FatSeller = GameObject.Find("FatMerchant");
                FatSeller.SetActive(false);

                //Change market and carriage name when day two
                if (textObjectText == "I’ll leave first thing tomorrow. ")
                {
                    Debug.Log("MarketChanged");
                    GameObject.Find("MarketStandRed").name = "MarketStandRed2";
                    GameObject.Find("carriageNotNextDayYet").name = "carriage";
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
                playerObject.LightObject.SetActive(false);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);
                break;

            case "CropsPuzzleHouse":
                currentSceneVar.Direction = new Vector2(0, -1);
                currentSceneVar.Position = new Vector2(3.7f, 0.6f);

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

                backgroundMusic.GetComponent<AudioSource>().clip = Resources.Load("Audio/2DGameTestLoopAudio") as AudioClip;
                backgroundMusic.GetComponent<AudioSource>().Play();
                break;

            case "WaterBucketPrototype":
                currentSceneVar.Direction = new Vector2(0, -1);
                currentSceneVar.Position = new Vector2(-3.7f, 0.6f);

                playerObject.LightObject.SetActive(false);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);
                break;
        }
    }

    IEnumerator WaitFuntion(float time, string text)
    {
        // Wait for an amount of time before displaying next dialogue
        yield return new WaitForSeconds(time);
        textObjectScript.virtualActivationFuntion(text, playerObject.transform.position);
    }
}
