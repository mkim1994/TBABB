using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
 

public class UIControls : MonoBehaviour {
	[SerializeField]private bool isLookingAtEmptyDropzone = false;
	private GameObject dropzoneBeingLookedAtGO;
	[SerializeField]private GameObject pickupableBeingLookedAt;
	private Dropzone dropzoneBeingLookedAt;
	
	[SerializeField]LayerMask controlsMask;
	[SerializeField] private LayerMask dropzoneOnlyMask;
	private Crosshair _crosshair;
	
	//images
	[SerializeField] private Image leftHandActionImage;
	[SerializeField] private Image rightHandActionImage;
	[SerializeField] private Image leftHandPickUpImage;
	[SerializeField] private Image rightHandPickUpImage;
	[SerializeField] private GameObject botCenterImg;
	
	//bools
	[SerializeField] private bool isLookingAtNPC;

	//text 
	[SerializeField]Text centerText;
	[SerializeField]Text[] leftHandControlsText;
	[SerializeField]Text[] rightHandControlsText;
	[SerializeField]Text bottomCenterText;
	[SerializeField] private Text bottomCenterInsText;
	[SerializeField]Text[] inLeftHandText;
	[SerializeField] private Text[] inRightHandText;
 	[SerializeField]string targetObj;
	[SerializeField] private Canvas myCanvas;
	private Camera myCam;
	[SerializeField]private GameObject leftHandObj;
	[SerializeField]private GameObject rightHandObj;
	[SerializeField]private bool isExceptionTextRequired = false;
	private int stringOffset;
	private PlayerInput player;
	private string[] buttonAndKeyStrings =
	{
		"",
		"",
		"Q",
		"E",
		"SPACE",

		"L1",
		"R1",
		"L2",
		"R2",
		""
	};

	private IEnumerator clearTextCoroutine;
//	private bool isUsingKeyboard = true;
	public bool isMessageOverrideOn = false;
	
	void Start()
	{
		_crosshair = FindObjectOfType<Crosshair>();
		player = Services.GameManager.playerInput;
		clearTextCoroutine = ClearTextCoroutine(centerText, 3);
		botCenterImg.SetActive(false);
		myCam = FindObjectOfType<Camera>();
	}
	
	void Update(){

		if (!isLookingAtNPC)
		{
			DropzoneOnlyRay();		
		}
		if (!isLookingAtEmptyDropzone)
		{
			UIRay();		
		}
		
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
				if(Services.GameManager.playerInput.sink == null && Services.GameManager.playerInput.iceMaker == null && player.pickupable == null){
					inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					leftHandActionImage.enabled = true;
					inLeftHandText[1].text = "pour";
					inRightHandText[0].text = buttonAndKeyStrings[1 + stringOffset];
					rightHandActionImage.enabled = true;
					inRightHandText[1].text = "pour";
				} else if (player.sink != null){
					if(leftHandObj.GetComponent<Glass>() != null){
						leftHandActionImage.enabled = true;
						inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
						inLeftHandText[1].text = "empty glass";
					} 
					if(rightHandObj.GetComponent<Glass>() != null){
						rightHandActionImage.enabled = true;
						inRightHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
						inRightHandText[1].text = "empty glass";
					}
				} 
			}
			//case 2: bottle in both hands
			else if (leftHandObj.GetComponent<Bottle>() != null && rightHandObj.GetComponent<Bottle>() != null)
			{
				if (Services.GameManager.playerInput.pickupable != null)
				{
					//looking at glass
					if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
					{
						inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
						leftHandActionImage.enabled = true;
						inLeftHandText[1].text = "pour";
						inRightHandText[0].text = buttonAndKeyStrings[1 + stringOffset];
						rightHandActionImage.enabled = true;
						inRightHandText[1].text = "pour";
					}	
				}
				else
				{
					rightHandActionImage.enabled = false;
					leftHandActionImage.enabled = false;
					ClearTextArray(inLeftHandText);
					ClearTextArray(inRightHandText);
				}
			}
			//case 3: glass in both hands
			else if (leftHandObj.GetComponent<Glass>() != null && rightHandObj.GetComponent<Glass>() != null)
			{
				if(player.sink != null){
					leftHandActionImage.enabled = true;
					inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inLeftHandText[1].text = "empty glass";

					rightHandActionImage.enabled = true;
					inRightHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inRightHandText[1].text = "empty glass";
				}
				else {
					ClearTextArray(inLeftHandText);
					leftHandActionImage.enabled = false;
					ClearTextArray(inRightHandText);
					rightHandActionImage.enabled = false;
				}
			}
		}
//		left hand not empty; right hand empty
		else if(leftHandObj != null && rightHandObj == null)
		{	
			inRightHandText[0].text = "";
			inRightHandText[1].text = "";
			ClearTextArray(inRightHandText);
			rightHandActionImage.enabled = false;
			   
			if (Services.GameManager.playerInput.pickupable != null)
			{
				//looking at glass, holding bottle
				if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
				{
					if(leftHandObj.GetComponent<Bottle>() != null){
						leftHandActionImage.enabled = true;
						inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
						inLeftHandText[1].text = "pour";			
					}
				}
			} 

			else if (Services.GameManager.playerInput.iceMaker != null){
				if(leftHandObj.GetComponent<Glass>() != null){
					leftHandActionImage.enabled = true;
					inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inLeftHandText[1].text = "get ice";
				}
			} else if (Services.GameManager.playerInput.sink != null){
				if(leftHandObj.GetComponent<Glass>() != null){
					leftHandActionImage.enabled = true;
					inLeftHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inLeftHandText[1].text = "empty glass";
				}
			}

			else
			{
				ClearTextArray(inLeftHandText);
				leftHandActionImage.enabled = false;
				// ClearTextArray(inRightHandText);
				// rightHandActionImage.enabled = false;
			}
		}
//		left hand empty, right hand not empty
		else if (leftHandObj == null && rightHandObj != null)
		{			
			inLeftHandText[0].text = "";
			inLeftHandText[1].text = "";
			ClearTextArray(inLeftHandText);
			leftHandActionImage.enabled = false;
			if (Services.GameManager.playerInput.pickupable != null)
			{
				//looking at glass, holding bottle
				if (Services.GameManager.playerInput.pickupable.gameObject.GetComponent<Glass>() != null)
				{
					if(rightHandObj.GetComponent<Bottle>() != null){
						rightHandActionImage.enabled = true;
						inRightHandText[0].text = buttonAndKeyStrings[1 + stringOffset];
						inRightHandText[1].text = "pour";			
					}
				} 
			} 
			//using icemaker
			else if (Services.GameManager.playerInput.iceMaker != null){
				if(rightHandObj.GetComponent<Glass>() != null){
					rightHandActionImage.enabled = true;
					inRightHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inRightHandText[1].text = "get ice";
				}
			}
			else if (Services.GameManager.playerInput.sink != null){
				if(rightHandObj.GetComponent<Glass>() != null){
					rightHandActionImage.enabled = true;
					inRightHandText[0].text = buttonAndKeyStrings[0 + stringOffset];
					inRightHandText[1].text = "empty glass";
				}
			}

			else
			{
				// leftHandActionImage.enabled = false;
				// ClearTextArray(inLeftHandText);
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


	private void DropzoneOnlyRay()
	{
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		float distanceToObj = 0;
		RaycastHit hit = new RaycastHit();
 
		if (Physics.Raycast(ray, out hit, rayDist, dropzoneOnlyMask))
		{
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			dropzoneBeingLookedAtGO = hitObj;
			dropzoneBeingLookedAt = dropzoneBeingLookedAtGO.GetComponent<Dropzone>();
			distanceToObj = Vector3.Distance(transform.position, hitObj.transform.position);
  			if (hitObj.GetComponent<Dropzone>() != null)
			{
  				Dropzone dropzone = hitObj.GetComponent<Dropzone>();
				if (!dropzone.isOccupied) //empty dropzone
				{
					isLookingAtEmptyDropzone = true;
					if (distanceToObj <= Services.GameManager.playerInput.maxInteractionDist)
					{
						if (leftHandObj != null)
						{
							// leftHandPickUpImage.enabled = true;
							leftHandActionImage.enabled = true;
							leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
							leftHandControlsText[1].text = "put back";
						}

						if (rightHandObj != null)
						{
							// rightHandPickUpImage.enabled = true;
							rightHandActionImage.enabled = true;
							rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
							rightHandControlsText[1].text = "put back";
						}
					}
				}
				else
				{
					isLookingAtEmptyDropzone = false;
				}
			}
			else
			  {
				  isLookingAtEmptyDropzone = false;
			  }
		}
		else
		{
			isLookingAtEmptyDropzone = false;
 		}
	}

	private void UIRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
//		float rayDist = Services.GameManager.playerInput.maxInteractionDist;
		float rayDist = Mathf.Infinity;
		float distanceToObj = 0;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, controlsMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			distanceToObj = Vector3.Distance(transform.position, hitObj.transform.position);
//			Debug.Log(hitObj.name);
			if(hitObj.GetComponent<Bottle>() != null && !isExceptionTextRequired)
			{
				pickupableBeingLookedAt = hitObj;
				isLookingAtNPC = false;
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
						case Mixer.vermouth:
							targetObj = "vermouth";
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
				if (distanceToObj <= Services.GameManager.playerInput.maxInteractionDist)
				{
					if (dropzoneBeingLookedAt != null)
					{
						if (dropzoneBeingLookedAt.objectsInMe.Count > 0)
						{
							if (leftHandObj == null)
							{
								// leftHandPickUpImage.enabled = true;
								leftHandActionImage.enabled = true;
								leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
								leftHandControlsText[1].text = "pick up";
							}
							else if (leftHandObj != null && dropzoneBeingLookedAt.objectsInMe[0] == pickupableBeingLookedAt)
							{
								// leftHandPickUpImage.enabled = true;
								leftHandActionImage.enabled = true;
								leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
								leftHandControlsText[1].text = "swap";
							}
							if (rightHandObj == null)
							{
								// rightHandPickUpImage.enabled = true;
								rightHandActionImage.enabled = true;
								rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
								rightHandControlsText[1].text = "pick up";
							}
							else
							{
								// rightHandPickUpImage.enabled = true;
								rightHandActionImage.enabled = true;
								rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
								rightHandControlsText[1].text = "swap";
							}					
						}
					}
				}

				else
				{
					ClearUI();
				}
			}

			else if (hitObj.GetComponent<IceMaker>() != null && !isExceptionTextRequired)
			{
				centerText.text = "ice dispenser";
			} 

			else if (hitObj.GetComponent<Sink> () != null && !isExceptionTextRequired){
				centerText.text = "sink";
			} 
			//ray hits glass
			else if (hitObj.GetComponent<Glass>() != null && !isExceptionTextRequired)
			{
				pickupableBeingLookedAt = hitObj;
				isLookingAtNPC = false;
				centerText.text = "glass";
				if (distanceToObj <= Services.GameManager.playerInput.maxInteractionDist)
				{
					if (dropzoneBeingLookedAt != null)
					{
						if (dropzoneBeingLookedAt.objectsInMe.Count > 0)
						{
							if (leftHandObj == null)
							{
								// leftHandPickUpImage.enabled = true;
								leftHandActionImage.enabled = true;
								leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
								leftHandControlsText[1].text = "pick up";
							}
							else if (leftHandObj != null)
							{
								// leftHandPickUpImage.enabled = true;
								leftHandActionImage.enabled = true;
								leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
								leftHandControlsText[1].text = "swap";
							}
	
							if (rightHandObj == null)
							{
								// rightHandPickUpImage.enabled = true;
								rightHandActionImage.enabled = true;
								rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
								rightHandControlsText[1].text = "pick up";
							}
							else
							{
								// rightHandPickUpImage.enabled = true;
								rightHandActionImage.enabled = true;
								rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
								rightHandControlsText[1].text = "swap";
							}
						}						
					}

				}
				else
				{
					ClearUI();
				}
			}

			else if (hitObj.GetComponent<Backdoor>() != null ){
				botCenterImg.SetActive(true);
				bottomCenterText.text = buttonAndKeyStrings[4 + stringOffset];
				bottomCenterInsText.text = "start the day";
			}
			//ray hits NPC
			else if (hitObj.GetComponent<NPC>() != null)
			{
				isLookingAtNPC = true;
 				if (!isExceptionTextRequired)
				{
					centerText.text = hitObj.GetComponent<NPC>().characterName;				
				}
				if (distanceToObj < Services.GameManager.playerInput.maxTalkingDist)
				{
					if (!Services.GameManager.dialogue.isDialogueRunning)
					{
						//Customer is not talking
						botCenterImg.SetActive(true);
						bottomCenterText.text = buttonAndKeyStrings[4 + stringOffset];
						bottomCenterInsText.text = "talk";
						if (rightHandObj != null && distanceToObj <= Services.GameManager.playerInput.maxInteractionDist+1)
						{
 							// rightHandPickUpImage.enabled = true;
							 rightHandActionImage.enabled = true;
							rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
							rightHandControlsText[1].text = "serve";
						} else if (distanceToObj > Services.GameManager.playerInput.maxInteractionDist+1)
						{
							ClearTextArray(rightHandControlsText);
							rightHandActionImage.enabled = false;
							// rightHandPickUpImage.enabled = false;
						}

						if (leftHandObj != null && distanceToObj <= Services.GameManager.playerInput.maxInteractionDist+1)
						{
							// leftHandPickUpImage.enabled = true;
							leftHandActionImage.enabled = true;
							leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
							leftHandControlsText[1].text = "serve";
						} else if (distanceToObj > Services.GameManager.playerInput.maxInteractionDist+1)
						{
							ClearTextArray(leftHandControlsText);
							leftHandActionImage.enabled = false;
							// leftHandPickUpImage.enabled = false;
						}
					}
					else if (Services.GameManager.dialogue.isDialogueRunning)
					{
						//customer is talking
						botCenterImg.SetActive(false);
						bottomCenterText.text = "";
					} 
				}
				else
				{
					ClearUI();
				}
			} 
			//ray hits dropzone

			else if (hitObj.GetComponent<Dropzone>() != null && !hitObj.GetComponent<Dropzone>().isOccupied)
			{
 				
				if (distanceToObj <= Services.GameManager.playerInput.maxInteractionDist)
				{
					// leftHandPickUpImage.enabled = true;
					leftHandActionImage.enabled = true;
					leftHandControlsText[0].text = buttonAndKeyStrings[2 + stringOffset];
					leftHandControlsText[1].text = "put back";
					// rightHandPickUpImage.enabled = true;
					rightHandActionImage.enabled = true;
					rightHandControlsText[0].text = buttonAndKeyStrings[3 + stringOffset];
					rightHandControlsText[1].text = "put back";
				}
				
				else
				{
					ClearUI();
				}
			} 
			//ray hits light switch
			else if (hitObj.GetComponent<LightSwitch>() != null && !isExceptionTextRequired)
			{
				isLookingAtNPC = false;
				if (Services.GameManager.dayManager.dayHasEnded)
				{
					centerText.text = "end the day";
					if (distanceToObj <= Services.GameManager.playerInput.maxInteractionDist)
					{
						botCenterImg.SetActive(true);
						bottomCenterInsText.text = "use";
						bottomCenterText.text = buttonAndKeyStrings[4 + stringOffset];
					} 
					else
					{
						botCenterImg.SetActive(false);
						bottomCenterInsText.text = "";
						bottomCenterText.text = "";
					}

				} 
				else if (!Services.GameManager.dayManager.dayHasEnded)
				{
					if (!isMessageOverrideOn)
					{
						centerText.text = "end the day";
					}
					// Debug.Log(distanceToObj);
					if (distanceToObj <= player.maxInteractionDist)
					{
						botCenterImg.SetActive(true);
						bottomCenterInsText.text = "use";
						bottomCenterText.text = buttonAndKeyStrings[4 + stringOffset];
						if (Services.GameManager.playerInput.i_talk)
						{
							centerText.text = "there are still customers to serve";
							if (!isMessageOverrideOn)
							{
								// Debug.Log("There are still customers to serve!");
 								// centerText.text = "there are still customers to serve";
								StartCoroutine(clearTextCoroutine);
								isMessageOverrideOn = true;
							}					
						}
						else if (!Services.GameManager.playerInput.i_talk && !isMessageOverrideOn)
						{
							centerText.text = "end the day";
						}
					}
					else
					{
						botCenterImg.SetActive(false);
						bottomCenterInsText.text = "";
						bottomCenterText.text = "";
					}
				} 
			} 
			
			else {
 				StopCoroutine(clearTextCoroutine);
				isMessageOverrideOn = false;
				botCenterImg.SetActive(false);
				bottomCenterText.text = "";
				bottomCenterInsText.text = "";
				if (!isExceptionTextRequired)
				{
					centerText.text = "";				
				}
				// rightHandPickUpImage.enabled = false;
				rightHandActionImage.enabled = false;
				rightHandControlsText[0].text = "";
				rightHandControlsText[1].text = "";			
				// leftHandPickUpImage.enabled = false;
				leftHandActionImage.enabled = false;
				leftHandControlsText[0].text = "";
				leftHandControlsText[1].text = "";
			} 
		} else {
			botCenterImg.SetActive(false);
			bottomCenterText.text = "";
			bottomCenterInsText.text = "";
			if (!isExceptionTextRequired)
			{
				centerText.text = "";				
			}
			// rightHandPickUpImage.enabled = false;
			rightHandActionImage.enabled = false;
			rightHandControlsText[0].text = "";
			rightHandControlsText[1].text = "";	
			// leftHandPickUpImage.enabled = false;
			leftHandActionImage.enabled = false;
			leftHandControlsText[0].text = "";
			leftHandControlsText[1].text = "";		
		}
	}

	public void ClearUI()
	{
		botCenterImg.SetActive(false);
		bottomCenterText.text = "";
		bottomCenterInsText.text = "";
		// rightHandPickUpImage.enabled = false;
		// rightHandActionImage.enabled = false;
		rightHandControlsText[0].text = "";
		rightHandControlsText[1].text = "";
		// leftHandPickUpImage.enabled = false;
		// leftHandActionImage.enabled = false;
		leftHandControlsText[0].text = "";
		leftHandControlsText[1].text = "";
	}

	private void ClearNonCenteredUI()
	{
		// rightHandPickUpImage.enabled = false;
		// rightHandControlsText[0].text = "";
		// rightHandControlsText[1].text = "";
		// leftHandPickUpImage.enabled = false;
		// leftHandControlsText[0].text = "";
		// leftHandControlsText[1].text = "";
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
		isExceptionTextRequired = false;
		isMessageOverrideOn = false;
	}
	
	public static Sprite GetSprite(string _fileName){
		Sprite mySprite;
		mySprite = Resources.Load<Sprite>("UI/" + _fileName);
		return mySprite; 
	}

	public void ChangeUIOnControllerConnect()
	{
		stringOffset = 5;
		leftHandActionImage.sprite = GetSprite("icon_trigger1");
		rightHandActionImage.sprite = GetSprite("icon_trigger1");
		// leftHandPickUpImage.sprite = GetSprite("icon_trigger2");
		// rightHandPickUpImage.sprite = GetSprite("icon_trigger2");
		botCenterImg.GetComponent<Image>().sprite = GetSprite("icon_x");
	}

	public void ChangeUIOnControllerDisconnect()
	{
		stringOffset = 0;
		leftHandActionImage.sprite = GetSprite("icon_lmb");
		rightHandActionImage.sprite = GetSprite("icon_rmb");
		// leftHandPickUpImage.sprite = GetSprite("icon_key");
		// rightHandPickUpImage.sprite = GetSprite("icon_key");
		botCenterImg.GetComponent<Image>().sprite = GetSprite("icon_key");
	}


	public void ChangeCenterTextWithLinger(string text)
	{
		isExceptionTextRequired = true;
		centerText.text = text;
  		StartCoroutine(ClearTextCoroutine(centerText, 3));
	}

	public void SetCenterText(string text){
		centerText.text = text;
	}
	void ClearText(Text text)
	{
		text.text = "";
	}





}
