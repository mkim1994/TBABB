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
	private Vector3 moveVector;
	private Vector2 lookVector;
	public int playerId = 0; 
	private Player player;
	private CharacterController cc;

	public Vector3 dropPos;
	private Camera myCam;
	public LayerMask layerMask;

	public LayerMask dropzoneLayerMask;
	public Pickupable pickupable;
	public Pickupable pickupableInLeftHand;
	public Pickupable pickupableInRightHand;
	private float maxInteractionDist = 4f;

	private float lookSensitivityAtStart;
	private float aimAssistSensitivity = 0;
	float verticalLook = 0f;

	bool i_pickupLeft;
	bool i_pickupRight;

	bool i_useLeft;
	bool i_useRight;
	bool i_restart;

	bool lookingAtDropzone;

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
  	}

	private void GetInput(){
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.z = player.GetAxis("Move Vertical");
		lookVector.x = player.GetAxis("Look Horizontal");
		lookVector.y = player.GetAxis("Look Vertical");
		i_pickupLeft = player.GetButtonDown("Pick Up Left");
		i_pickupRight = player.GetButtonDown("Pick Up Right");
		i_useLeft = player.GetButton("Use Left");
		i_useRight = player.GetButton("Use Right");
		i_restart = player.GetButtonDown("Restart");
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
		
		#region Pick Up Left
		if(i_pickupLeft){
			if(pickupable != null){ //check if looking at pickupable
				pickupable.InteractLeftHand();
			} else if(pickupableInLeftHand != null){
				if(dropPos != Vector3.zero){
					pickupableInLeftHand.dropPos = dropPos;
					pickupableInLeftHand.InteractLeftHand();
				}
			}
		}      
		#endregion 
		
		#region Pick Up Right
		if(i_pickupRight){
			if(pickupable != null){ //check if looking at pickupable
				pickupable.InteractRightHand();
			} else if(pickupableInRightHand != null){
				if(dropPos != Vector3.zero){
					pickupableInRightHand.dropPos = dropPos;
					pickupableInRightHand.InteractRightHand();
				}
			}
		}   
		#endregion

		#region Restart
		if(i_restart){
			SceneManager.LoadScene("main");
		}
		#endregion

		#region Use Left
		if(i_useLeft){
			if(pickupableInLeftHand != null && pickupable != null){ //if you're holding something in your left hand
				pickupableInLeftHand.UseLeftHand();
			}
		} else {
			if(pickupableInLeftHand != null){
				pickupableInLeftHand.ReversePourTween();
			}
		} 
		#endregion

		#region Use Right
		if(i_useRight){
 			if(pickupableInRightHand != null && pickupable != null){ //if you're holding something in your left hand
				pickupableInRightHand.UseRightHand();
			}
		} else {
			if(pickupableInRightHand != null){
				pickupableInRightHand.ReversePourTween();
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
				dropPos = hitObj.transform.position;
				lookingAtDropzone = true;
			} else{
				dropPos = Vector3.zero;
				lookingAtDropzone = false;
			}
		}
	}
}
