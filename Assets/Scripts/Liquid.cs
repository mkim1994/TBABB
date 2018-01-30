using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {

	float totalVolume;
	float alcoholVolume;
	float typeVolume;
	float flavorVolume;
	// float 
	public float height;
	public bool isPouring;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isPouring){
			GrowVertical();
		} 
	}

	public void GrowVertical(){
		height += 1000 * Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
	}
	public DrinkBase myBase;

	// public void AddIngredient(DrinkBase ,){

	// }
}
