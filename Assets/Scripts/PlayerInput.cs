using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour {
	
	[SerializeField]float moveSpeed = 2.0f;
	[SerializeField]float lookSensitivity = 0.01f;
	// [SerializeField]float smoothing = 2.0f;

	Vector2 smoothV;

	private Vector3 moveVector;
	private Vector2 lookVector;
	public int playerId = 0; 
	private Player player;
	private CharacterController cc;
	private Camera myCam;
	public LayerMask layerMask;
	Pickupable pickupable;
	private float maxInteractionDist = 4f;

	bool in_pickUpLeft;

	void Awake(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		player = ReInput.players.GetPlayer(playerId);
		cc = GetComponent<CharacterController>();
		myCam = GetComponentInChildren<Camera>();
	}

	void Update(){
		GetInput();
		ProcessInput();
 	}

	private void GetInput(){
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.z = player.GetAxis("Move Vertical");
		lookVector.x = player.GetAxis("Look Horizontal");
		lookVector.y = player.GetAxis("Look Vertical");
		in_pickUpLeft = player.GetButtonDown("Pick Up Left");

		// Debug.Log(lookVector.x + " " + lookVector.y);
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
		lookVector.y = Mathf.Clamp (lookVector.y, -90f, 90f);

		myCam.transform.Rotate (-lookVector.y * lookSensitivity, 0, 0f);
		cc.transform.Rotate (0, lookVector.x * lookSensitivity, 0);
		#endregion
		
		#region Pick Up Left
		if(in_pickUpLeft){
 			Debug.Log("Picking up with left hand");
			if(pickupable != null){
				pickupable.TweenToPlayer();
			}
		}       
		#endregion 

	}

	private void InteractionRay(){
		Ray ray = new Ray(transform.position, transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();

		if(Physics.Raycast(ray, out hit, rayDist, layerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
 			
			if(hitObj.GetComponent<Pickupable>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxInteractionDist){ //check if object looked at can be picked up
				pickupable = hitObj.GetComponent<Pickupable>(); //if it's Pickupable and close enough, assign it to pickupable.				  
 			} 		
		} else { //no hit
			pickupable = null; //if you're not looking at anything, make this null
 		}


	}

}
