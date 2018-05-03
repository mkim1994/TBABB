using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Notepad : Pickupable {

    [SerializeField]private Vector3 _rightHandStartPos;
    [SerializeField]private Vector3 _leftHandStartPos;
    [SerializeField]private Vector3 _signRightHandPos;
    [SerializeField]private Vector3 _signLeftHandPos;
    [SerializeField]private Vector3 _rightHandStartRot;
    [SerializeField]private Vector3 _leftHandStartRot;
    [SerializeField]private Vector3 _signRightHandRot;
    [SerializeField]private Vector3 _signLeftHandRot;
    public Texture[] notes;
    public Texture notesigned;
    
    [SerializeField] private Pen _pen;
    Material mat;
    bool onlyOnce;
	// Use this for initialization
	protected override void Start () {
        base.Start();        
		onlyOnce = false;
        mat = GetComponent<MeshRenderer>().material;
	    _pen = FindObjectOfType<Pen>();
	}
	
	// Update is called once per frame
	public override void Update () {
        if(mat.GetTexture("_MainTex") != notes[Services.GameManager.dayManager.currentDay]){
            if (Services.GameManager.dayManager.currentDay != 0)
            {
                mat.SetTexture("_MainTex", notes[Services.GameManager.dayManager.currentDay]);
            }
        }
		if (Services.GameManager.dayManager.noteSigned && !onlyOnce)
        {
            if (mat.GetTexture("_MainTex") != notesigned){
                mat.SetTexture("_MainTex", notesigned);
                onlyOnce = true;
            }
        }
	}

    public void SignNoteOnRightHand()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_signRightHandPos, 1f));
        s.Append(transform.DOLocalMove(_rightHandStartPos, 0.75f));
        s.OnComplete(() => Services.GameManager.dayManager.noteSigned = true);
	    Sequence r = DOTween.Sequence();
	    r.Append(transform.DOLocalRotate(_signRightHandRot, 1f));
	    r.Append(transform.DOLocalRotate(_rightHandStartRot, 0.75f));
    }

    public void SignNoteOnLeftHand()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_signLeftHandPos, 1f));
        s.Append(transform.DOLocalMove(_leftHandStartPos, 0.75f));
        s.OnComplete(() => Services.GameManager.dayManager.noteSigned = true);
	    Sequence r = DOTween.Sequence();
	    r.Append(transform.DOLocalRotate(_signLeftHandRot, 1f));
	    r.Append(transform.DOLocalRotate(_leftHandStartRot, 0.75f));
    }

	public void ReadNote()
	{
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
