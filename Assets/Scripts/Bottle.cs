using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bottle : Pickupable {

	public DrinkBase myDrinkBase;
	public Mixer myMixer;	
  	private Vector3 leftHandPourRot = new Vector3(80f, 25, 0);
	private Vector3 rightHandPourRot = new Vector3(80, -25, 6.915f);
 	
	protected override void Start()
	{
 		base.Start();
 	}
	public override void RotateTween(Vector3 rotation){
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalRotate(rotation, tweenTime, RotateMode.Fast)).SetEase(Ease.InOutBack);
		sequence.OnComplete(() => DeclareInactiveTween());
	} 
	
	public override void StartPourTween(Vector3 moveToPos)
	{
		DeclareActiveTween();
		Services.AudioLoopScript.playerAttackPour = true;
		Services.AudioLoopScript.playerAttackPour = false;
 		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, tweenTime, false)).SetEase(Ease.InOutQuart);
		sequence.OnComplete(() => DeclareInactiveTween());		
	}

	public override void EndPourTween()
	{
		DeclareActiveTween();
		Services.AudioLoopScript.isPlayerPouring = false;
 		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(startPos, tweenTime, false));
		sequence.OnComplete(() => DeclareInactiveTween());				
	}

	public override void UseLeftHand(){ 
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
 			base.RotateTween(leftHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(-0.482f, 0, 0.5f));
			Services.GameManager.playerInput.pickupable.GetComponent<Glass>().ReceivePourFromBottle(this, 0);
			Debug.Log("Pouring left!");
		} 
	}

	public override void UseRightHand(){
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
 			base.RotateTween(rightHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(0.482f, 0, 0.5f));
			Services.GameManager.playerInput.pickupable.GetComponent<Glass>().ReceivePourFromBottle(this, 1);
			Debug.Log("Pouring right!");
		} 
	}

	public void PourIntoPickedUpGlass(){
		if(Services.GameManager.playerInput.pickupableInLeftHand == this){ //if the bottle is in the left hand
			base.RotateTween(leftHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(-0.482f, 0, 0.5f));
 		} else if (Services.GameManager.playerInput.pickupableInRightHand == this){
			base.RotateTween(rightHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(0.482f, 0, 0.5f));
 		}
		// Services.GameManager.playerInput.pickupableInRightHand	
	}
	


}
