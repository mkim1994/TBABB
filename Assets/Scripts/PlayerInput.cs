using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Rewired;
using Rewired.ComponentControls.Data;
using UnityEngine.SceneManagement;
 
[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour {
	
	
	// [SerializeField]float smoothing = 2.0f;

	Vector2 smoothV;
	float t = 0;

	//movement
	private Vector3 moveVector;
	private Vector2 lookVector;
	public int playerId = 0; 
	private Player player;
	private CharacterController cc;
//	[SerializeField]Coaster targetCoaster;
	public NPC npc;

	public Vector3 dropPos;
	protected Camera myCam;
	//raycast management
	public Dropzone targetDropzone;
	[SerializeField] private LightSwitch lightSwitch;
	public LayerMask layerMask;
	public LayerMask dropzoneLayerMask;
	public LayerMask nonPickupableLayerMask;
	public LayerMask coasterLayerMask;
	public Pickupable pickupable;
	public Pickupable pickupableInLeftHand;
	public Pickupable pickupableInRightHand;
//	public List<GameObject> pickupableGOs = new List<GameObject>();
	public float maxInteractionDist = 4f;
	public float maxTalkingDist = 8f;
	
	[SerializeField]float moveSpeed = 2.0f;
	public float lookSensitivity;
	public float controllerSens;
	public float mouseSens;
	
	public float lookSensitivityAtStart;
	public float aimAssistSensitivity = 0;
	public float aimAssistFactor;
	float verticalLook = 0f;

	public bool isInputEnabled = true;
	public bool isUsingController = false;

	//buttons
	bool i_pickupLeft;
	bool i_pickupRight;
	bool i_useLeft;
	bool i_endUseLeft;
	bool i_useRight;
	bool i_endUseRight;
	bool i_restart;
	public bool i_talk;
	bool i_choose1;
	bool i_choose2;
	
	void Awake(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		player = ReInput.players.GetPlayer(playerId);
		cc = GetComponent<CharacterController>();
		myCam = GetComponentInChildren<Camera>();
//		lookSensitivityAtStart = lookSensitivity;
	}

	void Start()
	{
		if (isUsingController)
		{
			lookSensitivity = controllerSens;
			lookSensitivityAtStart = controllerSens;
			aimAssistSensitivity = lookSensitivity * aimAssistFactor;
		}
		
		else if (!isUsingController)
		{
			lookSensitivity = mouseSens;
			lookSensitivityAtStart = mouseSens;
			aimAssistSensitivity = lookSensitivity * aimAssistFactor;
		}
	}


	void Update(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		if (isInputEnabled)
		{
			GetInput();
			ProcessInput();
			InteractionRay();
			DropzoneRay();
			NonPickupableRay();
		}
		
		i_restart = player.GetButtonDown("Restart");
		
		#region Restart
		if(i_restart){
			SceneManager.LoadScene("main");
		}
		#endregion

  	}

	private void GetInput(){
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.z = player.GetAxis("Move Vertical");
		lookVector.x = player.GetAxis("Look Horizontal");
		lookVector.y = player.GetAxis("Look Vertical");
		i_pickupLeft = player.GetButtonDown("Pick Up Left");
		i_pickupRight = player.GetButtonDown("Pick Up Right");
		i_useLeft = player.GetButton("Use Left");
		i_endUseLeft = player.GetButtonUp("Use Left");
		i_useRight = player.GetButton("Use Right");
		i_endUseRight = player.GetButtonUp("Use Right");
		i_restart = player.GetButtonDown("Restart");
		i_talk = player.GetButtonDown("Talk");
		i_choose1 = player.GetButtonDown("Choose1");
		i_choose2 = player.GetButtonDown("Choose2");
	}

	private void ProcessInput(){		
		#region Movement
		//which direction you'll move in
		Vector3 moveDir = new Vector3 (moveVector.x, 0, moveVector.z);

		//Speed is directed towards where you're facing.
		moveDir = transform.rotation * moveDir;

		cc.Move (moveDir * moveSpeed * Time.deltaTime);
		#endregion

		#region MouseLook
		
		verticalLook -= lookVector.y * lookSensitivity;
		verticalLook = Mathf.Clamp (verticalLook, -90f, 90f);
		myCam.transform.localRotation = Quaternion.Euler (verticalLook, 0, 0);
		cc.transform.Rotate (0, lookVector.x * lookSensitivity, 0);

		if(pickupable != null){ //aim assist
			t += 2f * Time.deltaTime;
			lookSensitivity = Mathf.Lerp(lookSensitivityAtStart, aimAssistSensitivity, t);
			
		} else {
			t += 4f * Time.deltaTime;
			lookSensitivity = Mathf.Lerp(lookSensitivity, lookSensitivityAtStart, t);
		}			
	


		#endregion
		
		#region Pick Up Left / Drop Left
		if(i_pickupLeft && !Services.TweenManager.tweensAreActive){
			if(pickupable != null && pickupableInLeftHand == null){ //PICK UP WITH LEFT HAND, CHECK IF LEFT HAND IS EMPTY
				if (targetDropzone != null)
				{
					if (targetDropzone.GetComponentInParent<Coaster>() != null)
					{
						Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						if (targetCoaster.myCustomer.IsCustomerPresent() && !targetCoaster.myCustomer.HasAcceptedDrink()) //if  customer is present and customer HAS NOT accepted drink, then you can get it
						{
							pickupable.InteractLeftHand();						
//							targetDropzone.isOccupied = false;
						}
						else if(!targetCoaster.myCustomer.insideBar)
						{
							pickupable.InteractLeftHand();
						}
						else
						{
							GetComponent<UIControls>().ChangeCenterText("customer is still drinking");
						}
					}
					else
					{
						pickupable.InteractLeftHand();
					}
				}
			} else if(pickupable == null && pickupableInLeftHand != null && targetDropzone != null){ //DROP
				if(dropPos != Vector3.zero && !targetDropzone.isOccupied){
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInLeftHand.dropPos = dropPos;
						pickupableInLeftHand.targetDropzone = targetDropzone;
						pickupableInLeftHand.InteractLeftHand();
					}
					else
					{
						Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						if (!targetCoaster.myCustomer.IsCustomerPresent())
						{
 							GetComponent<UIControls>().ChangeCenterText("no one to serve");
						} else if (targetCoaster.myCustomer.IsCustomerPresent() && !targetCoaster.myCustomer.HasAcceptedDrink())
						{
							pickupableInLeftHand.dropPos = dropPos;
							pickupableInLeftHand.targetDropzone = targetDropzone;
							pickupableInLeftHand.InteractLeftHand();
						}
					}
				}		
			} else if (pickupable != null && pickupableInLeftHand != null){ //swap
				if(dropPos != Vector3.zero){			
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInLeftHand.dropPos = dropPos;
						pickupableInLeftHand.targetDropzone = targetDropzone;
						pickupableInLeftHand.SwapLeftHand();
						pickupable.SwapLeftHand();
					}
					else
					{
						Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						if (!targetCoaster.myCustomer.HasAcceptedDrink())
						{
							pickupableInLeftHand.dropPos = dropPos;
							pickupableInLeftHand.targetDropzone = targetDropzone;
							pickupableInLeftHand.SwapLeftHand();
							pickupable.SwapLeftHand();
						}
						else if (targetCoaster.myCustomer.HasAcceptedDrink())
						{
							GetComponent<UIControls>().ChangeCenterText("customer is still drinking");
						}
					}
				}
			}
		}      
		#endregion 
		
		#region Pick Up Right / Drop Right
		if(i_pickupRight && !Services.TweenManager.tweensAreActive){
 			if(pickupable != null && pickupableInRightHand == null){ //PICK UP WITH RIGHT HAND, CHECK IF RIGHT HAND IS EMPTY
				 if (targetDropzone != null)
				 {
					 if (targetDropzone.GetComponentInParent<Coaster>() != null)
					 {
						 Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						 if (targetCoaster.myCustomer.IsCustomerPresent() && !targetCoaster.myCustomer.HasAcceptedDrink()) //if  customer is present and customer HAS NOT accepted drink, then you can get it
						 {
							 pickupable.InteractRightHand();						
//							 targetDropzone.isOccupied = false;
						 }
						 else if(!targetCoaster.myCustomer.insideBar)
						 {
							 pickupable.InteractRightHand();
						 }
						 else
						 {
							 GetComponent<UIControls>().ChangeCenterText("customer is still drinking");
						 }
					 }
					 else
					 {
						 pickupable.InteractRightHand();
					 }
				 }
			} 
 			else if(pickupable == null && pickupableInRightHand != null && targetDropzone != null){ //DROP
				if(dropPos != Vector3.zero && !targetDropzone.isOccupied){
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInRightHand.dropPos = dropPos;
						pickupableInRightHand.targetDropzone = targetDropzone;
						pickupableInRightHand.InteractRightHand();
					}
					else
					{
						Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						if (!targetCoaster.myCustomer.IsCustomerPresent())
						{
 							GetComponent<UIControls>().ChangeCenterText("no one to serve");
						} else if (targetCoaster.myCustomer.IsCustomerPresent() && !targetCoaster.myCustomer.HasAcceptedDrink())
						{
							pickupableInRightHand.dropPos = dropPos;
							pickupableInRightHand.targetDropzone = targetDropzone;
							pickupableInRightHand.InteractRightHand();
						}
					}
 				}		
			} 
 			else if (pickupable != null && pickupableInRightHand != null){ //swap
				if(dropPos != Vector3.zero){
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInRightHand.dropPos = dropPos;
						pickupableInRightHand.targetDropzone = targetDropzone;
						pickupableInRightHand.SwapRightHand();
						pickupable.SwapRightHand();
					}
					else
					{
						Coaster targetCoaster = targetDropzone.GetComponentInParent<Coaster>();
						if (!targetCoaster.myCustomer.HasAcceptedDrink())
						{
							pickupableInRightHand.dropPos = dropPos;
							pickupableInRightHand.targetDropzone = targetDropzone;
							pickupableInRightHand.SwapRightHand();
							pickupable.SwapRightHand();							
						} else if (targetCoaster.myCustomer.HasAcceptedDrink())
						{
							GetComponent<UIControls>().ChangeCenterText("customer is still drinking");
							
						}
					}				
				}
			}
		}   
		#endregion

		#region Test
		//hahaha

		#endregion

		#region Use Left
		if(i_useLeft && !Services.TweenManager.tweensAreActive){
			//one-handed use on something on bar
			if (pickupableInLeftHand != null && pickupable != null && pickupableInRightHand == null){ 
				pickupableInLeftHand.UseLeftHand();
			} 
			
			else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use (left hand)
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) { //BOTTLE : GLASS
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>(), 1);
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Bottle>() != null){ //BOTTLE : BOTTLE
	                pickupableInLeftHand.UseLeftHand();				
                }  
            }
		
			if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use (right hand)
                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>(), 0);
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Rag>() != null){
 				} 
            }  
		} 
		if(i_endUseLeft){
			if(pickupableInLeftHand != null){
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInLeftHand.EndPourTween();
				if (pickupable != null)
				{
					if (pickupable.GetComponent<Glass>() != null)
					{
						pickupable.GetComponent<Glass>().EndPourFromBottle();
					}
				}
			}
			if (pickupableInLeftHand != null && pickupableInRightHand != null){
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInRightHand.RotateToZeroTween();
				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
					pickupableInLeftHand.GetComponent<Glass>().EndPourTween();
					pickupableInRightHand.EndPourTween();

				}
				if(pickupableInRightHand.GetComponent<Glass>() != null){	
					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
					pickupableInRightHand.GetComponent<Glass>().EndPourTween();
					pickupableInLeftHand.EndPourTween();
				}
			}
		} 
		
		#endregion

		#region Use Right

		if(i_useRight && !Services.TweenManager.tweensAreActive){
			//one-handed use on something on bar
			if (pickupableInRightHand != null && pickupable != null && pickupableInLeftHand == null){
				pickupableInRightHand.UseRightHand();				
			} 
			//two-handed use (bottle in left hand, glass in right)
			else if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) {
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>(), 1);
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Bottle>() != null){ //Dual bottle
	                pickupableInRightHand.UseRightHand();
                 } 
            }
			//two-handed use (bottle in right hand, glass in left)
			if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>(), 0);
                } 
            } 
		} 
		if(i_endUseRight){
			if(pickupableInRightHand != null){
				pickupableInRightHand.RotateToZeroTween();
				pickupableInRightHand.EndPourTween();
				if (pickupable != null)
				{
					if (pickupable.GetComponent<Glass>() != null)
					{
						pickupable.GetComponent<Glass>().EndPourFromBottle();
					}
				}
			}
			if (pickupableInLeftHand != null && pickupableInRightHand != null){
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInRightHand.RotateToZeroTween();
				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
					pickupableInLeftHand.GetComponent<Glass>().EndPourTween();
					pickupableInRightHand.EndPourTween();
				}
				if(pickupableInRightHand.GetComponent<Glass>() != null){	
					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
					pickupableInRightHand.GetComponent<Glass>().EndPourTween();
					pickupableInLeftHand.EndPourTween();
 				}
			}
		} 
		#endregion

		#region Talk
		if(i_talk){
			if(npc != null && !Services.GameManager.dialogue.isDialogueRunning){
				npc.InitiateDialogue();
			} 
			
			if (lightSwitch != null)
			{
				lightSwitch.EndDay();
			}
		}
		#endregion
		#region Dialogue Selection
		if(i_choose1){
			// Services.GameManager.dialogue.dialogueUI.ChooseOption(0);
 			Services.GameManager.dialogue.GetComponent<DialogueUI>().ChooseOption(0);
		}

		if(i_choose2){
			// Services.GameManager.dialogue.dialogueUI.ChooseOption(1);
			Services.GameManager.dialogue.GetComponent<DialogueUI>().ChooseOption(0);
 		}
		#endregion
	}

	private void InteractionRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, layerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
 			if(hitObj.GetComponent<Pickupable>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxInteractionDist){ //check if object looked at can be picked up
				pickupable = hitObj.GetComponent<Pickupable>(); //if it's Pickupable and close enough, assign it to pickupable.				  
 			} else if (hitObj.GetComponent<Pickupable>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > maxInteractionDist ){
				pickupable = null;
				t = 0;
			} 	
		} 
	}

	private void DropzoneRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, dropzoneLayerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			if (hitObj.GetComponent<Dropzone>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxInteractionDist){
				Dropzone hitDropzone = hitObj.GetComponent<Dropzone>(); // get a reference to the dropzone
				dropPos = hitObj.transform.position;
//				hitDropzone.playerIsLooking = true;
				targetDropzone = hitDropzone;
			} 
			else if (Vector3.Distance(transform.position, hitObj.transform.position) > maxInteractionDist) {
				dropPos = Vector3.zero;
				if (targetDropzone != null)
				{
					targetDropzone.playerIsLooking = false;				
				}
				targetDropzone = null;
			}
		} else
		{
			if (targetDropzone != null)
			{
//				targetDropzone.playerIsLooking = false;			
			}
			dropPos = Vector3.zero;
			targetDropzone = null;
		}
	}

	private void NonPickupableRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, nonPickupableLayerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
//			Debug.Log(hitObj.transform.name);
 			if(hitObj.GetComponent<NPC>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxTalkingDist){ //check if object looked at can be picked up
				npc = hitObj.GetComponent<NPC>(); //if it's NPC and close enough, assign it to NPC.				  
 			} else if (hitObj.GetComponent<NPC>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > maxTalkingDist){
				npc = null;
 			} 
			
			if (hitObj.GetComponent<LightSwitch>() != null)
			{
				lightSwitch = hitObj.GetComponent<LightSwitch>();
			} else if (hitObj.GetComponent<LightSwitch>() == null)
			{
				lightSwitch = null;
			}
		} else {
			npc = null;
			lightSwitch = null;
		}
	}

//	private void CoasterRay(){
//		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
//		float rayDist = Mathf.Infinity;
//		RaycastHit hit = new RaycastHit();
//		
//		if(Physics.Raycast(ray, out hit, rayDist, coasterLayerMask)){
//			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
// 			if(hitObj.GetComponent<Coaster>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxTalkingDist){ //check if object looked at can be picked up
// 				targetCoaster = hitObj.GetComponent<Coaster>(); //if it's NPC and close enough, assign it to NPC.				  
// 			} else if (hitObj.GetComponent<Coaster>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > maxTalkingDist){
//				targetCoaster = null;
// 			} 	
//		} else {
//			targetCoaster = null;
//		}
//	}
}
