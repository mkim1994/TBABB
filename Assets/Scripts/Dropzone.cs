using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropzone : MonoBehaviour {

	public bool isOccupied;
	// Use this for initialization
	void Start () {
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, Random.Range(0, 359), transform.eulerAngles.z);
		isOccupied = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleOccupiedStatus(){
		isOccupied = !isOccupied;
	}
}
