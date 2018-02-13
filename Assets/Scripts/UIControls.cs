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
	[SerializeField]Text[] inLeftHandText;
	[SerializeField] private Text[] inRightHandText;
	[SerializeField]Text inHandControlsText;
	[SerializeField]string targetObj;
	[SerializeField]List<string> rayControlsStrings = new List<string>();
	[SerializeField]List<string> inHandControlsStrings = new List<string>();
	private Camera myCam;
	private GameObject leftHandObj;
	private GameObject rightHandObj;
	
	void Start(){
		myCam = FindObjectOfType<Camera>();
		
	}
	void Update(){
		UIRay();
		//find the objects in hand
		if (Services.GameManager.playerInput.pickupableInLeftHand != null &&
		    Services.GameManager.playerInput.pickupableInRightHand != null)
		{
			leftHandObj = Services.GameManager.playerInput.pickupableInLeftHand.gameObject;
			rightHandObj = Services.GameManager.playerInput.pickupableInRightHand.gameObject;
		}
		
		if(leftHandObj != null && rightHandObj != null){ //if both hands have object in them
			if(leftHandObj.GetComponent<Bottle>() != null && rightHandObj.GetComponent<Glass>() != null
			|| rightHandObj.GetComponent<Bottle>() != null  && leftHandObj.GetComponent<Glass>() != null
			)
			{
				inLeftHandText[0].text = "LMB";
				inLeftHandText[1].text = "pour into glass";
				inRightHandText[0].text = "RMB";
				inRightHandText[1].text = "pour into glass";
			} else {
				inLeftHandText[0].text = "";
				inLeftHandText[1].text = "";
				inRightHandText[0].text = "";
				inRightHandText[1].text = "";
				//other cases			
 			}
		}
		else
		{
			inLeftHandText[0].text = "";
			inLeftHandText[1].text = "";
			inRightHandText[0].text = "";
			inRightHandText[1].text = "";			
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
				leftHandControlsText[0].text = "Q";
				leftHandControlsText[1].text = "pick up\nwith left hand";
				rightHandControlsText[0].text = "E";
				rightHandControlsText[1].text = "pick up\nwith right hand";
			} else if (hitObj.GetComponent<Glass>() != null){
				centerText.text = "glass";
				leftHandControlsText[0].text = "Q";
				leftHandControlsText[1].text = "pick up\nwith left hand";
				rightHandControlsText[0].text = "E";
				rightHandControlsText[1].text = "pick up\nwith right hand";
			} else if (hitObj.GetComponent<NPC>() != null){
				if(!Services.GameManager.dialogue.isDialogueRunning){
					centerText.text = hitObj.GetComponent<NPC>().characterName;
				} else if (Services.GameManager.dialogue.isDialogueRunning){
					centerText.text = hitObj.GetComponent<NPC>().characterName;
				}
			}
			else {
				bottomCenterText.text = "";
				centerText.text = "";
				rightHandControlsText[0].text = "";
				rightHandControlsText[1].text = "";			
				leftHandControlsText[0].text = "";
				leftHandControlsText[1].text = "";		
			} 
		} else {
			bottomCenterText.text = "";	
			centerText.text = "";
			rightHandControlsText[0].text = "";
			rightHandControlsText[1].text = "";	
			leftHandControlsText[0].text = "";
			leftHandControlsText[1].text = "";		
		}
	}
}
