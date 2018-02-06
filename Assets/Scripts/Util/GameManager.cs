using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour {

	public GameObject sceneRoot;
    public Camera currentCamera;


    public GameObject player;
	public PlayerInput playerInput;
    public DialogueRunner dialogue;
    public NPC CustomerIvory, CustomerSahana;
    public DayCycleManager dayManager;

    public Transform entrance;
    public GameObject directionalLight;
    public GameObject entranceLight;

    //public GameObject TestingScenes;
	void Awake()
	{
       // TestingScenes.SetActive(false);
		InitializeServices();
	}

	// Use this for initialization
	void Start()
	{
		//Services.EventManager.Register<Reset>(Reset);
	}

	// Update is called once per frame
	void Update()
	{

        if(Input.GetKeyUp(KeyCode.Escape)){
            Application.Quit();
        }

        if(Input.GetKeyUp(KeyCode.R)){
           // Services.SceneStackManager.Swap<TitleScreen>();
        }
	}

	void InitializeServices()
	{
		Services.GameManager = this;
		Services.EventManager = new EventManager();
		Services.Prefabs = Resources.Load<PrefabDB>("Prefabs/Prefabs");
		Services.TweenManager = FindObjectOfType<TweenManager>();
		Services.DrinkDictionary = new DrinkDictionary(); 
		Services.MixerDictionary = new MixerDictionary();
        //Services.Materials = Resources.Load<MaterialDB>("Art/Materials");
		//Services.SceneStackManager = new SceneStackManager<TransitionData>(sceneRoot, Services.Prefabs.Scenes);


	}

	void Reset(Reset e)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    //UI buttons

}
