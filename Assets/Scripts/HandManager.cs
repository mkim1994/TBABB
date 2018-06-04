using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{

	[SerializeField]private Pickupable _seenPickupable;
	private Camera _myCamera;
	[SerializeField]private Hand _leftHand;
	[SerializeField]private Hand _rightHand;
	[HideInInspector]public float _maxInteractionDist = 4f;
	private Vector3 _dropPos;
	
	//bools
	[SerializeField]private bool _isInDropRange;

	public bool IsInDropRange
	{
		get { return _isInDropRange; }
		private set { _isInDropRange = value; }
	}

	// Use this for initialization
	void Start ()
	{
		_myCamera = Camera.main;
	}

	//layer masks
	[SerializeField]private LayerMask _layerMask;
	[SerializeField] private LayerMask _dropzoneLayerMask;
	
	// Update is called once per frame
	void Update ()
	{
		PickupableRay();
		DropzoneRay();
		_leftHand.OnUpdate();
		_rightHand.OnUpdate();
	}
	
	private void PickupableRay(){
		Ray ray = new Ray(_myCamera.transform.position, _myCamera.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, _layerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
//			 Debug.Log(hitObj);				  
			if(	hitObj.GetComponent<Pickupable>() != null && 
			   	Vector3.Distance(transform.position, hitObj.transform.position) <= _maxInteractionDist){ //check if object looked at can be picked up
				
				_seenPickupable = hitObj.GetComponent<Pickupable>(); //if it's Pickupable and close enough, assign it to _pickupable.
				_leftHand.SeenPickupable = _seenPickupable;
				_rightHand.SeenPickupable = _seenPickupable;
				
			} 
			else if (	hitObj.GetComponent<Pickupable>() == null || 
			           	Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist ){
							
				_seenPickupable = null;
				_leftHand.SeenPickupable = null;
				_rightHand.SeenPickupable = null;
			} 	
		}
	}

	private void DropzoneRay()
	{
		Ray ray = new Ray(_myCamera.transform.position, _myCamera.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast(ray, out hit, rayDist, _dropzoneLayerMask))
		{
			_leftHand.DropPos = hit.point;
			_rightHand.DropPos = hit.point;
			if (Vector3.Distance(hit.point, transform.position) <= _maxInteractionDist)
			{
				_isInDropRange = true;
			}
			else if (Vector3.Distance(hit.point, transform.position) > _maxInteractionDist)
			{
				_isInDropRange = false;
			}
		}
	}
}
