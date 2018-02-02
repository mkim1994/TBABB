using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : Pickupable {
	public bool hasLiquid;
	public bool isFull;
	public bool isDirty;

	private Liquid liquid;

	private Vector3 leftHandPourRot = new Vector3(88.76f, 0, 0);
	private Vector3 rightHandPourRot = new Vector3(87.7370f, 0, 6.915f);

	void Start(){
		liquid = GetComponentInChildren<Liquid>();
	}
	public void ReceivePourFromBottle(Bottle bottleInHand){
		// Liquid liquid = GetComponentInChildren<Liquid>();
		liquid.isPouring = true;
		bottleInHand = Services.GameManager.player.GetComponentInChildren<Bottle>();
		liquid.AddIngredient(bottleInHand.myDrinkBase);
	} 

	public void EndPourFromBottle(){
		Liquid liquid = GetComponentInChildren<Liquid>();
		liquid.isPouring = false;
	}
}
