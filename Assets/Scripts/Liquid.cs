using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {
	private DrinkProfile myDrinkProfile;
	private float previousAlcoholVolume;
	private float prevABV;
 	public float height;
	public bool isPouring;
	private float totalVolume;
	public List<Coaster> coasters = new List<Coaster> ();
	public Coaster targetCoaster;
	DrinkBase baseBeingPoured;
	Garnish garnishBeingApplied;
	Mixer mixerBeingPoured;
	[SerializeField]float sodaVolume, tonicVolume, appleJuiceVolume, orangeJuiceVolume, lemonJuiceVolume;
	[SerializeField]float whiskeyVolume, tequilaVolume, rumVolume, ginVolume, beerVolume, wineVolume, brandyVolume, vodkaVolume;
 	[SerializeField]float smokiness, sweetness, sourness, bitterness, spiciness;
	[SerializeField]float alcoholVolume, abv;
	
	public DrinkProfile thisCocktail;
	

	// Use this for initialization
	void Start () {
		DetectCoasters ();
	}
	
	// Update is called once per frame
	void Update () {
		EvaluateDrinkInCoaster ();

		height = Mathf.Clamp(height, 0, 7058.475f);

		totalVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume 
					+ sodaVolume + tonicVolume + appleJuiceVolume + orangeJuiceVolume + lemonJuiceVolume; 
		alcoholVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume; 
		// abv = alcoholVolume/height;
		abv = AlcoholRate();
 	}

	public void GrowVertical(){
		height += 10000 * Time.deltaTime;
		transform.localScale = new Vector3 (transform.localScale.x, height, transform.localScale.z);
		thisCocktail = new DrinkProfile (sodaVolume/height, tonicVolume/height, appleJuiceVolume/height, lemonJuiceVolume/height, 0, 0, 0, 0, 0, 0, 0, 
		whiskeyVolume/height, ginVolume/height, tequilaVolume/height, vodkaVolume/height, rumVolume/height, beerVolume/height, 
		wineVolume/height, brandyVolume/height, abv, 
		smokiness, sweetness, sourness, bitterness, spiciness);
	}


	public void AddIngredient(DrinkBase _drinkBase){
		GrowVertical();
		myDrinkProfile = Services.DrinkDictionary.drinkBases[_drinkBase];
		baseBeingPoured = _drinkBase;
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
		mixerBeingPoured = _mixer;
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
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sweetRate*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sweetRate * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sweetRate * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].sweetRate * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sweetRate * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sweetRate * orangeJuiceVolume;

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
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].bitterRate*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].bitterRate * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].bitterRate * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].bitterRate * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].bitterRate * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].bitterRate * orangeJuiceVolume;
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
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sourRate*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sourRate * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sourRate * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.apple_juice].sourRate * appleJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sourRate * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sourRate * orangeJuiceVolume;
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

	private float AlcoholRate(){
		float _alcoholRate = 0;
		_alcoholRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].alcoholRate*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].alcoholRate*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].alcoholRate*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].alcoholRate*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].alcoholRate*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].alcoholRate*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].alcoholRate*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].alcoholRate*vodkaVolume;
		return _alcoholRate/height;
	}

 	private void IncrementFlavor(DrinkProfile _myDrinkProfile, float _drinkVolume)
    {
		smokiness = SmokyRate();
		bitterness = BitterRate();
		spiciness = SpicyRate();
		sourness = SourRate();
		sweetness = SweetRate();
    }

	private void DetectCoasters(){
		coasters.AddRange (FindObjectsOfType<Coaster> ());
	}
		
	private void EvaluateDrinkInCoaster(){
		foreach (var coaster in coasters) {
			if (Vector3.Distance (coaster.gameObject.transform.position, transform.position) <= 0.5f) {
				coaster.EvaluateDrink (this);
			}		
		}
	}
}
