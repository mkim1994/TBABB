

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
    public Coaster myCoaster;


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

    public bool isReadyToTalk;


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

    [YarnCommand("order")]
    public void OrderDrink(string flavor, string f, string mixer, string m, string drinkbase, string b){
        Flavor myFlavor = Flavor.none;
        Mixer myMixer = Mixer.none;
        DrinkBase myBase = DrinkBase.none;
        float fF = 0f; float mF = 0f; float bF = 0f;
        float.TryParse(f, out fF);
        float.TryParse(m, out mF);
        float.TryParse(b, out bF);
        switch (flavor)
        {
            case "bitter":
                myFlavor = Flavor.bitter;
                break;
            case "sweet":
                myFlavor = Flavor.sweet;
                break;
            case "sour":
                myFlavor = Flavor.sour;
                break;
            case "spicy":
                myFlavor = Flavor.spicy;
                break;
            case "smoky":
                myFlavor = Flavor.smoky;
                break;
        }
        switch (drinkbase)
        {
            case "whiskey":
                myBase = DrinkBase.whiskey;
                break;
            case "tequila":
                myBase = DrinkBase.tequila;
                break;
            case "gin":
                myBase = DrinkBase.gin;
                break;
            case "vodka":
                myBase = DrinkBase.vodka;
                break;
            case "beer":
                myBase = DrinkBase.beer;
                break;
            case "rum":
                myBase = DrinkBase.rum;
                break;
            case "wine":
                myBase = DrinkBase.wine;
                break;
        }
        switch (mixer)
        {
            case "applejuice":
                myMixer = Mixer.apple_juice;
                break;
            case "orangejuice":
                myMixer = Mixer.orange_juice;
                break;
            case "tonic":
                myMixer = Mixer.tonic;
                break;
            case "lemonjuice":
                myMixer = Mixer.lemon_juice;
                break;
            case "soda":
                myMixer = Mixer.soda;
                break;
        }
        if (myCoaster != null)
        {
            myCoaster.TakeOrder(DrinkProfile.OrderDrink(myFlavor, fF, myBase, bF, myMixer, mF));
        }
    }

    public void InitiateDialogue(){
        if (!Services.GameManager.dialogue.isDialogueRunning)
        {
            if (isReadyToTalk)
            {   
                if(!Services.GameManager.audioController.bgm.isPlaying){
                    Services.GameManager.audioController.bgm.Play();
                }
                //need to add 1 to currentDay to offset the 0 start
                //Services.GameManager.dialogue.variableStorage.SetValue("$content" + characterName, defaultVar);
                Services.GameManager.dialogue.StartDialogue((Services.GameManager.dayManager.currentDay + 1) + characterName);
            }
        }
    }

    public void SetCustomerVars(float score, float alcohol){
        //float f1 = Services.GameManager.dialogue.variableStorage.GetValue("$drinkType" + characterName).AsNumber;
        var v1 = new Yarn.Value(score);
        var v2 = new Yarn.Value(alcohol);
        Debug.Log(score);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkScore"+characterName,v1);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkAlcohol" + characterName, v2);
    } 

    public void ResetCustomerVars(){
        var defaultVar = new Yarn.Value(-1f);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkScore" + characterName, defaultVar);
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkAlcohol" + characterName, defaultVar);

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
        Services.GameManager.audioController.dooropen.Play();
        Services.GameManager.audioController.doorbell.Play();
        silhouette.gameObject.SetActive(true);
        Services.GameManager.directionalLight.SetActive(true);
        Services.GameManager.entranceLight.SetActive(true);
        Sequence enterThroughDoor = DOTween.Sequence();
        //open door
        enterThroughDoor.Append(Services.GameManager.entrance.DORotate(new Vector3(0, -125f,0), 2f));
        enterThroughDoor.Append(silhouette.transform.DOMoveZ(silhouetteLocation.z - 3f, 1f));
        enterThroughDoor.Append(Services.GameManager.entrance.DOScale(new Vector3(1.2f, 1f, 1f), 1f));

        //close door
        enterThroughDoor.AppendCallback(()=>Services.GameManager.audioController.doorclose.Play());
        enterThroughDoor.Append(Services.GameManager.entrance.DORotate(new Vector3(0, 0, 0), 1f));
        enterThroughDoor.AppendCallback(()=>Services.GameManager.directionalLight.SetActive(false));
        enterThroughDoor.AppendCallback(() => Services.GameManager.entranceLight.SetActive(false));
        enterThroughDoor.Append(Services.GameManager.entrance.DOScale(new Vector3(1.2f, 1f, 1f), 5f)); //time from door to bar
        enterThroughDoor.OnComplete(() => enterBarAnimFinished = true);
    }

    public void TakeSeatAction(){
        Services.GameManager.audioController.spotlightsfx.Play();
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

        public override void Update(){
            TransitionTo<ReadyToTalk>();
            return;
        }
    }

    private class ReadyToTalk : CustomerState
    {
        public override void OnEnter(){
            Context.isReadyToTalk = true;
        }

        public override void Update()
        {
            if(Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName) == new Yarn.Value(0)){
                TransitionTo<WaitingForDrink>();
                return;
            }
        }
    }

    private class WaitingForDrink : CustomerState{ // unable to talk at this point
        //might want to change it so the customer repeats the order if asked
        public override void OnEnter()
        {
            Context.isReadyToTalk = false;
        }
        public override void Update(){
            /*
             * if coaster has received something
             * 
             * */
        }
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

