using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : Pickupable {
	public bool hasLiquid;
	public bool isFull;
	public bool isDirty;

	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);

	public void ReceivePourFromBottle(){
		if(Services.GameManager.playerInput.pickupableInLeftHand == this){ //if the bottle is in the left hand
			base.RotateTween(leftHandPourRot);
		} else {
			base.RotateTween(rightHandPourRot);
		}
		// Services.GameManager.playerInput.pickupableInRightHand	
	}
}
