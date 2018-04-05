﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ice : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TweenToLeftGlass(){
		if(Services.GameManager.playerInput.pickupableInLeftHand != null){
			transform.SetParent(Services.GameManager.playerInput.pickupableInLeftHand.transform);
			// transform.localPosition = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOLocalRotate(Vector3.zero, 0.75f));
			sequence.Append(transform.DOLocalMove(Vector3.zero + new Vector3 (0, 0.25f, 0), 0.5f, false).SetEase(Ease.InSine));
		}
	}

	public void TweenToRightGlass(){
		if(Services.GameManager.playerInput.pickupableInRightHand != null){
			transform.SetParent(Services.GameManager.playerInput.pickupableInRightHand.transform);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOLocalRotate(Vector3.zero, 0.75f));
			sequence.Append(transform.DOLocalMove(Vector3.zero + new Vector3 (0, 0.25f, 0), 0.5f, false).SetEase(Ease.InSine));
		}
	}

}
