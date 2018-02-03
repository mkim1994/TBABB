using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {

 	public float height;
	public bool isPouring;
	private float totalVolume;
	[SerializeField]DrinkBase baseBeingPoured;
	[SerializeField]Garnish garnishBeingApplied;
	[SerializeField]Mixer mixerBeingPoured;
	[SerializeField]float sodaVolume, tonicVolume, appleJuiceVolume, orangeJuiceVolume, lemonJuiceVolume;
	[SerializeField]float whiskeyVolume, tequilaVolume, rumVolume, ginVolume, beerVolume, wineVolume, brandyVolume, vodkaVolume;
 	[SerializeField]float smokiness, sweetness, sourness, bitterness, spiciness;
	[SerializeField]float alcoholVolume, abv;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		height = Mathf.Clamp(height, 0, 7058.475f);

		totalVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume 
					+ sodaVolume + tonicVolume + appleJuiceVolume + orangeJuiceVolume + lemonJuiceVolume; 
		alcoholVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume; 
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
