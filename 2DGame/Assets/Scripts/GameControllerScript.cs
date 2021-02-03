using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class GameControllerScript : MonoBehaviour
{
    public GameObject canvas;
    public GameObject player;
    public GameObject backgroundMusic;
    public GameObject eventSystem;
    public GameObject MiniMap;

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

    GameObject vCam;
    CinemachineVirtualCamera vCamObject;

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

        //Keep the objects regardless of scene change
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(backgroundMusic);
        DontDestroyOnLoad(eventSystem);
        DontDestroyOnLoad(MiniMap);
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
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;

                playerObject.transform.localScale = new Vector3(1.34f, 1.34f, 1);
                playerObject.speed = playerOriginalSpeed * 1.34f;

                playerObject.lookDirection = new Vector2(1, 0);
                playerRigidBody2D.position = new Vector2(-6, -2);
                playerObject.inTransition = false;
                break;

            case "Forest":
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;

                //playerObject.transform.localScale = new Vector3(1.34f, 1.34f, 1);
                playerObject.lockedMovement = false;
                playerObject.speed = playerOriginalSpeed * 1.34f;
                playerObject.transform.localScale = new Vector3(1.0f, 1.0f, 1);

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                playerObject.lookDirection = new Vector2(1, 0);
                playerRigidBody2D.position = new Vector2(-8.33f, -1.69f);
                playerObject.inTransition = false;

                DemonKing = GameObject.Find("DemonKing");
                DemonKingObject = DemonKing.GetComponent<DemonKingScript>();
                DemonKingObject.playerObject = player;
                DemonKingObject.textObject = canvas.transform.GetChild(1).gameObject;
                DemonKingObject.sceneLoaded = "Forest";
                break;

            case "PrincessChamber":
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;

                playerObject.transform.localScale = new Vector3(2f, 2f, 1);
                playerObject.speed = playerOriginalSpeed * 2f;

                textObjectScript.virtualActivationFuntion("Chamber", playerObject.transform.position);
                //playerObject.textState = true;

                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.position = new Vector2(-7, 0);
                playerObject.inTransition = false;

                DemonKing = GameObject.Find("DemonKing");
                DemonKingObject = DemonKing.GetComponent<DemonKingScript>();
                DemonKingObject.sceneLoaded = "PrincessChamber";
                break;

            case "FarmHut":
                Debug.Log("Running FarmHut case");
                Debug.Log(canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                playerObject.lockedMovement = false;
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;

                playerObject.transform.localScale = new Vector3(1.34f, 1.34f, 1);
                playerObject.speed = playerOriginalSpeed * 1.34f;

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.position = new Vector2(-3.7f, 0.6f);

                //Change letter name when boar food is done when entering the farm hut second time
                if (canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "PLAYER: There you go, that's should last for a month or two...")
                {
                    playerRigidBody2D.position = new Vector2(4.318831f, -4.872068f);
                    GameObject.Find("CabinetWithLetter").name = "CabinetWithLetter2";
                }

                //Change letter name after day two
                if (canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "I’ll leave first thing tomorrow. ")
                {
                    GameObject.Find("CabinetWithLetter").name = "CabinetWithLetterDone";
                }
                playerObject.inTransition = false;
                break;

            case "Farm":
                playerObject.lockedMovement = false;
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;

                playerObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                playerObject.speed = playerOriginalSpeed;

                playerAnimator.runtimeAnimatorController = Resources.Load("Art/Animation/Controller/Protagonist") as RuntimeAnimatorController;
                playerSpriteRenderer.sprite = Resources.Load("Art/Animation/Sprites/ProtagSpriteSheet1stTo3rd") as Sprite;
                playerBoxCollider.offset = new Vector2(0.015f, 0.27f);
                playerBoxCollider.size = new Vector2(0.53f, 0.4f);

                playerObject.lookDirection = new Vector2(0, -1);
                playerRigidBody2D.position = new Vector2(-11.4f, -7f);
                playerObject.inTransition = false;

                GameObject.Find("carriage").name = "carriageNotNextDayYet";
                FatSeller = GameObject.Find("FatMerchant");
                FatSeller.SetActive(false);

                //Change market and carriage name when day two
                if (canvas.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "I’ll leave first thing tomorrow. ")
                {
                    Debug.Log("MarketChanged");
                    GameObject.Find("MarketStandRed").name = "MarketStandRed2";
                    GameObject.Find("carriageNotNextDayYet").name = "carriage";
                    GameObject.Find("MarketMerchant").SetActive(false);
                    FatSeller.SetActive(true);
                }
                break;

	    case "PushingStonePuzzle":
                playerObject.lockedMovement = false;
                vCam = GameObject.Find("CM vcam1");
                vCamObject = vCam.GetComponent<CinemachineVirtualCamera>();
                vCamObject.m_Follow = playerObject.transform;
                playerObject.lookDirection = new Vector2(0, 1);
                playerRigidBody2D.position = new Vector2(0f, -5f);
                playerObject.inTransition = false;
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
