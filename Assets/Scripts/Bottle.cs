using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bottle : Pickupable {
	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);

	public override void UseLeftHand(){
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
			base.PourTween(leftHandPourRot);
		} else {
			//Do nothing
		}
	}

	public override void UseRightHand(){
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
			base.PourTween(rightHandPourRot);
		} else {
			//Do nothing
		}
	}

}
