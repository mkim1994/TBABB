using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class HandManager : MonoBehaviour
{
	
	[SerializeField]private Hand _leftHand;
	[SerializeField]private Hand _rightHand;

	public Hand LeftHand
	{
		get { return _leftHand; }
	}

	public Hand RightHand
	{
		get { return _rightHand; }
	}

	//behavior tree
	private Tree<HandManager> _tree;
	private FSM<HandManager> _fsm;
	
	[SerializeField]private Pickupable _seenPickupable;
	[SerializeField]private Glass _seenGlass;
	[SerializeField] private FirstPersonCharacter _firstPersonCharacter;

	public FirstPersonCharacter FirstPersonCharacter
	{
		get { return _firstPersonCharacter; }
	}

	public Glass SeenGlass
	{
		get { return _seenGlass; }
		private set { _seenGlass = value; }
	}

	private Camera _myCamera;
	
	[HideInInspector]public float _maxInteractionDist = 4f;
	private Vector3 _dropPos;
		
	//bools
	[SerializeField]private bool _isInDropRange;

	public bool IsInDropRange
	{
		get { return _isInDropRange; }
		private set { _isInDropRange = value; }
	}

	private bool _isLookingAtGlass;

	public bool IsLookingAtGlass
	{
		get { return _isLookingAtGlass; }
		set { _isLookingAtGlass = value; }
	}

	// Use this for initialization
	void Start ()
	{
		_myCamera = Camera.main;

		_tree = new Tree<HandManager>(new Selector<HandManager>()
		);
	}

	//layer masks
	[SerializeField]private LayerMask _pickupableLayerMask;
	[SerializeField] private LayerMask _dropLayerMask;
	
	// Update is called once per frame
	void Update ()
	{
		PickupableRay();
		DropRay();
		_leftHand.OnUpdate();
		_rightHand.OnUpdate();
		_tree.Update(this);
//		_fsm.Update();
	}
	
	private void PickupableRay(){
		Ray ray = new Ray(_myCamera.transform.position, _myCamera.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, _pickupableLayerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
//			 Debug.Log(hitObj);				  
			if(	hitObj.GetComponent<Pickupable>() != null && 
			   	Vector3.Distance(transform.position, hitObj.transform.position) <= _maxInteractionDist){ //check if object looked at can be picked up
				
				_seenPickupable = hitObj.GetComponent<Pickupable>(); //if it's Pickupable and close enough, assign it to _pickupable.
				_leftHand.SeenPickupable = _seenPickupable;
				_rightHand.SeenPickupable = _seenPickupable;
				
				//check what kind of pickupable it is
				if (_seenPickupable.GetComponent<Glass>() != null)
				{
					_isLookingAtGlass = true;
				}
			} 
			else if (	hitObj.GetComponent<Pickupable>() == null || 
			           	Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist ){
							
				_seenPickupable = null;
				_seenGlass = null;
				_leftHand.SeenPickupable = null;
				_rightHand.SeenPickupable = null;
			} 	
		}
	}

	private void DropRay()
	{
		Ray ray = new Ray(_myCamera.transform.position, _myCamera.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast(ray, out hit, rayDist, _dropLayerMask))
		{
//			Debug.Log(hit.transform.name);
			_leftHand.DropPos = hit.point;
			_rightHand.DropPos = hit.point;
			
			if (Vector3.Distance(hit.point, transform.position) <= _maxInteractionDist
			    && hit.transform.gameObject.layer == 9) //only set to true if object is of Dropzone Layer
			{
				_isInDropRange = true;				
			}
			else if (Vector3.Distance(hit.point, transform.position) > _maxInteractionDist
				 || hit.transform.gameObject.layer != 9) 
			{
				_isInDropRange = false;				
			}
		}
	}

//	private class IsLeftHandTweening : Node<HandManager>
//	{
//		public override bool Update(HandManager context)
//		{
//			
//			return false;
//		}
//	}

}
