using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using DG.Tweening;

public class Glass : Pickupable
{
	public bool hasLiquid;
	[SerializeField] private bool isFull;
	public bool isDirty;

	public Liquid liquid;

//	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
//	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);
	private Vector3 leftHandPourRot = new Vector3(0, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(0, 0, 6.915f);
	private Vector3 leftHandPourPos = new Vector3(-0.14f, -0.5f, 1.75f);
	private Vector3 rightHandPourPos = new Vector3(0.14f, -0.5f, 1.75f);

	private enum GlassType
	{
		Highball,
		Shot,
		Square,
		Wine_glass,
		Beer_mug
	}

	[SerializeField] GlassType glassType;

	protected override void Start()
	{
		base.Start();
		liquid = GetComponentInChildren<Liquid>();
		switch (glassType)
		{
			case GlassType.Beer_mug:
				break;
			case GlassType.Highball:
				break;
			case GlassType.Shot:
				break;
			case GlassType.Square:
				break;
			case GlassType.Wine_glass:
				break;
			default:
				break;
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
//		Debug.Log("Receive Pour From Bottle Called!");
		
		if (bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none)
		{
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
		liquid.EvaluateDrinkInCoaster();
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

	public void EmptyGlass()
	{	
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		Vector3 moveToPos = Vector3.forward + new Vector3(-0.482f, 0, 0.5f);
		sequence.Append(transform.DOLocalMove(moveToPos, 0.5f, false));
		sequence.Append(transform.DOLocalMove(startPos, 0.5f, false));
		sequence.OnComplete(() => DeclareInactiveTween());
		
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DOLocalRotate(Vector3.right * 90f, 0.5f, RotateMode.Fast));
		rotateSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast));
		rotateSequence.OnComplete(() => liquid.EmptyLiquid());
		
//		liquid.empty;
	}

}
