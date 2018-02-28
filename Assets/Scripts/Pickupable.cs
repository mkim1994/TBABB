using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour {
    #region Left Hand Positions/Rotations
    private Vector3 leftHandPos = new Vector3 (-1.022f, -0.25f, 1.241f);
    protected Vector3 dropOffset;
    #endregion
    private Vector3 rightHandPos = new Vector3 (0.954f, -0.25f, 1.473f);
    public Dropzone targetDropzone;
    public Vector3 startPos;
    public Vector3 dropPos;
    public Vector3 dropzoneOffset;
    public bool pickedUp = false;
 
    public string myName = "";

    public Liquid myLiquid;
    // public List<Coaster> coasters = new List<Coaster> ();

    protected virtual void Start(){
        if (gameObject.GetComponent<Bottle>() != null)
        {
            dropzoneOffset = new Vector3(0, 0.05f, 0);
            CreateDropzone();
        } else if (gameObject.GetComponent<Glass>() != null)
        {
            dropzoneOffset = new Vector3(0, 0.15f, 0);
            CreateDropzone();
        }
    }

    protected virtual void CreateDropzone()
    {
        GameObject dropzoneGO = Instantiate(Resources.Load("Prefabs/dropzone"), transform.position + dropzoneOffset, Quaternion.identity) as GameObject;
        dropzoneGO.GetComponent<Dropzone>().isOccupied = true;
    }

    public virtual void InteractLeftHand(){
        if(!pickedUp){
            //pick up with left hand
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            PickupTween(leftHandPos);
        } else if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
 
            if(targetDropzone != null){
                 DropTween(dropPos, dropOffset, targetDropzone);
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
                DropTween(dropPos, dropOffset, targetDropzone);
            }
        }
    }

    public virtual void SwapLeftHand(){
        if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            DropTween(dropPos, dropOffset, targetDropzone);
            pickedUp = false;
        }
        else if(!pickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            pickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(leftHandPos);
        }
    }

    public virtual void SwapRightHand(){
        if(pickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            DropTween(dropPos, dropOffset, targetDropzone);
            pickedUp = false;
        }
        else if(!pickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            pickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(rightHandPos);
        }
    }

    public virtual void UseLeftHand(){        
    }

    public virtual void UseRightHand(){
    }

    public virtual void StartPourTween(Vector3 moveToPos)
    {
        
    }

    public virtual void EndPourTween()
    {
        
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
        StartCoroutine(ChangeToFirstPersonLayer(0.25f));
        pickedUp = true;
    }

    public virtual void DropTween(Vector3 dropPos, Vector3 dropOffset, Dropzone _targetDropzone){
        DeclareActiveTween();
        _targetDropzone.isOccupied = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, 0.25f, false));
        transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToWorldLayer(0.25f));
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

    IEnumerator ChangeToFirstPersonLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
        startPos = transform.localPosition;
        if (targetDropzone != null)
        {
            targetDropzone.isOccupied = false;
        }
        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
             Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;
        }
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 13;        
        }
    }

    IEnumerator ChangeToWorldLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        startPos = transform.localPosition;
         int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 0;
        }

    }

    protected virtual void ResetEvaluationStatus()
    {
        
    }
}
