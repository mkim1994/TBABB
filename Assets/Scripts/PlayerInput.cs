using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ComponentControls.Data;
using UnityEngine.SceneManagement;
 
[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour {
	
	[SerializeField]float moveSpeed = 2.0f;
	[SerializeField]float lookSensitivity = 0.005f;
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
	public List<GameObject> pickupableGOs = new List<GameObject>();
	private float maxInteractionDist = 4f;
	private float maxTalkingDist = 8f;

	private float lookSensitivityAtStart;
	private float aimAssistSensitivity = 0;
	float verticalLook = 0f;

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
		lookSensitivityAtStart = lookSensitivity;
		aimAssistSensitivity = lookSensitivity * 0.49f;
 	}

	void Update(){
		GetInput();
		ProcessInput();
		InteractionRay();
		DropzoneRay();
		NonPickupableRay();
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
			if(pickupable != null && pickupableInLeftHand == null){ //PICK UP, CHECK IF LEFT HAND IS EMPTY
				pickupable.InteractLeftHand();
				if (targetDropzone != null)
				{
					targetDropzone.isOccupied = false;
				}	
			} else if(pickupable == null && pickupableInLeftHand != null && targetDropzone != null){ //DROP
				if(dropPos != Vector3.zero && !targetDropzone.isOccupied){
					pickupableInLeftHand.dropPos = dropPos;
					pickupableInLeftHand.targetDropzone = targetDropzone;
					pickupableInLeftHand.InteractLeftHand();
				}		
			} else if (pickupable != null && pickupableInLeftHand != null){ //swap
				if(dropPos != Vector3.zero){ 
					pickupableInLeftHand.dropPos = dropPos;
					pickupableInLeftHand.targetDropzone = targetDropzone;
					pickupableInLeftHand.SwapLeftHand();
					pickupable.SwapLeftHand();		
				}
			}
		}      
		#endregion 
		
		#region Pick Up Right / Drop Right
		if(i_pickupRight && !Services.TweenManager.tweensAreActive){
 			if(pickupable != null && pickupableInRightHand == null){ //PICK UP, CHECK IF LEFT HAND IS EMPTY
 				pickupable.InteractRightHand();
				targetDropzone.isOccupied = false;
			} 
 			else if(pickupable == null && pickupableInRightHand != null && targetDropzone != null){ //DROP
				if(dropPos != Vector3.zero && !targetDropzone.isOccupied){
					pickupableInRightHand.dropPos = dropPos;
					pickupableInRightHand.targetDropzone = targetDropzone;
					pickupableInRightHand.InteractRightHand();
 				}		
			} 
 			else if (pickupable != null && pickupableInRightHand != null){ //swap
				if(dropPos != Vector3.zero){
 					pickupableInRightHand.dropPos = dropPos;
					pickupableInRightHand.targetDropzone = targetDropzone;
					pickupableInRightHand.SwapRightHand();
					pickupable.SwapRightHand();		
				}
			}
		}   
		#endregion

		#region Restart
		if(i_restart){
			SceneManager.LoadScene("main");
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
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) {
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>());
                } else if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Rag>() != null){
 				} 
            }
		
			if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use (right hand)
                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>());
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Rag>() != null){
 				} 
            }  
		} 
		if(i_endUseLeft){
			if(pickupableInLeftHand != null){
				pickupableInLeftHand.RotateToZeroTween();
			}
			if (pickupableInLeftHand != null && pickupableInRightHand != null){
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInRightHand.RotateToZeroTween();
				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
				}
				if(pickupableInRightHand.GetComponent<Glass>() != null){	
					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
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
			//two-handed use (bottle in left hand
			else if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) {
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>());
                } else if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Rag>() != null){
 				} 
            }
			//two-handed use (bottle in right hand)
			if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>());
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Rag>() != null){
 				} 
            } 
		} 
		if(i_endUseRight){
			if(pickupableInRightHand != null){
				pickupableInRightHand.RotateToZeroTween();
			}
			if (pickupableInLeftHand != null && pickupableInRightHand != null){
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInRightHand.RotateToZeroTween();
				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
				}
				if(pickupableInRightHand.GetComponent<Glass>() != null){	
					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
				}
			}
		} 
		#endregion

		#region Talk
		if(i_talk){
			if(npc != null){
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
				targetDropzone = hitDropzone;
			} 
			else if (Vector3.Distance(transform.position, hitObj.transform.position) > maxInteractionDist) {
				dropPos = Vector3.zero;
				targetDropzone = null;
			}
		} else {
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
