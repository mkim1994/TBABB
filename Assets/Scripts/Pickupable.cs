﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour {
    private Vector3 leftHandPos = new Vector3 (-1.022f, 0, 1.241f);
    private Vector3 rightHandPos = new Vector3 (0.954f, 0, 1.473f);
    public bool tweensAreActive = false;
    public bool pickedUp = false;
    public void InteractLeftHand(){
        if(!pickedUp)
        {
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            PickUpTween(leftHandPos);
        }
        else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            pickedUp = false;
        }
	}

    public void InteractRightHand(){
        if(!pickedUp)
        {
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            PickUpTween(rightHandPos);
        }
        else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            pickedUp = false;
        }
    }


    private void PickUpTween(Vector3 handPos)
    {
        transform.SetParent(Services.GameManager.player.transform);
        DeclareActiveTween();
        pickedUp = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(handPos, 0.25f, false));
        sequence.OnComplete(() => DeclareInactiveTween());
    }

    public void DeclareActiveTween(){
        tweensAreActive = true;
    }
    public void DeclareInactiveTween(){
        tweensAreActive = false;
    }

    public void SetPickedUpToTrue(){
        pickedUp = true;
    }
	
}
