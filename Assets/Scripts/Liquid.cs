using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {
	private DrinkProfile myDrinkProfile;
	private float previousAlcoholVolume;
	private float prevABV;
 	public float height;
	public bool isPouring;

	public bool isEvaluated = false;
	public float totalVolume;
	public List<Coaster> coasters = new List<Coaster> ();
// 	DrinkBase baseBeingPoured;
	Garnish garnishBeingApplied;
//	Mixer mixerBeingPoured;
	[SerializeField]float sodaVolume, tonicVolume, appleJuiceVolume, orangeJuiceVolume, lemonJuiceVolume;
	[SerializeField]float whiskeyVolume, tequilaVolume, rumVolume, ginVolume, beerVolume, wineVolume, brandyVolume, vodkaVolume;
 	[SerializeField]float smokiness, sweetness, sourness, bitterness, spiciness;
	[SerializeField]float alcoholVolume, abv;
	
	public DrinkProfile thisCocktail;
	

	// Use this for initialization
	void Start () {
		DetectCoasters ();
		thisCocktail = new DrinkProfile (sodaVolume/height, tonicVolume/height, appleJuiceVolume/height, lemonJuiceVolume/height, 0, 0, 0, 0, 0, 0, 0, 
			whiskeyVolume/height, ginVolume/height, tequilaVolume/height, vodkaVolume/height, rumVolume/height, beerVolume/height, 
			wineVolume/height, brandyVolume/height, abv, 
			smokiness, sweetness, sourness, bitterness, spiciness);
	}
	
	// Update is called once per frame
	void Update () {
 		EvaluateDrinkInCoaster ();
 
		height = Mathf.Clamp(height, 0, 7058.475f);

		totalVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume 
					+ sodaVolume + tonicVolume + appleJuiceVolume + orangeJuiceVolume + lemonJuiceVolume;
		
//		alcoholVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume; 
		// abv = alcoholVolume/height;
		abv = GetAlcoholicStrength();
		if (thisCocktail != null)
		{
			thisCocktail.totalVolume = totalVolume;
 		}
	}

	public void GrowVertical(){
		height += 10000 * Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
		thisCocktail = new DrinkProfile (sodaVolume/height, tonicVolume/height, appleJuiceVolume/height, lemonJuiceVolume/height, 0, 0, 0, 0, 0, 0, 0, 
		whiskeyVolume/height, ginVolume/height, tequilaVolume/height, vodkaVolume/height, rumVolume/height, beerVolume/height, 
		wineVolume/height, brandyVolume/height, abv, 
		smokiness, sweetness, sourness, bitterness, spiciness);
//		thisCocktail.totalVolume = totalVolume;

	}


	public void AddIngredient(DrinkBase _drinkBase){
		GrowVertical();
		myDrinkProfile = Services.DrinkDictionary.drinkBases[_drinkBase];
//		baseBeingPoured = _drinkBase;
		switch (_drinkBase){
            case DrinkBase.whiskey:
			whiskeyVolume = height - totalVolume + whiskeyVolume;
			IncrementFlavor(myDrinkProfile, whiskeyVolume);
			break;
            case DrinkBase.gin:
			ginVolume = height - totalVolume + ginVolume;
			IncrementFlavor(myDrinkProfile, ginVolume);
			break;
			case DrinkBase.tequila:
			tequilaVolume = height - totalVolume + tequilaVolume;
			IncrementFlavor(myDrinkProfile, tequilaVolume);
			break;
			case DrinkBase.vodka:
			vodkaVolume = height - totalVolume + vodkaVolume;
			IncrementFlavor(myDrinkProfile, vodkaVolume);		
			break;
			case DrinkBase.rum:
			rumVolume = height - totalVolume + rumVolume;
			IncrementFlavor(myDrinkProfile, rumVolume);
			break;
			case DrinkBase.beer:
			beerVolume = height - totalVolume + beerVolume;
			IncrementFlavor(myDrinkProfile, beerVolume);
			break;
			case DrinkBase.wine:
			wineVolume = height - totalVolume + wineVolume;
			IncrementFlavor(myDrinkProfile, wineVolume);
			break;
			case DrinkBase.brandy:
			brandyVolume = height - totalVolume + brandyVolume;
			IncrementFlavor(myDrinkProfile, brandyVolume);
			break;
			default:
			break;
		}
				
		// alcoholVolume = myDrinkProfile.alcoholVolume * height;
		// abv = alcoholVolume/height;		
	}

	public void AddMixer(Mixer _mixer){
		GrowVertical();
		myDrinkProfile = Services.MixerDictionary.mixers[_mixer];
//		mixerBeingPoured = _mixer;
		switch (_mixer){
			case Mixer.soda:
			sodaVolume = height - totalVolume + sodaVolume;
			IncrementFlavor(myDrinkProfile, sodaVolume);
			break;
			case Mixer.tonic:
			tonicVolume = height - totalVolume + tonicVolume;
			IncrementFlavor(myDrinkProfile, tonicVolume);
			break;
			case Mixer.apple_juice:
			appleJuiceVolume = height - totalVolume + appleJuiceVolume;
			IncrementFlavor(myDrinkProfile, appleJuiceVolume);
			break;
			case Mixer.orange_juice:
			orangeJuiceVolume = height - totalVolume + orangeJuiceVolume;
			IncrementFlavor(myDrinkProfile, orangeJuiceVolume);
			break;
			case Mixer.lemon_juice:
			lemonJuiceVolume = height - totalVolume + lemonJuiceVolume;
			IncrementFlavor(myDrinkProfile, lemonJuiceVolume);
			break;
			default:
			break;
		}
	}

	private float GetSmokiness(){
		float _smokyRate = 0;
		_smokyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].smokiness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].smokiness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].smokiness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].smokiness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].smokiness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].smokiness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].smokiness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].smokiness*vodkaVolume;
		return _smokyRate/height;	
 	}

	private float GetSweetness(){
		float _sweetRate = 0;
		_sweetRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sweetness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sweetness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sweetness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sweetness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sweetness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sweetness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sweetness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sweetness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sweetness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sweetness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].sweetness * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sweetness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sweetness * orangeJuiceVolume;

		return _sweetRate/height;	
 	}

	private float GetBitterness(){
		float _bitterRate = 0;
		_bitterRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].bitterness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].bitterness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].bitterness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].bitterness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].bitterness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].bitterness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].bitterness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].bitterness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].bitterness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].bitterness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].bitterness * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].bitterness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].bitterness * orangeJuiceVolume;
		return _bitterRate/height;
	}

	private float GetSourness(){
		float _sourRate = 0;
		_sourRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sourness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sourness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sourness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sourness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sourness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sourness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sourness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sourness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sourness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sourness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].sourness * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sourness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sourness * orangeJuiceVolume;
		return _sourRate/height;	
 	}
	private float GetSpiciness(){
		float _spicyRate = 0;		
		_spicyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].spiciness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].spiciness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].spiciness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].spiciness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].spiciness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].spiciness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].spiciness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].spiciness*vodkaVolume;
		return _spicyRate/height;	
 	}

	private float GetAlcoholicStrength(){
		float _alcoholRate = 0;
		_alcoholRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].alcoholicStrength*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].alcoholicStrength*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].alcoholicStrength*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].alcoholicStrength*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].alcoholicStrength*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].alcoholicStrength*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].alcoholicStrength*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].alcoholicStrength*vodkaVolume;
		return _alcoholRate/height;
	}

 	private void IncrementFlavor(DrinkProfile _myDrinkProfile, float _drinkVolume)
    {
		smokiness = GetSmokiness();
		bitterness = GetBitterness();
		spiciness = GetSpiciness();
		sourness = GetSourness();
		sweetness = GetSweetness();
    }

	private void DetectCoasters(){
		coasters.AddRange (FindObjectsOfType<Coaster> ());
	}
		
	private void EvaluateDrinkInCoaster(){
		foreach (var coaster in coasters) {
			if (Vector3.Distance (coaster.gameObject.transform.position, transform.position) <= 0.75f) {
				if(!isEvaluated){
					Debug.Log("Got evaluated");
					coaster.EvaluateDrink (this.thisCocktail);
					isEvaluated = true;
				}
 			}
//			else if (Vector3.Distance (coaster.gameObject.transform.position, transform.position) > 1f) 
//			{
//				Debug.Log("Setting isEvaluated to false!");
//				isEvaluated = false;
//			}
		}
	}

}
