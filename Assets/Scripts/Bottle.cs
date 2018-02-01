using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bottle : Pickupable {

	public DrinkBase myDrinkBase;
  	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);
	void Start(){
	}
	public override void UseLeftHand(){ 
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
			base.RotateTween(leftHandPourRot);
		} 
	}

	public override void UseRightHand(){
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
			base.RotateTween(rightHandPourRot);
		} 
	}

	public void PourIntoPickedUpGlass(){
		if(Services.GameManager.playerInput.pickupableInLeftHand == this){ //if the bottle is in the left hand
			base.RotateTween(leftHandPourRot);
		} else {
			base.RotateTween(rightHandPourRot);
		}
		// Services.GameManager.playerInput.pickupableInRightHand	
	}

}
