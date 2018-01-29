using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerInteraction : MonoBehaviour {
	public LayerMask layerMask;
	Pickupable leftHandObj;
	Pickupable rightHandObj;
	Pickupable pickupable;

	float maxInteractionDist = 4f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		InteractionRay();
		if(Input.GetKeyDown(KeyCode.Q)){
			PickSomethingUp();
		} 

		if(Input.GetKeyDown(KeyCode.E)){
			PickSomethingUp();
		}
	}

	void PickSomethingUp(){
		if(pickupable != null){
			pickupable.transform.SetParent(this.transform);
			// pickupable.transform.DOMove(transform.forward, 0.25f, false);
		}
	}

	void InteractionRay(){
		Ray ray = new Ray(transform.position, transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();

		if(Physics.Raycast(ray, out hit, rayDist, layerMask)){
			//if you're actually looking at something
			GameObject hitObj = hit.transform.gameObject;
 			//check if object looked at can be picked up
			if(hitObj.GetComponent<Pickupable>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= maxInteractionDist){
				//if the object you're looking at is close enough AND is an interactable, assign it to interactableCIRAL@. 
				//BUT ONLY IF YOU CAN ACTUALLY PICK IT UP.
				pickupable = hitObj.GetComponent<Pickupable>(); 
				  
 			} 		
		} else {
			//if you're not looking at anything, make this null.
			pickupable = null;
 			//no hit
 		}


	}
}
