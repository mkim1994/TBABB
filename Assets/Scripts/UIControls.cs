using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour {
	[SerializeField]LayerMask controlsMask;
	[SerializeField]Text controlsText;
	[SerializeField]string targetObj;

	[SerializeField]List<string> controls = new List<string>();
	private Camera myCam;

	void Start(){
		controls.Add("Press Q to pick up " + targetObj + " with left hand");
		controls.Add("Press E to pick up " + targetObj + " with right hand");
		controls.Add("Press E to drop object in right hand");
		controls.Add("Press Q to drop object in left hand");
		controls.Add("Hold LMB or RMB to pour drink");
		controls.Add("Press Q to serve drink");
		controls.Add("Press E to serve drink");
	}
	void Update(){
		UIRay();
	}
	private void UIRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, controlsMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			if(hitObj.GetComponent<Bottle>() != null){
				Bottle targetBottle = hitObj.GetComponent<Bottle>();
				switch (targetBottle.myDrinkBase){
					case DrinkBase.beer:
						targetObj = "beer";
					break;
					case DrinkBase.brandy:
						targetObj = "brandy";
					break;
					case DrinkBase.gin:
						targetObj = "gin";
					break;
					case DrinkBase.rum:
						targetObj = "rum";
					break;
					case DrinkBase.tequila:
						targetObj = "tequila";
					break;
					case DrinkBase.vodka:
						targetObj = "vodka";
					break;
					case DrinkBase.whiskey:
						targetObj = "whiskey";
					break;
					case DrinkBase.wine:
						targetObj = "wine";
					break;
					default:
					break;
				}
			}
		} else {
			//no hit.

		}
	}
}
