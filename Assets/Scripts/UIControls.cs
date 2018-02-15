using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class UIControls : MonoBehaviour {
	
	[SerializeField]LayerMask controlsMask;
	
	//images
	[SerializeField] private Image leftHandActionImage;
	[SerializeField] private Image rightHandActionImage;
	[SerializeField] private Image leftHandPickUpImage;
	[SerializeField] private Image rightHandPickUpImage;
	[SerializeField] private GameObject botCenterImg;

	//text 
	[SerializeField]Text centerText;
	[SerializeField]Text[] leftHandControlsText;
	[SerializeField]Text[] rightHandControlsText;
	[SerializeField]Text bottomCenterText;
	[SerializeField]Text[] inLeftHandText;
	[SerializeField] private Text[] inRightHandText;
 	[SerializeField]string targetObj;
	private Camera myCam;
	private GameObject leftHandObj;
	private GameObject rightHandObj;

	private IEnumerator clearTextCoroutine;
//	private bool isUsingKeyboard = true;
	[SerializeField]private bool isMessageOverrideOn = false;
	
	void Start(){
//		if ( /*some Rewired code here*/)
//		{
//			isUsingKeyboard = true;
//		}
//		else
//		{
//			isUsingKeyboard = false;
//		}
		clearTextCoroutine = ClearTextCoroutine(centerText, 3);
		botCenterImg.SetActive(false);
		myCam = FindObjectOfType<Camera>();
	}
	void Update(){

		UIRay();
		//find the objects in hand
 
		if (Services.GameManager.playerInput.pickupableInLeftHand != null)
		{
			leftHandObj = GetComponent<PlayerInput>().pickupableInLeftHand.gameObject;
		}
		else
		{
			leftHandObj = null;
		}

		if (Services.GameManager.playerInput.pickupableInRightHand != null)
		{
			rightHandObj = GetComponent<PlayerInput>().pickupableInRightHand.gameObject;
		}
		else
		{
			rightHandObj = null;
		}


		if(leftHandObj != null && rightHandObj != null){ //if both hands have object in them
			//case 1: if bottle in one, glass in the other
			if(leftHandObj.GetComponent<Bottle>() != null && rightHandObj.GetComponent<Glass>() != null
			|| rightHandObj.GetComponent<Bottle>() != null  && leftHandObj.GetComponent<Glass>() != null
			)
			{
//				inLeftHandText[0].text = "LMB";
				leftHandActionImage.enabled = true;
				inLeftHandText[1].text = "pour";
//				inRightHandText[0].text = "RMB";
				rightHandActionImage.enabled = true;
				inRightHandText[1].text = "pour";
			}
			//case 2: bottle in both hands
			else if (leftHandObj.GetComponent<Bottle>() != null && rightHandObj.GetComponent<Bottle>() != null)
			{
				if (Services.GameManager.playerInput.pickupable != null)
				{
					//looking at glass
					if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
					{
//						inLeftHandText[0].text = "LMB";
						leftHandActionImage.enabled = true;
						inLeftHandText[1].text = "pour";
//						inRightHandText[0].text = "RMB";
						rightHandActionImage.enabled = true;
						inRightHandText[1].text = "pour";
					}	
				}
				else
				{
					rightHandActionImage.enabled = false;
					leftHandActionImage.enabled = false;
					ClearTextArray(inLeftHandText);
				}
			}
			//case 3: glass in both hands
			else if (leftHandObj.GetComponent<Bottle>() != null && rightHandObj.GetComponent<Bottle>() != null)
			{
				ClearTextArray(inLeftHandText);
				leftHandActionImage.enabled = false;
				ClearTextArray(inRightHandText);
				rightHandActionImage.enabled = false;
			}
		}
//		left hand not empty; right hand empty
		else if(leftHandObj != null && rightHandObj == null)
		{	
//			inRightHandText[0].text = "";
//			inRightHandText[1].text = "";
			ClearTextArray(inRightHandText);
			rightHandActionImage.enabled = false;
			if (Services.GameManager.playerInput.pickupable != null)
			{
				//looking at glass, holding bottle
				if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
				{
					if(leftHandObj.GetComponent<Bottle>() != null){
						leftHandActionImage.enabled = true;
						inLeftHandText[1].text = "pour";			
					}
				}
			}
			else
			{
				ClearTextArray(inLeftHandText);
				leftHandActionImage.enabled = false;
				ClearTextArray(inRightHandText);
				rightHandActionImage.enabled = false;
			}
		}
//		left hand empty, right hand not empty
		else if (leftHandObj == null && rightHandObj != null)
		{			
//			inLeftHandText[0].text = "";
//			inLeftHandText[1].text = "";
			ClearTextArray(inLeftHandText);
			leftHandActionImage.enabled = false;
			if (Services.GameManager.playerInput.pickupable != null)
			{
				//looking at glass, holding bottle
				if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
				{
					if(rightHandObj.GetComponent<Bottle>() != null){
						rightHandActionImage.enabled = true;
						inRightHandText[1].text = "pour";			
					}
				}
			}
			else
			{
				leftHandActionImage.enabled = false;
				ClearTextArray(inLeftHandText);
				rightHandActionImage.enabled = false;
				ClearTextArray(inRightHandText);
			}
		}
		else if (leftHandObj == null && rightHandObj == null)
		{
			leftHandActionImage.enabled = false;
			ClearTextArray(inLeftHandText);
			rightHandActionImage.enabled = false;
			ClearTextArray(inRightHandText);
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

				if (leftHandObj == null)
				{
					leftHandPickUpImage.enabled = true;
					leftHandControlsText[0].text = "Q";
					leftHandControlsText[1].text = "pick up";
				}
				else if (leftHandObj != null)
				{
					leftHandPickUpImage.enabled = true;
					leftHandControlsText[0].text = "Q";
					leftHandControlsText[1].text = "swap";
				}
				if (rightHandObj == null)
				{
					rightHandPickUpImage.enabled = true;
					rightHandControlsText[0].text = "E";
					rightHandControlsText[1].text = "pick up";
				}
				else
				{
					rightHandPickUpImage.enabled = true;
					rightHandControlsText[0].text = "E";
					rightHandControlsText[1].text = "swap";
				}
			} else if (hitObj.GetComponent<Glass>() != null){
				centerText.text = "glass";
				if (leftHandObj == null)
				{
					leftHandPickUpImage.enabled = true;
					leftHandControlsText[0].text = "Q";
					leftHandControlsText[1].text = "pick up";
				} 
				else if (leftHandObj != null)
				{
					leftHandPickUpImage.enabled = true;
					leftHandControlsText[0].text = "Q";
					leftHandControlsText[1].text = "swap";
				}

				if (rightHandObj == null)
				{
					rightHandPickUpImage.enabled = true;
					rightHandControlsText[0].text = "E";
					rightHandControlsText[1].text = "pick up";
				}
				else
				{
					rightHandPickUpImage.enabled = true;
					rightHandControlsText[0].text = "E";
					rightHandControlsText[1].text = "swap";
				}
			} else if (hitObj.GetComponent<NPC>() != null)
			{
				if (!Services.GameManager.dialogue.isDialogueRunning)
				{
					//Customer is not talking
					centerText.text = hitObj.GetComponent<NPC>().characterName;
					botCenterImg.SetActive(true);
					bottomCenterText.text = "SPACE";
				}
				else if (Services.GameManager.dialogue.isDialogueRunning)
				{
					//customer is talking
					centerText.text = hitObj.GetComponent<NPC>().characterName;
					botCenterImg.SetActive(false);
					bottomCenterText.text = "";
				}
			} else if (hitObj.GetComponent<Dropzone>() != null)
			{
				leftHandPickUpImage.enabled = true;
				leftHandControlsText[0].text = "Q";
				leftHandControlsText[1].text = "put back";
				rightHandPickUpImage.enabled = true;
				rightHandControlsText[0].text = "E";
				rightHandControlsText[1].text = "put back";
			} 
			else if (hitObj.GetComponent<LightSwitch>() != null)
			{
				if (Services.GameManager.dayManager.dayHasEnded)
				{
					centerText.text = "end the day";
				}
				else if (!Services.GameManager.dayManager.dayHasEnded)
				{
					if (Services.GameManager.playerInput.i_talk)
					{
						if (!isMessageOverrideOn)
						{
							isMessageOverrideOn = true;
							centerText.text = "there are still customers to serve";
							StartCoroutine(clearTextCoroutine);
						}					
					} else if (!Services.GameManager.playerInput.i_talk && !isMessageOverrideOn)
					{
						centerText.text = "end the day";
					}


				} 
			}
			else {
 				StopCoroutine(clearTextCoroutine);
				isMessageOverrideOn = false;
				botCenterImg.SetActive(false);
				bottomCenterText.text = "";
				centerText.text = "";
				rightHandPickUpImage.enabled = false;
				rightHandControlsText[0].text = "";
				rightHandControlsText[1].text = "";			
				leftHandPickUpImage.enabled = false;
				leftHandControlsText[0].text = "";
				leftHandControlsText[1].text = "";		
			} 
		} else {
			botCenterImg.SetActive(false);
			bottomCenterText.text = "";	
			centerText.text = "";
			rightHandPickUpImage.enabled = false;
			rightHandControlsText[0].text = "";
			rightHandControlsText[1].text = "";	
			leftHandPickUpImage.enabled = false;
			leftHandControlsText[0].text = "";
			leftHandControlsText[1].text = "";		
		}
	}

	void ClearTextArray(Text[] texts)
	{
		for (int i = 0; i < texts.Length; i++)
		{
			texts[i].text = "";
		}
	}

	IEnumerator ClearTextCoroutine(Text text, float delay)
	{
		yield return new WaitForSeconds(delay);
		text.text = "";
		Debug.Log("what cororuite");
		isMessageOverrideOn = false;
	}

}
