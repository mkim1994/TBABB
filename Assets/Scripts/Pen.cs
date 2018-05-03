﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pen : Pickupable
{

	[SerializeField] private Notepad _notepad;
	private PlayerInput _player;
	[SerializeField]private Vector3 _leftHandStartPos;
	[SerializeField]private Vector3 _rightHandStartPos;
	[SerializeField]private Vector3 _writeLeftHandPos;
	[SerializeField]private Vector3 _writeRightHandPos;

	[SerializeField]private Vector3 _leftHandStartRot;
	[SerializeField]private Vector3 _writeLeftHandRot;
	[SerializeField]private Vector3 _rightHandStartRot;
	[SerializeField]private Vector3 _writeRightHandRot;

	protected override void Start(){
		base.Start();
		_player = Services.GameManager.playerInput;
	}

	public void WriteLeftHanded(){
		Sequence w = DOTween.Sequence();
		w.Append(transform.DOLocalMove(_writeLeftHandPos, 1f, false));
		w.Append(transform.DOLocalMove(_leftHandStartPos, 0.75f, false));
		Sequence r = DOTween.Sequence();
		r.Append(transform.DOLocalRotate(_writeLeftHandRot, 1f));
		r.Append(transform.DOLocalRotate(_leftHandStartRot, 0.75f));
	}
	
	public void WriteRightHanded(){
		Sequence w = DOTween.Sequence();
		w.Append(transform.DOLocalMove(_writeRightHandPos, 1f, false));
		w.Append(transform.DOLocalMove(_rightHandStartPos, 0.75f, false));
		Sequence r = DOTween.Sequence();
		r.Append(transform.DOLocalRotate((_writeRightHandRot), 1f));
		r.Append(transform.DOLocalRotate(_rightHandStartRot, 0.75f));
	}
	
	public override void InteractLeftHand(){
		if(!pickedUp){
			//pick up with left hand
			transform.SetParent(Services.GameManager.player.transform.GetChild(0));
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
			PickupTween(_leftHandStartPos, _leftHandStartRot);
		} else if(pickedUp){
			transform.SetParent(null);
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
 
			if(targetDropzone != null){
				DropTween(dropPos, dropOffset, targetDropzone);
			}
		}
	}

	public override void InteractRightHand(){
		if(!pickedUp){
			transform.SetParent(Services.GameManager.player.transform.GetChild(0));
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
			PickupTween(_rightHandStartPos, _rightHandStartRot);
		} else if(pickedUp){
			transform.SetParent(null);
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            
			if(targetDropzone != null){
				DropTween(dropPos, dropOffset, targetDropzone);
			}
		}
	}
	
	public override void PickupTween(Vector3 moveToPos, Vector3 startRotation){
		DeclareActiveTween();
//         if(targetDropzone != null){
//             targetDropzone.isOccupied = false;
//         }
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, pickupDropTime, false));
		transform.DOLocalRotate(startRotation, pickupDropTime, RotateMode.FastBeyond360);
		sequence.OnComplete(() => DeclareInactiveTween());
		// Debug.Log("Pickup Tween called!");
		StartCoroutine(ChangeToFirstPersonLayer(pickupDropTime));
		pickedUp = true;
		tweenSequences.Add(sequence);
	}
}