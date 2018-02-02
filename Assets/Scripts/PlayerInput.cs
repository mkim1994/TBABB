using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;
 
[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour {
	
	[SerializeField]float moveSpeed = 2.0f;
	[SerializeField]float lookSensitivity = 0.01f;
	// [SerializeField]float smoothing = 2.0f;

	Vector2 smoothV;
	float t = 0;

	//movement
	private Vector3 moveVector;
	private Vector2 lookVector;
	public int playerId = 0; 
	private Player player;
	private CharacterController cc;
	public Dropzone targetDropzone;
	public NPC npc;

	public Vector3 dropPos;
	private Camera myCam;
	//raycast management
	public LayerMask layerMask;
	public LayerMask dropzoneLayerMask;
	public LayerMask customerLayerMask;
	public Pickupable pickupable;
	public Pickupable pickupableInLeftHand;
	public Pickupable pickupableInRightHand;
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
	bool i_talk;

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
		CustomerRay();
  	}

	void FixedUpdate(){

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
			Vector3 aimAssistDir = myCam.transform.position - pickupable.transform.position;
 			//aim assist attempt (not working)
 			// if(Mathf.Abs){
				// float aimY = verticalLook;
				// aimY = Mathf.Lerp(verticalLook, aimAssistDir.y, t);
				// float aimX = lookVector.x;
				// aimX = Mathf.Lerp(lookVector.x, aimAssistDir.x, t);
				// myCam.transform.localRotation = Quaternion.Euler(aimY, 0, 0);
				// cc.transform.Rotate(0, aimX, 0);
			// }  
			// if(lookSensitivity == aimAssistSensitivity){
			// 	t = 0;
			// }
			
		} else {
 			t += 4f * Time.deltaTime;
			lookSensitivity = Mathf.Lerp(lookSensitivity, lookSensitivityAtStart, t);
			// if(lookSensitivity == lookSensitivityAtStart){
			// 	t = 0;
			// }
		}

		#endregion
		
		#region Pick Up Left / Drop Left
		if(i_pickupLeft && !Services.TweenManager.tweensAreActive){
			if(pickupable != null && pickupableInLeftHand == null){ //PICK UP, CHECK IF LEFT HAND IS EMPTY
				pickupable.InteractLeftHand();
				targetDropzone.isOccupied = false;
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
			if (pickupableInLeftHand != null && pickupable != null){ //one-handed use on something on bar (prioritize this)
				pickupableInLeftHand.UseLeftHand();
			} else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use 
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) {
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>());
                } else if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Rag>() != null){
 				} 
            }
			if (pickupableInRightHand != null && pickupable != null){ //one-handed use on something on bar (prioritize this)
				pickupableInRightHand.UseRightHand();
			} else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use 
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
			if (pickupableInLeftHand != null && pickupable != null){ //one-handed use on something on bar (prioritize this)
				pickupableInLeftHand.UseLeftHand();
			} else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use 
                if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) {
 					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>());
                } else if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Rag>() != null){
 				} 
            }
			if (pickupableInRightHand != null && pickupable != null){ //one-handed use on something on bar (prioritize this)
				pickupableInRightHand.UseRightHand();
			} else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use 
                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>());
                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Rag>() != null){
 				} 
            } 
		} 
		if(i_endUseRight){
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

		#region Talk
		if(i_talk){
			if(npc != null){
				npc.InitiateDialogue();
			}
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

	private void CustomerRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, customerLayerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
 			if(hitObj.GetComponent<NPC>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxTalkingDist){ //check if object looked at can be picked up
				npc = hitObj.GetComponent<NPC>(); //if it's NPC and close enough, assign it to NPC.				  
 			} else if (hitObj.GetComponent<NPC>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > maxTalkingDist){
				npc = null;
 			} 	
		} else {
			npc = null;
		}
	}
}
