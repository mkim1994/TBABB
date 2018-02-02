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
 	public float smokiness;
	private float sweetVolume;
	public float sweetness;
	private float sourVolume;
	public float sourness;
	private float bitterVolume;
	public float bitterness;
	private float spicyVolume;
	public float spiciness;
	private float alcoholVolume; 

	public float abv;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		height = Mathf.Clamp(height, 0, 7058.475f);

		totalVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume; 
 	}

	public void GrowVertical(){
		height += 10000 * Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
	}

	private DrinkProfile myDrinkProfile;
	private float previousAlcoholVolume;
	private float prevABV;
	public void AddIngredient(DrinkBase _drinkBase){
		GrowVertical();
		myDrinkProfile = Services.DrinkDictionary.drinkBases[_drinkBase];
		myBase = _drinkBase;
		switch (_drinkBase){
            case DrinkBase.whiskey:
			whiskeyVolume = height - totalVolume + whiskeyVolume;
			IncrementFlavor(myDrinkProfile, whiskeyVolume);
			break;
            case DrinkBase.gin:
			ginVolume = ((myDrinkProfile.ginVolume * height) - totalVolume + ginVolume);
			IncrementFlavor(myDrinkProfile, ginVolume);
			break;
			case DrinkBase.tequila:
			tequilaVolume = ((myDrinkProfile.tequilaVolume * height) - totalVolume + tequilaVolume);
			IncrementFlavor(myDrinkProfile, tequilaVolume);
			break;
			case DrinkBase.vodka:
			vodkaVolume = ((myDrinkProfile.vodkaVolume * height) - totalVolume + vodkaVolume);
			IncrementFlavor(myDrinkProfile, vodkaVolume);		
			break;
			case DrinkBase.rum:
			rumVolume = ((myDrinkProfile.rumVolume * height) - totalVolume + rumVolume);
			IncrementFlavor(myDrinkProfile, rumVolume);
			break;
			case DrinkBase.beer:
			beerVolume = ((myDrinkProfile.beerVolume * height) - totalVolume + beerVolume);
			IncrementFlavor(myDrinkProfile, beerVolume);
			break;
			case DrinkBase.wine:
			wineVolume = ((myDrinkProfile.wineVolume * height) - totalVolume + wineVolume);
			IncrementFlavor(myDrinkProfile, wineVolume);
			break;
			case DrinkBase.brandy:
			brandyVolume = ((myDrinkProfile.brandyVolume * height) - totalVolume + brandyVolume);
			IncrementFlavor(myDrinkProfile, brandyVolume);
			break;
			default:
			break;
		}		
		// alcoholVolume = myDrinkProfile.alcoholVolume * height;
		// abv = alcoholVolume/height;		
	}

	private float SmokyRate(){
		float _smokyRate = 0;
		_smokyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].smokyRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].smokyRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].smokyRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].smokyRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].smokyRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].smokyRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].smokyRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].smokyRate*vodkaVolume;
		return _smokyRate/height;	
 	}

	private float SweetRate(){
		float _sweetRate = 0;
		_sweetRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sweetRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sweetRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sweetRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sweetRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sweetRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sweetRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sweetRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sweetRate*vodkaVolume;
		return _sweetRate/height;	
 	}

	private float BitterRate(){
		float _bitterRate = 0;
		_bitterRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].bitterRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].bitterRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].bitterRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].bitterRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].bitterRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].bitterRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].bitterRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].bitterRate*vodkaVolume;
		return _bitterRate/height;
	}

	private float SourRate(){
		float _sourRate = 0;
		_sourRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sourRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sourRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sourRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sourRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sourRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sourRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sourRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sourRate*vodkaVolume;
		return _sourRate/height;	
 	}
	private float SpicyRate(){
		float _spicyRate = 0;
		_spicyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].spicyRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].spicyRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].spicyRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].spicyRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].spicyRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].spicyRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].spicyRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].spicyRate*vodkaVolume;
		return _spicyRate/height;	
 	}

 	private void IncrementFlavor(DrinkProfile _myDrinkProfile, float _drinkVolume)
    {
		smokiness = SmokyRate();
		bitterness = BitterRate();
		spiciness = SpicyRate();
		sourness = SourRate();
		sweetness = SweetRate();
       /*  if (_myDrinkProfile.smokyRate != 0)
        {
						
        }
        if (_myDrinkProfile.spicyRate != 0)
        {
            spicyVolume = (_myDrinkProfile.spicyRate * _drinkVolume) - totalFlavor + _drinkVolume;
        }
        if (_myDrinkProfile.bitterRate != 0)
        {
            bitterVolume = (_myDrinkProfile.bitterRate * _drinkVolume) - totalFlavor + _drinkVolume;
        }
        if (_myDrinkProfile.sourRate != 0)
        {
            sourVolume = (_myDrinkProfile.sourRate * _drinkVolume) - totalFlavor + _drinkVolume;
        }
        if (_myDrinkProfile.sweetRate != 0)
        {
            sweetVolume = (_myDrinkProfile.sweetRate * _drinkVolume) - totalFlavor + _drinkVolume;
        }*/
    }
}
