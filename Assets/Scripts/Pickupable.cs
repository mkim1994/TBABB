using System.Collections;
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
    public bool PickedUp = false;
    public List<Sequence> tweenSequences = new List<Sequence>();
    public Vector3 origPos;
    private Dropzone myChildDropzone;
      
    protected virtual void Start()
    {
        CreateDropzone();
        origPos = transform.position;
//        EventManager.Instance.Register<DayEndEvent>(ReturnHome);
     }


    public virtual void Update(){
//        if(pickedUp && !Services.TweenManager.tweensAreActive){
//            // transform.rotation = Quaternion.identity;
//            // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, offsetZ);
//         }
        if (PickedUp)
        {
            ChangeLayerToFirstPerson();
        }
        else
        {
            ChangeLayerToWorld();
        }
    }

    public void CreateDropzone()
    {
//        GameObject dropzoneGO = Instantiate(Resources.Load("Prefabs/dropzoneParent"), transform.position, Quaternion.identity) as GameObject;
//        myChildDropzone = dropzoneGO.GetComponentInChildren<Dropzone>();
        if (isForDropzoneOnly)
        {
            SetupDropzoneThenDestroySelf();
        } else if (!isForDropzoneOnly)
        {
//            myChildDropzone.isOccupied = true;
        }
    }

    public virtual void InteractLeftHand(){
        if(!PickedUp){
            //pick up with left hand
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            PickupTween(leftHandPos, Vector3.zero);            
        } else if(PickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
 
            if(targetDropzone != null){
                 DropTween(dropPos, dropOffset, targetDropzone);
            }
         }
	}

    public virtual void InteractRightHand(){
        if(!PickedUp){
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            PickupTween(rightHandPos, Vector3.zero);            
        } else if(PickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            if(targetDropzone != null){
                DropTween(dropPos, dropOffset, targetDropzone);
            }
        }
    }

    public virtual void SwapLeftHand(){
        if(PickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
            DropTween(dropPos, dropOffset, targetDropzone);               
            PickedUp = false;
        }
        else if(!PickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
            PickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(leftHandPos, Vector3.zero);
        }
    }

    public virtual void SwapRightHand(){
        if(PickedUp){
            transform.SetParent(null);
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            DropTween(dropPos, dropOffset, targetDropzone);             
            PickedUp = false;
        }
        else if(!PickedUp){
            Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
            PickedUp = true;
            transform.SetParent(Services.GameManager.player.transform.GetChild(0));
            PickupTween(rightHandPos, Vector3.zero);
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

    public virtual void PickupTween(Vector3 moveToPos, Vector3 pickupRot){
         DeclareActiveTween();
//         if(targetDropzone != null){
//             targetDropzone.isOccupied = false;
//         }
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(moveToPos, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
//        sequence.AppendCallback(() => GetComponent<Collider>().enabled = false);
        sequence.OnComplete(() => DeclareInactiveTween());
        // Debug.Log("Pickup Tween called!");
        StartCoroutine(ChangeToFirstPersonLayer(pickupDropTime));
        PickedUp = true;
        if (targetDropzone != null)
        {
            targetDropzone.isOccupied = false;        
        }

        tweenSequences.Add(sequence);
    }

    public virtual void DropTween(Vector3 dropPos, Vector3 dropOffset, Dropzone _targetDropzone){
        DeclareActiveTween();
        _targetDropzone.isOccupied = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
//        sequence.AppendCallback(() => GetComponent<Collider>().enabled = true);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToWorldLayer(pickupDropTime));
        PickedUp = false;
        tweenSequences.Add(sequence);
    }

     public virtual void MoveTween(Vector3 dropPos){
        DeclareActiveTween();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
        transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
        sequence.OnComplete(() => DeclareInactiveTween());
        StartCoroutine(ChangeToWorldLayer(pickupDropTime));
        PickedUp = false;
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
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(Vector3.zero, tweenEndTime, RotateMode.Fast));
        sequence.OnComplete(() => DeclareInactiveTween());
        tweenSequences.Add(sequence);
    } 

    public void DeclareActiveTween(){
        Services.TweenManager.tweensAreActive = true;
    }
    
    public virtual void DeclareInactiveTween(){
        // Debug.Log("Declare inactive tween called!");
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
        PickedUp = true;
    }

    public virtual IEnumerator ChangeToFirstPersonLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
             Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;    
        }

        gameObject.layer = 13;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 13;
    
//            grandchildren.transform.GetChild(i).gameObject.layer = 13;

        }
    }

    public virtual void ChangeLayerToFirstPerson()
    {
        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
            Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;    
        }

        if (gameObject.layer != 13)
        {
            int children = transform.childCount;
            gameObject.layer = 13;
            for (int i = 0; i < children; ++i)
            {
                transform.GetChild(i).gameObject.layer = 13;
        
    //            grandchildren.transform.GetChild(i).gameObject.layer = 13;
    
            }
        }
    }

    public virtual IEnumerator ChangeToWorldLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
        gameObject.layer = 12;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 12;
        }
    }

    public virtual void ChangeLayerToWorld()
    {
        int children = transform.childCount;

        if (gameObject.layer != 0)
        {
            gameObject.layer = 0;
            for (int i = 0; i < children; ++i)
            {
                transform.GetChild(i).gameObject.layer = 0;
            }
        }
    }

    protected virtual void ResetEvaluationStatus()
    {
        
    }

    private void SetupDropzoneThenDestroySelf()
    {
//        myChildDropzone.objectsInMe.Clear();
//        myChildDropzone.isOccupied = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
//        EventManager.Instance.Unregister<DayEndEvent>(ReturnHome);
    }

    public virtual void ReturnHome(GameEvent e){
        DayEndEvent dayEndEvent = e as DayEndEvent;
        transform.position = origPos;
        PickedUp = false;
        transform.eulerAngles = Vector3.zero;
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ChangeToWorldLayer(1f));
        }

//        if(GetComponent<Glass>() != null){
//            Glass glass = GetComponent<Glass>();
//            glass.ClearIce();
//            glass.hasIce = false;
//            glass.liquid.isEvaluated = false;
//            glass.liquid.EmptyLiquid();
//            glass.glassServeState = Glass.GlassServeState.NotReadyToServe;
//            glass.liquid.myDrinkBase = DrinkBase.none;
//            glass.liquid.myMixer = Mixer.none;
//        }
    }
}

public class DayEndEvent : GameEvent{

}


