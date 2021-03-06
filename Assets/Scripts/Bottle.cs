﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bottle : Pickupable {

	public DrinkBase myDrinkBase;
	public Mixer myMixer;	
  	[HideInInspector]public Vector3 leftHandPourRot = new Vector3(80f, 25, 0);
	[HideInInspector]public Vector3 rightHandPourRot = new Vector3(80, -25, 6.915f);
 	public Vector3 rightHandPourPos;

	public Vector3 leftHandPourPos; 
	protected override void Start()
	{
		base.Start();
   	}

	public override void RotateTween(Vector3 rotation){
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalRotate(rotation, tweenTime, RotateMode.Fast)).SetEase(Ease.InOutBack);
//		sequence.OnComplete(() => DeclareInactiveTween());
		tweenSequences.Add(sequence);
	} 
	
	public override void StartPourTween(Vector3 moveToPos)
	{
		DeclareActiveTween();
		Services.AudioLoopScript.playerAttackPour = true;
		Services.AudioLoopScript.playerAttackPour = false;
 		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, tweenTime, false)).SetEase(Ease.InOutQuart);
		sequence.AppendCallback(()=>Services.GameManager.playerInput.isPourTweenDone = true);
//		sequence.OnComplete(() => DeclareInactiveTween());		
		tweenSequences.Add(sequence);
	}

	public override void EndPourTween()
	{
		DeclareActiveTween();
		Services.AudioLoopScript.isPlayerPouring = false;
 		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(startPos, tweenEndTime, false)).SetEase(Ease.InOutSine);
		sequence.AppendCallback(() => Services.GameManager.playerInput.isPourTweenDone = false);
//		sequence.OnComplete(() => Services.GameManager.playerInput.isPouringTweenStarting = false);
//		sequence.OnComplete(() => DeclareInactiveTween());			
		tweenSequences.Add(sequence);
	}

	public override void UseLeftHand(){ 
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
 			base.RotateTween(leftHandPourRot);
			StartPourTween(leftHandPourPos);
//			Services.GameManager.playerInput.pickupable.GetComponent<Glass>().ReceivePourFromBottle(this, 0);
 		} 
	}

	public override void UseRightHand(){
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
 			base.RotateTween(rightHandPourRot);
			StartPourTween(rightHandPourPos);
//			Services.GameManager.playerInput.pickupable.GetComponent<Glass>().ReceivePourFromBottle(this, 1);
 		} 
	}

	public void PourIntoPickedUpGlass(){
		if(Services.GameManager.playerInput.pickupableInLeftHand == this){ //if the bottle is in the left hand
			base.RotateTween(leftHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(-0.482f, 1.5f, 0.5f));
 		} else if (Services.GameManager.playerInput.pickupableInRightHand == this){
			base.RotateTween(rightHandPourRot);
			StartPourTween(Vector3.forward + new Vector3(0.482f, 1.5f, 0.5f));
 		}
		// Services.GameManager.playerInput.pickupableInRightHand	
	}
	


}
