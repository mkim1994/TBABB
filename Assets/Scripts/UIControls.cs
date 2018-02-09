using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour {
	[SerializeField]LayerMask controlsMask;
	[SerializeField]Text centerText;
	[SerializeField]Text[] leftHandControlsText;
	[SerializeField]Text[] rightHandControlsText;
	[SerializeField]Text bottomCenterText;

	[SerializeField]Text inHandControlsText;
	[SerializeField]string targetObj;
	[SerializeField]List<string> rayControlsStrings = new List<string>();
	[SerializeField]List<string> inHandControlsStrings = new List<string>();
	private Camera myCam;

	void Start(){
		myCam = FindObjectOfType<Camera>();
		rayControlsStrings.Add("");
		rayControlsStrings.Add("Press Q to pick up " + targetObj + " with left hand \n Press E to pick up " + targetObj + " with right hand");
		rayControlsStrings.Add("Press E to drop object in right hand");
		rayControlsStrings.Add("Press Q to drop object in left hand");
		rayControlsStrings.Add("Press Q to serve drink");
		rayControlsStrings.Add("Press E to serve drink");
		inHandControlsStrings.Add("Hold LMB or RMB to pour drink");
	}
	void Update(){
		UIRay();
		if(Services.GameManager.playerInput.pickupableInLeftHand != null && Services.GameManager.playerInput.pickupableInRightHand != null){
			if(Services.GameManager.playerInput.pickupableInLeftHand.name.Contains("Bottle") && Services.GameManager.playerInput.pickupableInRightHand.name.Contains("Glass")
			|| Services.GameManager.playerInput.pickupableInRightHand.name.Contains("Bottle") && Services.GameManager.playerInput.pickupableInLeftHand.name.Contains("Glass")
			){
				// leftHandControlsText[0].text = "E";
				// leftHandControlsText[1].text = "pick up\nwith left hand";
				rightHandControlsText[2].text = "LMB";
				rightHandControlsText[3].text = "pour with left hand";	
			} else {
				rightHandControlsText[2].text = "";
				rightHandControlsText[3].text = "";
			}
		} else {
			rightHandControlsText[2].text = "";
			rightHandControlsText[2].text = "";
		}
	}
	private void UIRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, controlsMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			if(hitObj.GetComponent<Bottle>() != null){
				Bottle targetBottle = hitObj.GetComponent<Bottle>();
				if(targetBottle.myDrinkBase != DrinkBase.none){
					switch (targetBottle.myDrinkBase){
						case DrinkBase.beer:
							targetObj = "beer";
						break;
						case DrinkBase.brandy:
							targetObj = "brandy";
						break;
						case DrinkBase.gin:
							targetObj = "gin";
						break;
						case DrinkBase.rum:
							targetObj = "rum";
						break;
						case DrinkBase.tequila:
							targetObj = "tequila";
						break;
						case DrinkBase.vodka:
							targetObj = "vodka";
						break;
						case DrinkBase.whiskey:
							targetObj = "whiskey";
						break;
						case DrinkBase.wine:
							targetObj = "wine";
						break;
						default:
						break;
					} 
				} else if (targetBottle.myMixer != Mixer.none){
					switch (targetBottle.myMixer){
						case Mixer.tonic:
							targetObj = "tonic";
						break;
						case Mixer.apple_juice:
							targetObj = "apple juice";
						break;
						case Mixer.soda:
							targetObj = "soda";
						break;
						case Mixer.orange_juice:
							targetObj = "orange juice";
						break;
						case Mixer.lemon_juice:
							targetObj = "lemon juice";
						break;
						default:
						break;
					} 
				}
				centerText.text = targetObj;
			} else if (hitObj.GetComponent<Glass>() != null){
				centerText.text = "glass";
				rightHandControlsText[0].text = "E";
				rightHandControlsText[1].text = "pick up\nwith left hand";
			} else if (hitObj.GetComponent<NPC>() != null){
				if(!Services.GameManager.dialogue.isDialogueRunning){
					centerText.text = "Press SPACE to talk";
				} else if (Services.GameManager.dialogue.isDialogueRunning){
					centerText.text = "";
				}
			}
			else {
				centerText.text = "";	
				rightHandControlsText[0].text = "";
				rightHandControlsText[1].text = "";			
			} 
		} else {
			centerText.text = "";
			rightHandControlsText[0].text = "";
			rightHandControlsText[1].text = "";		
		}
	}
}
