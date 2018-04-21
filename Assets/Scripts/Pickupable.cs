﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour
{
    public bool isForDropzoneOnly;   
    public float pickupDropTime;
    protected float offsetZ = 2f;
    protected float offsetY = -1.5f;
    public Vector3 leftHandPos = new Vector3 (-0.64f, -0.89f, 2f);
    // public Vector3 leftHandPos = new Vector3 (-0.64f, -0.89f, 1.241f);
    protected Vector3 dropOffset;
    public Vector3 rightHandPos = new Vector3 (0.64f, -0.89f, 2f);
    // public Vector3 rightHandPos = new Vector3 (0.64f, -0.89f, 1.473f);
    public Dropzone targetDropzone;
    public Vector3 startPos;
    public Vector3 dropPos;
    public float tweenTime;
    public float tweenEndTime;
    public bool pickedUp = false;
    public List<Sequence> tweenSequences = new List<Sequence>();
    public Vector3 origPos;
    [SerializeField] private Dropzone myChildDropzone;
     
    protected virtual void Start()
    {
        CreateDropzone();
        origPos = transform.position;
        EventManager.Instance.Register<DayEndEvent>(ReturnHome);
    }


    public virtual void Update(){
        if(pickedUp && !Services.TweenManager.tweensAreActive){
            // transform.rotation = Quaternion.identity;
            // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, offsetZ);
         }
    }
    public void CreateDropzone()
    {
        GameObject dropzoneGO = Instantiate(Resources.Load("Prefabs/dropzoneParent"), transform.position, Quaternion.identity) as GameObject;
        myChildDropzone = dropzoneGO.GetComponentInChildren<Dropzone>();
        if (isForDropzoneOnly)
        {
            SetupDropzoneThenDestroySelf();
        } else if (!isForDropzoneOnly)
        {
            myChildDropzone.isOccupied = true;
        }
    }

    public virtual void InteractLeftHand(){
        if(!pickedUp){
            //pick up with left hand
            Debug.Log("Interact Left Hand called in Pickupable");
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
            Debug.Log("Swap called!");
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
//         if(targetDropzone != null){
//             targetDropzone.isOccupied = false;
//         }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(moveToPos, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        Debug.Log("Pickup Tween called!");
        StartCoroutine(ChangeToFirstPersonLayer(pickupDropTime));
        pickedUp = true;
        tweenSequences.Add(sequence);
    }

    public virtual void DropTween(Vector3 dropPos, Vector3 dropOffset, Dropzone _targetDropzone){
        DeclareActiveTween();
        _targetDropzone.isOccupied = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToWorldLayer(pickupDropTime));
        pickedUp = false;
        tweenSequences.Add(sequence);
    }

     public virtual void MoveTween(Vector3 dropPos){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToWorldLayer(pickupDropTime));
        pickedUp = false;
        tweenSequences.Add(sequence);
    }

    public virtual void RotateTween(Vector3 rotation){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(rotation, tweenTime, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
        tweenSequences.Add(sequence);
    }

    public virtual void RotateToZeroTween(){
        DeclareActiveTween();
        Debug.Log("Rotating to zero!");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(Vector3.zero, tweenEndTime, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
        tweenSequences.Add(sequence);
    } 

    public void DeclareActiveTween(){
        Services.TweenManager.tweensAreActive = true;
    }
    
    public virtual void DeclareInactiveTween(){
        Debug.Log("Declare inactive tween called!");
        Services.TweenManager.tweensAreActive = false;
    }

    public static void DeclareAllTweensInactive()
    {
        Services.TweenManager.tweensAreActive = false;
    }

    public void DeclareSpecificInactiveTween(Sequence _sequence)
    {
        Services.TweenManager.tweensAreActive = false;
    }

    public void SetPickedUpToTrue(){
        pickedUp = true;
    }

    public virtual IEnumerator ChangeToFirstPersonLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
        startPos = transform.localPosition;
//        if (targetDropzone != null)
//        {
//            targetDropzone.isOccupied = false;
//        }
        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
             Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;
        }
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 13;
    
//            grandchildren.transform.GetChild(i).gameObject.layer = 13;

        }
    }

    public virtual IEnumerator ChangeToWorldLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        startPos = transform.localPosition;
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 12;
        }

    }

    protected virtual void ResetEvaluationStatus()
    {
        
    }

    private void SetupDropzoneThenDestroySelf()
    {
        myChildDropzone.objectsInMe.Clear();
        myChildDropzone.isOccupied = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister<DayEndEvent>(ReturnHome);
    }

    public void ReturnHome(GameEvent e){
        DayEndEvent dayEndEvent = e as DayEndEvent;
        transform.position = origPos;
        pickedUp = false;
        transform.eulerAngles = Vector3.zero;
        StartCoroutine(ChangeToWorldLayer(1f));
        if(GetComponent<Glass>() != null){
            Glass glass = GetComponent<Glass>();
            glass.ClearIce();
            glass.hasIce = false;
            glass.liquid.isEvaluated = false;
            glass.liquid.EmptyLiquid();
               
            glass.liquid.myDrinkBase = DrinkBase.none;
            glass.liquid.myMixer = Mixer.none;
            // if (GetComponent<Bottle>() == null)
            // {
            //     glass.liquid.transform.localScale = new Vector3(0, 0, 0);
            // }
        }
    }
}

public class DayEndEvent : GameEvent{

}


