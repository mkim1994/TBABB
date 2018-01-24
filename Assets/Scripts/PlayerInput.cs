using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour {
	
	[SerializeField]float moveSpeed = 2.0f;
	[SerializeField]float lookSensitivity = 0.01f;
	[SerializeField]float smoothing = 2.0f;

	Vector2 smoothV;

	private Vector3 moveVector;
	private Vector2 lookVector;
	public int playerId = 0; 
	private Player player;
	
	private CharacterController cc;
	private Camera myCam;

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
		// Debug.Log(lookVector.x + " " + lookVector.y);
	}

	private void ProcessInput(){
		if(moveVector.x != 0.0f || moveVector.y != 0.0f) {
            cc.Move(moveVector * moveSpeed * Time.deltaTime);
        }

		lookVector.y = Mathf.Clamp (lookVector.y, -90f, 90f);

		myCam.transform.Rotate (-lookVector.y * lookSensitivity, 0, 0f);
		cc.transform.Rotate (0, lookVector.x * lookSensitivity, 0);

		// myCam.transform.localRotation = Quaternion.AngleAxis(-lookVector.y * lookSensitivity, Vector3.right); //look up/down
		// transform.localRotation = Quaternion.AngleAxis(lookVector.x * lookSensitivity, Vector3.up); //look left/right

		// myCam.transform.Rotate (-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0f);

		// 	//unroll our camera
		// myCam.transform.localEulerAngles = new Vector3 (myCam.transform.localEulerAngles.x, myCam.transform.localEulerAngles.y, 0f); 
	}

}
