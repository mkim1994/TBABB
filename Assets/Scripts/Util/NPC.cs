

using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using Yarn.Unity;
using DG.Tweening;
/// attached to the non-player characters, and stores the name of the
/// Yarn node that should be run when you talk to them.
public class NPC : MonoBehaviour
{
    private CustomerData customerData;
    public string characterName = "";

    [FormerlySerializedAs("startNode")]
    public string talkToNode = "";

    [Header("Optional")]
    public TextAsset scriptToLoad;

    public FSM<NPC> fsm;
    // Use this for initialization

    public bool insideBar;

    public Vector3[] seatLocations;

    public bool enterBarAnimFinished;
    public SpriteRenderer silhouette;
    private Vector3 silhouetteLocation;

    void Start()
    {
        DOTween.Init();
        insideBar = false;
        if (scriptToLoad != null)
        {
            FindObjectOfType<DialogueRunner>().AddScript(scriptToLoad);
        }
        enterBarAnimFinished = false;

        customerData = GetComponent<CustomerData>();
        fsm = new FSM<NPC>(this);
        fsm.TransitionTo<OutsideBar>();
        silhouetteLocation = silhouette.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    public void InitiateDialogue(){
        if (!Services.GameManager.dialogue.isDialogueRunning)
        {
            //need to add 1 to currentDay to offset the 0 start
            Services.GameManager.dialogue.StartDialogue(characterName + (Services.GameManager.dayManager.currentDay + 1));
        }
    }

    public void SetCustomerVars(float type, float flavor, float alcohol, float drinkable){
        //float f1 = Services.GameManager.dialogue.variableStorage.GetValue("$drinkType" + characterName).AsNumber;
        var v1 = new Yarn.Value(type);
        var v2 = new Yarn.Value(flavor);
        var v3 = new Yarn.Value(alcohol);
        var v4 = new Yarn.Value(drinkable);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkType" + characterName, v1);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkFlavor" + characterName, v2);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkAlcohol" + characterName, v3);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkable" + characterName, v4);
        
    } 

    public void ResetCustomerVars(){
        var defaultVar = new Yarn.Value(-1f);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkType" + characterName, defaultVar);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkFlavor" + characterName, defaultVar);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkAlcohol" + characterName, defaultVar);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkable" + characterName, defaultVar);

        Services.GameManager.dialogue.variableStorage.SetValue("$content" + characterName, defaultVar);
        Services.GameManager.dialogue.variableStorage.SetValue("$drunk" + characterName, defaultVar);
    }

    public void OutsideBarAction(){
        silhouette.gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponentInChildren<Light>().enabled = false;
    }

    public void EnterBarAction(){

        silhouette.gameObject.SetActive(true);
        Services.GameManager.directionalLight.SetActive(true);
        Services.GameManager.entranceLight.SetActive(true);
        Sequence enterThroughDoor = DOTween.Sequence();
        enterThroughDoor.Append(Services.GameManager.entrance.DORotate(new Vector3(0, -125f,0), 2f));
        enterThroughDoor.Append(silhouette.transform.DOMoveZ(silhouetteLocation.z - 3f, 1f));
        enterThroughDoor.Append(Services.GameManager.entrance.DOScale(new Vector3(1.2f, 1f, 1f), 1f));
        enterThroughDoor.Append(Services.GameManager.entrance.DORotate(new Vector3(0, 0, 0), 1f));
        enterThroughDoor.AppendCallback(()=>Services.GameManager.directionalLight.SetActive(false));
        enterThroughDoor.AppendCallback(() => Services.GameManager.entranceLight.SetActive(false));
        enterThroughDoor.Append(Services.GameManager.entrance.DOScale(new Vector3(1.2f, 1f, 1f), 5f)); //time from door to bar
        enterThroughDoor.OnComplete(() => enterBarAnimFinished = true);
    }

    public void TakeSeatAction(){
        Debug.Log(characterName +" takes seat");
        silhouette.transform.position = silhouetteLocation;
        silhouette.gameObject.SetActive(false);
        transform.position = seatLocations[BestSeat()];
        GetComponent<BoxCollider>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponentInChildren<Light>().enabled = true;

    }

    int BestSeat(){
        int minIndex = 0;
        for (int i = 0; i < customerData.rankedseats.Length; ++i){
            if(customerData.rankedseats[i] < customerData.rankedseats[minIndex]){
                minIndex = i;
            }
        }
        return minIndex;
    }

    private class CustomerState : FSM<NPC>.State{}

    private class OutsideBar : CustomerState{ //not in the bar
        public override void OnEnter(){
            Context.OutsideBarAction(); //disable components
        }
        public override void Update(){
            if(Context.insideBar){
                TransitionTo<EnterBar>();
                return;
            }
        }
    }

    private class EnterBar : CustomerState{
        //door anim
        public override void OnEnter(){
            //on animation complete, <takeseat>
            Debug.Log("onenter enter bar");
            Context.EnterBarAction();
        }
        public override void Update(){
            if (Context.enterBarAnimFinished)
            {
                Context.enterBarAnimFinished = false;
                TransitionTo<TakeSeat>();
                return;
            }
        }
    }

    private class TakeSeat : CustomerState{
        //choose seat, re-enable sprites and scripts
        public override void OnEnter(){

            Context.TakeSeatAction();
        }
    }

    private class ReadyToTalk : CustomerState
    {

    }

    private class Waiting : CustomerState{ //not drinking, just waiting
        
    }
    private class Drinking : CustomerState{
        
    }

    private class TalkingWithoutDrink : CustomerState{
        
    }

    private class TalkingWithDrink : CustomerState{
        
    }

    private class DrinkServed : CustomerState{
        
    }

    private class LeavingBar : CustomerState{
        
    }
}

