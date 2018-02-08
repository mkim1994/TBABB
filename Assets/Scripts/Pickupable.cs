using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour {
    #region Left Hand Positions/Rotations
    private Vector3 leftHandPos = new Vector3 (-1.022f, -0.25f, 1.241f);
    protected Vector3 dropOffset = Vector3.zero;
    #endregion
    private Vector3 rightHandPos = new Vector3 (0.954f, -0.25f, 1.473f);
    public Dropzone targetDropzone;
    public Vector3 dropPos;
    public bool pickedUp = false;

    public string myName = "";
    // public List<Coaster> coasters = new List<Coaster> ();

    void Start(){
    }

    public virtual void InteractLeftHand(){
        if(!pickedUp){
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            PickupTween(leftHandPos);
        } else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            if(targetDropzone != null){
                 DropTween(dropPos,targetDropzone);
            }
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
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(leftHandPos);
        }
    }

    public virtual void SwapRightHand(){
        if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            DropTween(dropPos,targetDropzone);
            // }
            pickedUp = false;
        }
        else if(!pickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            // StartCoroutine(SetNewParent(0.1f));
            pickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(rightHandPos);
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
        pickedUp = true;
    }

    public virtual void DropTween(Vector3 dropPos, Dropzone _targetDropzone){
        DeclareActiveTween();
        _targetDropzone.isOccupied = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, 0.25f, false));
        transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        pickedUp = false;
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
        Services.TweenManager.tweensAreActive = true;
    }
    public void DeclareInactiveTween(){
        Services.TweenManager.tweensAreActive = false;
    }

    public void SetPickedUpToTrue(){
        pickedUp = true;
    }

    public string GetMyName(DrinkBase _base, Mixer _mixer){
        string _myName = "";
        if(_base != DrinkBase.none && _mixer == Mixer.none){
            switch (_base){
                case DrinkBase.beer:
                    _myName = "beer";
                break;
                case DrinkBase.brandy:
                    _myName = "brandy";
                break;
                case DrinkBase.gin:
                    _myName = "gin";
                break;
                case DrinkBase.rum:
                    _myName = "rum";
                break;
                case DrinkBase.tequila:
                    _myName = "tequila";
                break;
                case DrinkBase.vodka:
                    _myName = "vodka";
                break;
                case DrinkBase.whiskey:
                    _myName = "whiskey";
                break;
                case DrinkBase.wine:
                    _myName = "wine";
                break;
                default:
                break;
            }
        }

        if(_base == DrinkBase.none && _mixer != Mixer.none){
            switch (_mixer){
                case Mixer.tonic:
                    _myName = "tonic";
                break;
                case Mixer.soda:
                    _myName = "soda";
                break;
                case Mixer.orange_juice:
                    _myName = "orange juice";
                break;
                case Mixer.lemon_juice:
                    _myName = "lemon juice";
                break;
                case Mixer.apple_juice:
                    _myName = "apple juice";
                break;
                default:
                break;
            }
        }

        return _myName;
    }
       
}
