using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Glass : Pickupable {
	public bool hasLiquid;
	public bool isFull;
	public bool isDirty;
	private Liquid liquid;
//	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
//	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);
	private Vector3 leftHandPourRot = new Vector3(0, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(0, 0, 6.915f);
	private Vector3 leftHandPourPos = new Vector3(-0.14f, -0.5f, 1.75f);
	private Vector3 rightHandPourPos = new Vector3(0.14f, -0.5f, 1.75f);
	
	private enum GlassType { 
		Highball,
		Shot,
		Square,
		Wine_glass,
		Beer_mug
	}

	[SerializeField]GlassType glassType;

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
	
	public void ReceivePourFromBottle(Bottle bottleInHand, int handNum){

	//left hand is 0, right hand is 1


		if(bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none){
			liquid.AddIngredient(bottleInHand.myDrinkBase);
			if (pickedUp)
			{	
//				base.RotateTween(leftHandPourRot);
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);				
				} else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);				
				}
			}
		} else if (bottleInHand.myMixer != Mixer.none && bottleInHand.myDrinkBase == DrinkBase.none){
			liquid.AddMixer(bottleInHand.myMixer);
			if (pickedUp)
			{	
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);				
				} else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);				
				}
			} 
		}
	}
	
	public override void StartPourTween(Vector3 moveToPos)
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, 0.25f, false));
		sequence.OnComplete(() => DeclareInactiveTween());		
	}
	
	public void EndPourFromBottle(){
		Liquid liquid = GetComponentInChildren<Liquid>();
//		liquid.isPouring = false;
	}
	
	public override void EndPourTween()
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(startPos, 0.25f, false));
		sequence.OnComplete(() => DeclareInactiveTween());				
	}
	
	public override void UseLeftHand(){ 
		if(Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null){
			base.RotateTween(leftHandPourRot);
//			StartPourTween(Vector3.forward + new Vector3(-0.482f, 0, 0.5f));
 		} 
	}

}
