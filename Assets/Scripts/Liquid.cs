using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {

	float totalVolume;
	float alcoholVolume;
	// float 
	public float height;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GrowVertical(){
		height += Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
	}

	public void AddIngredient(){

	}
}
