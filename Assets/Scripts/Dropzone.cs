using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropzone : MonoBehaviour {

	public bool isOccupied;
	// Use this for initialization
	Pickupable[] pickupables; 

	void Start () {
		pickupables = FindObjectsOfType<Pickupable>();
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, Random.Range(0, 359), transform.eulerAngles.z);
		foreach(var pickupable in pickupables){
 			if(Vector3.Distance(pickupable.transform.position, this.transform.position) <= 1f){
				isOccupied = true;				
			}
		}
		// isOccupied = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Occupied(){
		isOccupied = !isOccupied;
	}
}
