using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using DG.Tweening;

public class Glass : Pickupable
{
	public GameObject focalPoint;
	public List<Ice> myIceList = new List<Ice>();
	public bool hasLiquid;
	public bool hasIce;
	[SerializeField] private bool isFull;
	public bool isDirty;

	public Liquid liquid;

	public GameObject liquidSurfaceParent;
	public GameObject liquidSurfaceChild;

//	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
//	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);
	private Vector3 leftHandPourRot = new Vector3(80f, 0, 6.915f);
	private Vector3 rightHandPourRot = new Vector3(80f, 0, 6.915f);
	private Vector3 leftHandPourPos = new Vector3(-0.14f, -0.5f, 1.75f);
	private Vector3 rightHandPourPos = new Vector3(0.14f, -0.5f, 1.75f);

	[SerializeField]private Vector2 _playerLookVec2;
	[SerializeField]private float _playerLookSens;

	[SerializeField]private float _playerLookX;
	public enum GlassType
	{
		Highball,
		Shot,
		Square,
		Wine_glass,
		Beer_mug
	}

	[SerializeField]private GlassType glassType;

	protected override void Start()
	{
		base.Start();
		_playerLookSens = Services.GameManager.playerInput.lookSensitivity;
		liquid = GetComponentInChildren<Liquid>();
		if (liquid != null)
		{
			liquid._glassType = glassType;		
		}

	}

	private Vector3 myRot = Vector3.zero;
	public override void Update()
	{	
		_playerLookVec2 = Services.GameManager.playerInput.lookVector;

 		if(pickedUp && !Services.TweenManager.tweensAreActive){
			// transform.rotation = Quaternion.identity;
			// _playerLookX -= _playerLookVec2.y * _playerLookSens;
			// _playerLookX = Mathf.Clamp (_playerLookX, -75f, -25f);
			// transform.localRotation = Quaternion.Euler (_playerLookX, 0, 0);	 
		}

		if(myIceList.Count >= 3){
			liquid.hasIce = true;
		} else {
			liquid.hasIce = false;
		}
	}


	public bool IsFull
	{
		get
		{
			if (liquid.totalVolume >= 100f)
			{
				isFull = true;
			}
			else
			{
				isFull = false;
			}

			return isFull;
		}
	}

 	public void ReceivePourFromBottle(Bottle bottleInHand, int handNum)
	{
		//left hand is 0, right hand is 1		
		if (bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none)
		{
			Debug.Log(bottleInHand.myDrinkBase);
			liquid.AddIngredient(bottleInHand.myDrinkBase);
			if (pickedUp)
			{
//				base.RotateTween(leftHandPourRot);
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);
				}
				else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);
				}
			}
		}
		else if (bottleInHand.myMixer != Mixer.none && bottleInHand.myDrinkBase == DrinkBase.none)
		{
			liquid.AddMixer(bottleInHand.myMixer);
			if (pickedUp)
			{
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);
				}
				else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);
				}
			}
		}
	}

	public override void DropTween(Vector3 dropPos, Vector3 dropOffset, Dropzone _targetDropzone)
	{
		if (_targetDropzone.GetComponentInParent<Coaster>() != null)
		{
			Debug.Log("Target dropzone has coaster!");
//			dropOffset = new Vector3(0, -0.10f, 0);
		}
		else
		{
			dropOffset = new Vector3(0, 0, 0);
		}

		DeclareActiveTween();
		_targetDropzone.isOccupied = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
		transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
		sequence.OnComplete(() => DeclareInactiveTween());
		StartCoroutine(ChangeToWorldLayer(pickupDropTime));
		pickedUp = false;
	}

	public override void StartPourTween(Vector3 moveToPos)
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, tweenTime, false));
		sequence.OnComplete(() => DeclareInactiveTween());
	}

	public void EndPourFromBottle()
	{
//		Liquid liquid = GetComponentInChildren<Liquid>();
		liquid.isBeingPoured = false;
		liquid.isEvaluated = false;
//		liquid.EvaluateDrinkInCoaster();
//		liquid.isPouring = false;
	}

	public override void EndPourTween()
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(startPos, tweenTime, false));
		sequence.OnComplete(() => DeclareInactiveTween());
	}

	public override void UseLeftHand()
	{
		if (Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null)
		{
			base.RotateTween(leftHandPourRot);
//			StartPourTween(Vector3.forward + new Vector3(-0.482f, 0, 0.5f));
		}
	}

	
	public void LeftHandEmptyGlass()
	{	
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		Vector3 moveToPos = Vector3.forward + new Vector3(-0.482f, 0, 0.5f);
		sequence.Append(transform.DOLocalMove(leftHandPourPos, 0.5f, false));
		sequence.Append(transform.DOLocalMove(startPos, 0.5f, false));
		sequence.OnComplete(() => DeclareInactiveTween());
		
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DOLocalRotate(leftHandPourRot, 0.5f, RotateMode.Fast));
		rotateSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast));
		rotateSequence.OnComplete(() => liquid.EmptyLiquid()); 
		// rotateSequence.OnComplete(()=>ClearIce());
		ClearIce();
 	}
	
	public void RightHandEmptyGlass()
	{	
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		Vector3 moveToPos = Vector3.forward + new Vector3(-0.482f, 0, 0.5f);
		sequence.Append(transform.DOLocalMove(rightHandPourPos, 0.5f, false));
		sequence.Append(transform.DOLocalMove(startPos, 0.5f, false));
		sequence.OnComplete(() => DeclareInactiveTween());
		
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DOLocalRotate(rightHandPourRot, 0.5f, RotateMode.Fast));
		rotateSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast));
		rotateSequence.OnComplete(() => liquid.EmptyLiquid());
		// rotateSequence.OnComplete(()=>ClearIce());
		ClearIce();
//		liquid.empty;
	}

	public void ClearIce(){
		foreach (var ice in myIceList){
			//Do stuff
			Destroy(ice.gameObject);
		}
		myIceList.Clear();
	}

	public override IEnumerator ChangeToFirstPersonLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
		// Debug.Log("Children: " + children);
        startPos = transform.localPosition;

        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
             Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;
        }

		transform.GetChild(0).gameObject.layer = 13;
		transform.GetChild(1).gameObject.layer = 13;
		transform.GetChild(2).gameObject.layer = 13;
		transform.GetChild(3).gameObject.layer = 13;
		transform.GetChild(3).GetChild(0).gameObject.layer = 14;
    }
	public override IEnumerator ChangeToWorldLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        startPos = transform.localPosition;
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 0;
        }
		transform.GetChild(3).GetChild(0).gameObject.layer = 0;
    }

}
