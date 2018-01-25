using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bottle : Pickupable {
	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);

	public override void UseLeftHand(){
		base.PourTween(leftHandPourRot);
	}

	public override void UseRightHand(){
		base.PourTween(rightHandPourRot);
	}

}
