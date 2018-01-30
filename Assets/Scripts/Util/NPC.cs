

using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using Yarn.Unity;
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


    // Use this for initialization
    void Start()
    {
        if (scriptToLoad != null)
        {
            FindObjectOfType<DialogueRunner>().AddScript(scriptToLoad);
        }

        customerData = GetComponent<CustomerData>();

    }

    // Update is called once per frame
    void Update()
    {

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
}

