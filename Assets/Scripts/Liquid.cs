using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {

 	public float height;
	public bool isPouring;

	private float totalVolume;
	public DrinkBase myBase;
	public Garnish myGarnish;
	public Mixer myMixer;

	public float whiskeyVolume;
	public float tequilaVolume;
	public float rumVolume;
	public float ginVolume;
	public float beerVolume;
	public float wineVolume;
	public float brandyVolume;
	public float vodkaVolume;
	public float smokyVolume;
	public float sweetVolume;
	public float sourVolume;
	public float bitterVolume;
	public float alcoholVolume; 
	public float spicyVolume;

	public float abv;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isPouring){
			GrowVertical();
		} 

		abv = alcoholVolume/height;
	}

	public void GrowVertical(){
		height += 1000 * Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
	}

	private DrinkProfile myDrinkProfile;
	public void AddIngredient(DrinkBase _drinkBase){
		myDrinkProfile = Services.DrinkDictionary.drinkBases[_drinkBase];
		myBase = _drinkBase;
		switch (_drinkBase){
			case DrinkBase.whiskey:
			whiskeyVolume = height * myDrinkProfile.whiskeyVolume;
			break;
			case DrinkBase.gin:
			ginVolume = myDrinkProfile.ginVolume * height;
			break;
			case DrinkBase.tequila:
			tequilaVolume = myDrinkProfile.tequilaVolume * height;
			break;
			case DrinkBase.vodka:
			vodkaVolume = myDrinkProfile.vodkaVolume * height;
			break;
			case DrinkBase.rum:
			rumVolume = myDrinkProfile.rumVolume * height;
			break;
			case DrinkBase.beer:
			beerVolume = myDrinkProfile.beerVolume * height;
			break;
			case DrinkBase.wine:
			wineVolume = myDrinkProfile.wineVolume * height;
			break;
			case DrinkBase.brandy:
			brandyVolume = myDrinkProfile.brandyVolume * height;
			break;
			default:
			break;
		}
		alcoholVolume = myDrinkProfile.alcoholVolume * height;
		sweetVolume = myDrinkProfile.sweetVolume * height;
		sourVolume = myDrinkProfile.sourVolume * height;
		bitterVolume = myDrinkProfile.bitterVolume * height;
		spicyVolume = myDrinkProfile.spicyVolume * height;
		smokyVolume = myDrinkProfile.smokyVolume * height;		
	}
}
