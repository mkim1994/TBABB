using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour
{

    [SerializeField] protected bool isForDropzoneOnly;
     private Vector3 leftHandPos = new Vector3 (-1.022f, -0.25f, 1.241f);
    protected Vector3 dropOffset;
     private Vector3 rightHandPos = new Vector3 (0.954f, -0.25f, 1.473f);
    public Dropzone targetDropzone;
    public Vector3 startPos;
    public Vector3 dropPos;
    public Vector3 dropzoneOffset;
    public float tweenTime;
    public float pickupDropTime;
    public bool pickedUp = false;

    [SerializeField] private Dropzone myChildDropzone;
 
    public string myName = "";

    public Liquid myLiquid;

    protected virtual void Start()
    {
        CreateDropzone();
    }

    public void CreateDropzone()
    {
        GameObject dropzoneGO = Instantiate(Resources.Load("Prefabs/dropzone"), transform.position + dropzoneOffset, Quaternion.identity) as GameObject;
        myChildDropzone = dropzoneGO.GetComponent<Dropzone>();
        if (isForDropzoneOnly)
        {
             Destroy(gameObject);
        } else if (!isForDropzoneOnly)
        {
            dropzoneGO.GetComponent<Dropzone>().isOccupied = true;
        }
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
        sequence.Append(transform.DOLocalMove(moveToPos, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToFirstPersonLayer(pickupDropTime));
        pickedUp = true;
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
    }

    public virtual void RotateTween(Vector3 rotation){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(rotation, tweenTime, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
    }

    public virtual void RotateToZeroTween(){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(Vector3.zero, tweenTime, RotateMode.Fast));
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
//            targetDropzone.isOccupied = false;
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
            transform.GetChild(i).gameObject.layer = 12;
        }

    }

    protected virtual void ResetEvaluationStatus()
    {
        
    }

    private void DoThisWhenYouDie()
    {
        myChildDropzone.objectsInMe.Clear();
        myChildDropzone.isOccupied = false; //set my dropzone's isOccupied to false so it can take object drops
    }

    private void OnDestroy()
    {
        DoThisWhenYouDie();   
    }
}
