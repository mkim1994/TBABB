using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using Rewired;
using Rewired.ComponentControls.Data;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{


	// [SerializeField]float smoothing = 2.0f;
	public delegate void TweenManagerDelegate();
	private TweenManagerDelegate _tweenManagerDelegate;

	public delegate void StartPourDelegate(Bottle bottle, int num);
	private StartPourDelegate _startPourDelegate;

	private IEnumerator tweenManagerCoroutine;
	private IEnumerator startPourCoroutine;

	public enum InteractionState
	{
		BothHandsInUse,
		LeftHasObject_RightEmpty,
		LeftEmpty_RightHasObject,
		BothHandsEmpty,
		LeftHasBottle_RightHasGlass,
		LeftHasGlass_RightHasBottle
	}

	private InteractionState _interactionState;

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
	private LightSwitch lightSwitch;
	private Sink sink;
	[SerializeField]private Backdoor backdoor;
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
	[SerializeField]float verticalLook = 0f;

	public bool isInputEnabled = true;
	public bool isUsingController = false;

	//buttons
	bool i_pickupLeft;
	bool i_pickupRight;
	public bool i_startUseLeft;
	public bool i_startUseRight;
	public bool i_useLeft;
	public bool i_endUseLeft;
	public bool i_useRight;
	public bool i_endUseRight;
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
		 _tweenManagerDelegate = Pickupable.DeclareAllTweensInactive; 
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

	private void FixedUpdate()
	{
		if (isInputEnabled)
		{
			GetInput();
			ProcessInput();
			InteractionRay();
			DropzoneRay();
			NonPickupableRay();
		}
	}

	void Update(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		//checks what player is holding to change the interaction state
		if (pickupableInLeftHand != null && pickupableInRightHand != null)
		{
			if (pickupableInLeftHand.GetComponent<Bottle>() != null && 
			    pickupableInRightHand.GetComponent<Glass>() != null)
			{
				_interactionState = InteractionState.LeftHasBottle_RightHasGlass;
			} 
			
			else if (pickupableInLeftHand.GetComponent<Glass>() != null &&
			           pickupableInRightHand.GetComponent<Bottle>() != null)
			{
				_interactionState = InteractionState.LeftHasGlass_RightHasBottle;
			} else if (pickupableInLeftHand != null && pickupableInRightHand == null)
			{
				_interactionState = InteractionState.LeftHasObject_RightEmpty;
			} else if (pickupableInLeftHand == null && pickupableInRightHand != null)
			{
				_interactionState = InteractionState.LeftEmpty_RightHasObject;
			}
		}

		#region Restart
		i_restart = player.GetButtonDown("Restart");
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
		i_startUseLeft = player.GetButtonDown("Use Left");
		i_startUseRight = player.GetButtonDown("Use Right");
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
			} else if (pickupable != null && pickupableInLeftHand != null && targetDropzone != null){ //swap
				if(dropPos != Vector3.zero){			
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInLeftHand.dropPos = dropPos;
						pickupableInLeftHand.targetDropzone = targetDropzone;
						if (pickupableInLeftHand.targetDropzone.objectsInMe.Count > 0)
						{
							if (pickupable.gameObject == pickupableInLeftHand.targetDropzone.objectsInMe[0])
							{
								pickupableInLeftHand.SwapLeftHand();
								pickupable.SwapLeftHand();	
							}
							else
							{
								pickupableInLeftHand.InteractLeftHand();
							}
						}

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
//			 else if (pickupable != null && pickupableInRightHand != null && targetDropzone != null)
//			 {
//				 if (!targetDropzone.isOccupied)
//				 {
//					 pickupableInRightHand.dropPos = targetDropzone.transform.position;
//					 pickupableInRightHand.targetDropzone = targetDropzone;
//					 pickupableInRightHand.InteractRightHand();
//				 }
//			 }
			 else if (pickupable != null && pickupableInRightHand != null && targetDropzone != null){ //swap
				if(dropPos != Vector3.zero){
					if (targetDropzone.GetComponentInParent<Coaster>() == null)
					{
						pickupableInRightHand.dropPos = dropPos;
						pickupableInRightHand.targetDropzone = targetDropzone;
						if (pickupableInRightHand.targetDropzone.objectsInMe.Count > 0)
						{
							if (pickupable.gameObject == pickupableInRightHand.targetDropzone.objectsInMe[0])
							{
								pickupableInRightHand.SwapRightHand();
								pickupable.SwapRightHand();
							}
						}
						else
						{
							pickupableInRightHand.InteractRightHand(); //else just drop
						}
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
			if (pickupableInLeftHand != null && pickupable != null){ 
 				if (pickupable.GetComponent<Glass>() != null && pickupableInLeftHand.GetComponent<Bottle>() != null)
				{
//					Debug.Log("i_useLeft only functions are getting called!");
					Bottle bottle = pickupableInLeftHand.GetComponent<Bottle>();
					Glass glass = pickupable.GetComponent<Glass>();
					_startPourDelegate = glass.ReceivePourFromBottle;
					if (i_startUseLeft)
					{
 						startPourCoroutine = UtilCoroutines.WaitThenPour(bottle.tweenTime, glass.ReceivePourFromBottle, bottle, 0);
						StartCoroutine(startPourCoroutine);					
						bottle.StartPourTween(Vector3.forward + new Vector3(-0.64f, 0, 0.5f));
						bottle.RotateTween(bottle.leftHandPourRot);
					}
				}
			}
		} 
		
		if(i_endUseLeft){
			if(pickupableInLeftHand != null){
				if (pickupableInLeftHand.GetComponent<Bottle>() != null)
				{
					Bottle bottle = pickupableInLeftHand.GetComponent<Bottle>();
					foreach (var sequence in bottle.tweenSequences)
					{
						sequence.Kill(false);
					}
 				}
				pickupableInLeftHand.RotateToZeroTween();
				pickupableInLeftHand.EndPourTween();
				StopCoroutine(startPourCoroutine);
				StartCoroutine(UtilCoroutines.WaitThenSetTweensToInactive(pickupableInLeftHand.tweenEndTime, _tweenManagerDelegate));
 				if (pickupable != null)
				{
					if (pickupable.GetComponent<Glass>() != null)
					{
						pickupable.GetComponent<Glass>().EndPourFromBottle();
					}
				}
			}
		} 
		
//		if(i_useLeft && !Services.TweenManager.tweensAreActive){
//			//one-handed use on something on bar
//			if (pickupableInLeftHand != null && pickupable != null && pickupableInRightHand == null){ 
//				pickupableInLeftHand.UseLeftHand();
//			} 
//			
//			else if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use (left hand)
//				if (pickupableInLeftHand.GetComponent<Bottle>() != null && pickupableInRightHand.GetComponent<Glass>() != null) { //BOTTLE : GLASS
//					pickupableInLeftHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
//					pickupableInRightHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInLeftHand.GetComponent<Bottle>(), 1);
//				} else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Bottle>() != null){ //BOTTLE : BOTTLE
//					pickupableInLeftHand.UseLeftHand();				
//				}
//			}
//		
////			if (pickupableInLeftHand != null && pickupableInRightHand != null) { //two-handed use (right hand)
////                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
//// 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
////					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>(), 0);
////                } else if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Rag>() != null){
//// 				} 
////			}	  
//		} 
		
//		if(i_endUseLeft){
//			if(pickupableInLeftHand != null){
//				pickupableInLeftHand.RotateToZeroTween();
//				pickupableInLeftHand.EndPourTween();
//				if (pickupable != null)
//				{
//					if (pickupable.GetComponent<Glass>() != null)
//					{
//						pickupable.GetComponent<Glass>().EndPourFromBottle();
//					}
//				}
//			}
//			if (pickupableInLeftHand != null && pickupableInRightHand != null){
//				pickupableInLeftHand.RotateToZeroTween();
//				pickupableInRightHand.RotateToZeroTween();
//				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
//					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
//					pickupableInLeftHand.GetComponent<Glass>().EndPourTween();
//					pickupableInRightHand.EndPourTween();
//
//				}
//				if(pickupableInRightHand.GetComponent<Glass>() != null){	
//					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
//					pickupableInRightHand.GetComponent<Glass>().EndPourTween();
//					pickupableInLeftHand.EndPourTween();
//				}
//			}
//		} 
		
		#endregion

		#region Use Right
		if(i_useRight && !Services.TweenManager.tweensAreActive){
			//one-handed use on something on bar
			if (pickupableInRightHand != null && pickupable != null){ 
//				pickupableInLeftHand.UseLeftHand();
				if (pickupable.GetComponent<Glass>() != null && pickupableInRightHand.GetComponent<Bottle>() != null)
				{	
 					Bottle bottle = pickupableInRightHand.GetComponent<Bottle>();
					Glass glass = pickupable.GetComponent<Glass>();
					_startPourDelegate = glass.ReceivePourFromBottle;
					if (i_startUseRight)
					{
						startPourCoroutine = UtilCoroutines.WaitThenPour(bottle.tweenTime, glass.ReceivePourFromBottle, bottle, 0);
						StartCoroutine(startPourCoroutine);					
						bottle.StartPourTween(Vector3.forward + new Vector3(0.64f, 0, 0.5f));
						bottle.RotateTween(bottle.rightHandPourRot);
					}
//					glass.ReceivePourFromBottle(bottle, 1);
//					bottle.StartPourTween(Vector3.forward + new Vector3(0.64f, 0, 0.5f));
//					bottle.RotateTween(bottle.rightHandPourRot);
				}
			}
		} 
		if(i_endUseRight){
			if(pickupableInRightHand != null){
				if (pickupableInRightHand.GetComponent<Bottle>() != null)
				{
					Bottle bottle = pickupableInRightHand.GetComponent<Bottle>();
					foreach (var sequence in bottle.tweenSequences)
					{
						sequence.Kill(false);
					}
				}
				pickupableInRightHand.RotateToZeroTween();
				pickupableInRightHand.EndPourTween();
				StopCoroutine(startPourCoroutine);
				StartCoroutine(UtilCoroutines.WaitThenSetTweensToInactive(pickupableInRightHand.tweenEndTime, _tweenManagerDelegate));

				if (pickupable != null)
				{
					if (pickupable.GetComponent<Glass>() != null)
					{
						pickupable.GetComponent<Glass>().EndPourFromBottle();
					}
				}
			}
		} 

//		if(i_useRight && !Services.TweenManager.tweensAreActive){
//			//one-handed use on something on bar
//			if (pickupableInRightHand != null && pickupable != null && pickupableInLeftHand == null){
//				pickupableInRightHand.UseRightHand();				
//			}  
//			//two-handed use (bottle in left hand, glass in right)
//			else if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
//                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Bottle>() != null){ //Dual bottle
//	                pickupableInRightHand.UseRightHand();
//                 } 
//            }
//			//two-handed use (bottle in right hand, glass in left)
//			if (pickupableInLeftHand != null && pickupableInRightHand != null) { 
//                if (pickupableInRightHand.GetComponent<Bottle>() != null && pickupableInLeftHand.GetComponent<Glass>() != null) {
// 					pickupableInRightHand.GetComponent<Bottle>().PourIntoPickedUpGlass();
//					pickupableInLeftHand.GetComponent<Glass>().ReceivePourFromBottle(pickupableInRightHand.GetComponent<Bottle>(), 0);
//                } 
//            } 
//		} 
//		if(i_endUseRight){
//			if(pickupableInRightHand != null){
//				pickupableInRightHand.RotateToZeroTween();
//				pickupableInRightHand.EndPourTween();
//				if (pickupable != null)
//				{
//					if (pickupable.GetComponent<Glass>() != null)
//					{
//						pickupable.GetComponent<Glass>().EndPourFromBottle();
//					}
//				}
//			}
//			if (pickupableInLeftHand != null && pickupableInRightHand != null){
//				pickupableInLeftHand.RotateToZeroTween();
//				pickupableInRightHand.RotateToZeroTween();
//				if(pickupableInLeftHand.GetComponent<Glass>() != null){	
//					pickupableInLeftHand.GetComponent<Glass>().EndPourFromBottle();
//					pickupableInLeftHand.GetComponent<Glass>().EndPourTween();
//					pickupableInRightHand.EndPourTween();
//				}
//				if(pickupableInRightHand.GetComponent<Glass>() != null){	
//					pickupableInRightHand.GetComponent<Glass>().EndPourFromBottle();
//					pickupableInRightHand.GetComponent<Glass>().EndPourTween();
//					pickupableInLeftHand.EndPourTween();
// 				}
//			}
//		} 
		#endregion

		#region Two-handed Interactions

		if (i_useLeft && i_useRight && !Services.TweenManager.tweensAreActive)
		{
			switch (_interactionState)
			{
				case InteractionState.LeftHasBottle_RightHasGlass:
 					Bottle bottle = pickupableInLeftHand.GetComponent<Bottle>();
					Glass glass = pickupableInRightHand.GetComponent<Glass>();
					glass.ReceivePourFromBottle(bottle, 1);
					bottle.StartPourTween(Vector3.forward + new Vector3(-0.482f, 1.5f, 0.5f));
					bottle.RotateTween(bottle.leftHandPourRot);
					break;
				case InteractionState.LeftHasGlass_RightHasBottle:
 					Glass glass0 = pickupableInLeftHand.GetComponent<Glass>();
					Bottle bottle0 = pickupableInRightHand.GetComponent<Bottle>();
					glass0.ReceivePourFromBottle(bottle0, 0);
					bottle0.StartPourTween(Vector3.forward + new Vector3(0.482f, 1.5f, 0.5f));
					bottle0.RotateTween(bottle0.rightHandPourRot);
					break;
				default:
					break;
			}
		}
		else if (i_useLeft && i_endUseRight)
		{
			switch (_interactionState)
			{
				case InteractionState.LeftHasBottle_RightHasGlass:
					Bottle bottle = pickupableInLeftHand.GetComponent<Bottle>();
					Glass glass = pickupableInRightHand.GetComponent<Glass>();
					bottle.RotateToZeroTween();
					bottle.EndPourTween();
					glass.EndPourFromBottle();
					glass.EndPourTween();
					break;
				case InteractionState.LeftHasGlass_RightHasBottle:
					Glass glass0 = pickupableInLeftHand.GetComponent<Glass>();
					Bottle bottle0 = pickupableInRightHand.GetComponent<Bottle>();
					bottle0.RotateToZeroTween();
					bottle0.EndPourTween();
					glass0.EndPourFromBottle();
					glass0.EndPourTween();
					break;
				default:
					break;
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

			if (sink != null)
			{
				if (pickupableInLeftHand != null)
				{
					if (pickupableInLeftHand.GetComponent<Glass>() != null)
					{
						Glass leftHandGlass = pickupableInLeftHand.GetComponent<Glass>();
						leftHandGlass.EmptyGlass();
					}
				}

				if (pickupableInRightHand != null)
				{
					if (pickupableInRightHand.GetComponent<Glass>() != null)
					{
						Glass rightHandGlass = pickupableInRightHand.GetComponent<Glass>();
						rightHandGlass.EmptyGlass();
					}
				}
			}
			
			if (backdoor != null)
			{
 				Services.GameManager.dayManager.doorOpened = true;
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
				dropPos = hitObj.transform.parent.position;
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
//			Debug.Log(hitObj);
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

			if (hitObj.GetComponent<Sink>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxInteractionDist)
			{
				sink = hitObj.GetComponent<Sink>();
			} else if (hitObj.GetComponent<Sink>() == null)
			{
				sink = null;
			}

			if (hitObj.GetComponent<Backdoor>() != null &&
			    Vector3.Distance(transform.position, hitObj.transform.position) <= 6f)
			{
				backdoor = hitObj.GetComponent<Backdoor>();
			} else if (hitObj.GetComponent<Backdoor>() == null)
			{
				backdoor = null;
			}
		} else {
			npc = null;
			lightSwitch = null;
			sink = null;
			backdoor = null;
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

