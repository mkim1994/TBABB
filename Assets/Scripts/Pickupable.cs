using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour {
    #region Left Hand Positions/Rotations
    private Vector3 leftHandPos = new Vector3 (-1.022f, -0.25f, 1.241f);
    
    #endregion
    private Vector3 rightHandPos = new Vector3 (0.954f, -0.25f, 1.473f);
    public Dropzone targetDropzone;
    public bool tweensAreActive = false;

    public Vector3 dropPos;
    public bool pickedUp = false;

    void Start(){
    }

    private void TestMessage(){
        Debug.Log("My name is " + this.name);
    }
    public virtual void InteractLeftHand(){
        if(!pickedUp){
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            pickedUp = true;
            PickupTween(leftHandPos);
        } else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            if(targetDropzone != null){
                 DropTween(dropPos,targetDropzone);
            }
            pickedUp = false;
        }
	}

    public virtual void InteractRightHand(){
        if(!pickedUp){
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            PickupTween(rightHandPos);
        }
        else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            if(targetDropzone != null){
                DropTween(dropPos, targetDropzone);
            }
            pickedUp = false;
        }
    }

    public virtual void SwapLeftHand(){
        if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            // if(targetDropzone != null){
                DropTween(dropPos,targetDropzone);
            // }
            pickedUp = false;
        }
        else if(!pickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            // StartCoroutine(SetNewParent(0.1f));
            pickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(leftHandPos);
        }
    }

    public virtual void UseLeftHand(){        
    }

    public virtual void UseRightHand(){
    }


    public virtual void PickupTween(Vector3 moveToPos){
         DeclareActiveTween();
        // if(targetDropzone != null){
        //     targetDropzone.isOccupied = false;
        // }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(moveToPos, 0.25f, false));
        transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
    }

    public virtual void DropTween(Vector3 dropPos, Dropzone _targetDropzone){
        DeclareActiveTween();
        _targetDropzone.isOccupied = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos, 0.25f, false));
        transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
    }

    public virtual void RotateTween(Vector3 rotation){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(rotation, 0.25f, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
    } 

    public virtual void RotateToZeroTween(){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
    } 

    public void DeclareActiveTween(){
        Debug.Log("A tween is active!");
        tweensAreActive = true;
        Services.TweenManager.tweensAreActive = true;
    }
    public void DeclareInactiveTween(){
        Debug.Log("Tween now inactive!");
        tweensAreActive = false;
        Services.TweenManager.tweensAreActive = false;
    }

    public void SetPickedUpToTrue(){
        pickedUp = true;
    }
	
}
