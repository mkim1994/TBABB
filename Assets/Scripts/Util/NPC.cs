

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
    public bool isReadyToServe;
    public bool hasAcceptedDrink;
 
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

    public void ResetDrinkScore(){
        Services.GameManager.dialogue.variableStorage.SetValue("$drinkScore" + characterName, new Yarn.Value(-1));
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
            Debug.Log("Ordered flavor " + myFlavor + " " + fF);
            Debug.Log("ordered base " + myBase + " " + bF);   
            Debug.Log("Ordered mixer " + myMixer + " " + mF);
        }
    }

    public void PreemptiveOrder(){
        Debug.Log("preemptive ordering");
        Services.GameManager.dialogue.StartDialogue((Services.GameManager.dayManager.currentDay + 1) +"Order"+ characterName);
    }

    public bool IsCustomerPresent(){
        if (GetComponentInChildren<SpriteRenderer>().enabled)
        {
            return true;
        }
        return false;
    }

    public bool HasAcceptedDrink(){
        return hasAcceptedDrink;
    }

    public void InitiateDialogue(){
        /* if(Services.GameManager.dialogue.isDialogueRunning){
             Services.GameManager.dialogue.ResetDialogue();
         }*/
        /* if (!Services.GameManager.dialogue.isDialogueRunning)
          {*/
        Debug.Log("initiating dialogue");
        if (isReadyToTalk)
        {
            if (!Services.GameManager.audioController.bgm.isPlaying)
            {
                Services.GameManager.audioController.bgm.Play();
            }
            //need to add 1 to currentDay to offset the 0 start
            //Services.GameManager.dialogue.variableStorage.SetValue("$content" + characterName, defaultVar);
            Services.GameManager.dialogue.StartDialogue((Services.GameManager.dayManager.currentDay + 1) + characterName);
        }
       // }
    }

    public void SetCustomerVars(float score, float alcohol){
        Debug.Log("set customer vars(score = " + score + ", alcohol = " + alcohol);
        //float f1 = Services.GameManager.dialogue.variableStorage.GetValue("$drinkType" + characterName).AsNumber;
        var v1 = new Yarn.Value(score);
        var v2 = new Yarn.Value(alcohol);
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
        PreemptiveOrder();
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
        //transform.position = seatLocations[BestSeat()];
        GetComponent<BoxCollider>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponentInChildren<Light>().enabled = true;

    }

    public void LeavingBarAction(){
        insideBar = false;
        Services.GameManager.dayManager.currentCustomers.Remove(this);
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponentInChildren<Light>().enabled = false;
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
            Context.isReadyToServe = false;
            Context.isReadyToTalk = false;
            Context.hasAcceptedDrink = false;
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
            Debug.Log("enter bar");
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
            Debug.Log("take seat");

            Context.isReadyToTalk = true;
            Context.isReadyToServe = true;
            Context.TakeSeatAction();
        }

        public override void Update(){
            TransitionTo<Default>();
            return;
        }
    }
    //this is where it should track different $states and go to different states.
    private class Default : CustomerState
    {
        /*
         * $state+characterName:
         * -1 = reset/nothing
         * 0 = isReadyToServe
         * 1 = ready to talk after receiving a drink.
         * 2 = 
         * */
        public override void OnEnter()
        {
            Debug.Log("default");

        }
        public override void Update()
        {
            if (Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString != "-1")
            {
                if (Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString == "5")
                {
                    //customer is going to leave
                    Context.isReadyToTalk = false;
                    Services.GameManager.dialogue.variableStorage.SetValue("$state" + Context.characterName, new Yarn.Value(-1));
                    TransitionTo<LeavingBar>();
                    return;
                }
                else if (Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString == "0")
                {
                    //waiting for a drink
                   // Context.isReadyToTalk = false;
                    TransitionTo<WaitingForDrink>();
                    return;
                }
                else if(Services.GameManager.dialogue.variableStorage.GetValue("$state" +Context.characterName).AsString == "1")
                {

                    //customer is ready to talk, has accepted drink
                    TransitionTo<ReadyToTalk>();
                    return;
                }
            }
        }
    }

    private class ReadyToTalk : CustomerState
    {
        private string val;
        public override void OnEnter(){
            Debug.Log("ready to talk");
            Context.isReadyToTalk = true;
            Context.hasAcceptedDrink = true;
            val = Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString;
        }

        public override void Update()
        {
            if (Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString != val){
                TransitionTo<Default>();
                return;
            }
        }
    }



    private class WaitingForDrink : CustomerState{ 
        public override void OnEnter()
        {
            Debug.Log("waiting for drink");
            Context.isReadyToTalk = true; //can talk to clarify order
        }
        public override void Update(){
            if(Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName).AsString != "0"){
                Context.isReadyToTalk = true;
                TransitionTo<Default>();
                return;
            }
            /*
             * if coaster has received something
             * 
             * */
        }
    }

    /*
     * how to determine when the customer has stopped talking: 
     * when $state is 5 (set at the end of a conversation tree), reset it to -1
     * and take out "this" from currentCustomers in DayCycleManager
     **/

    private class Drinking : CustomerState{ // actually, state should change in here after time delay? so that it's talking??
        public override void OnEnter(){
            Debug.Log("drinking");
        }
        public override void Update(){
            if (Services.GameManager.dialogue.variableStorage.GetValue("$state" + Context.characterName) == new Yarn.Value(1))
            {
                TransitionTo<Default>();
                return;
            }
        }
    }



    private class DrinkServed : CustomerState{
        
    }

    private class LeavingBar : CustomerState{
        public override void OnEnter(){
            Debug.Log(Context.characterName+" LeavingBar");
        }
        public override void Update(){
            Context.LeavingBarAction();
            TransitionTo<OutsideBar>();
            return;
        }
    }
}

