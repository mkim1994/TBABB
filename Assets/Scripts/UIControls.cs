using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour {

	private Camera myCam;
	private void UIRay(){
		Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
 	
		} else {

		}
	}
}
