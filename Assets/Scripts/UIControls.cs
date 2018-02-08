using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour {
	[SerializeField]LayerMask controlsMask;
	[SerializeField]Text rayControlsText;
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
				inHandControlsText.text = inHandControlsStrings[0];	
			} else {
				inHandControlsText.text = "";
			}
		} else {
			inHandControlsText.text = "";
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
				rayControlsText.text = "Press Q to pick up " + targetObj + " with left hand \n Press E to pick up " + targetObj + " with right hand";
			} else if (hitObj.GetComponent<Glass>() != null){
				rayControlsText.text = "Press Q to pick up glass with left hand \n Press E to pick up glass with right hand";
			} else if (hitObj.GetComponent<NPC>() != null){
				if(hitObj.GetComponent<NPC>().isReadyToTalk){
					rayControlsText.text = "Press SPACE to talk";
				} else if (!hitObj.GetComponent<NPC>().isReadyToTalk){
					rayControlsText.text = "";
				}
			}
			else {
				rayControlsText.text = rayControlsStrings[0];				
			} 
		} else {
			rayControlsText.text = rayControlsStrings[0];
		}
	}
}
