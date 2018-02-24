using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : Pickupable {
	public bool hasLiquid;
	public bool isFull;
	public bool isDirty;
	private Liquid liquid;
//	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
//	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);

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
		liquid = GetComponentInChildren<Liquid>();
 		switch (glassType)
		{
			case GlassType.Beer_mug:
			break;
			case GlassType.Highball:
				dropOffset = new Vector3(0, -0.15f, 0);
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
	
	public void ReceivePourFromBottle(Bottle bottleInHand){
		// Liquid liquid = GetComponentInChildren<Liquid>();
		liquid.isPouring = true;
//		bottleInHand = Services.GameManager.player.GetComponentInChildren<Bottle>();

		if(bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none){
			liquid.AddIngredient(bottleInHand.myDrinkBase);
		} else if (bottleInHand.myMixer != Mixer.none && bottleInHand.myDrinkBase == DrinkBase.none){
			liquid.AddMixer(bottleInHand.myMixer);
		}
	} 

	public void EndPourFromBottle(){
		Liquid liquid = GetComponentInChildren<Liquid>();
		liquid.isPouring = false;
	}
}
