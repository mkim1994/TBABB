using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DrinkProfile {

	 //mixers 
	public float 	sodaRate, tonicRate, appleJuiceRate, orangeJuiceRate, lemonJuiceRate; 
	
	//garnish
	public float 	lemonSliceRate, orangePeelRate, bitterGarnishRate, sugarRate, 
					oliveRate, chiliPowderRate;
	
	//drink bases
	public float whiskeyRate, tequilaRate, rumRate, ginRate, beerRate, wineRate, brandyRate, vodkaRate;
	public float smokiness, sweetness, sourness, bitterness, alcoholicStrength, spiciness;
 
	public float drinkSimilarity = 0;
	public float totalVolume;
	public DrinkProfile(float _sodaRate = 0, float _tonicRate = 0, float _appleJuiceRate = 0, float _orangeJuiceRate = 0,
						float _lemonJuiceRate = 0, //mixers
						
						float _lemonSliceRate = 0, float _orangePeelRate = 0, float _bitterGarnishRate = 0,
						float _sugarRate = 0, float _oliveRate = 0, float _chiliPowderRate = 0, //garnish

						float _whiskeyRate = 0, float _ginRate = 0, float _tequilaRate = 0, float _vodkaRate = 0, float _rumRate = 0, 
						float _beerRate = 0, float _wineRate = 0, float _brandyRate = 0, 
						
						float _alcoholicStrength = 0, 
						
						float _smokiness = 0, float _sweetness = 0, float _sourness = 0, float _bitterness = 0, float _spiciness = 0){ //mixers
		
		//mixers
		sodaRate = _sodaRate;
		tonicRate = _tonicRate;
		appleJuiceRate = _appleJuiceRate;
		orangeJuiceRate = _orangeJuiceRate;
		lemonJuiceRate = _lemonJuiceRate;

		//garnish
		lemonSliceRate = _lemonSliceRate;
		orangePeelRate = _orangePeelRate;
		bitterGarnishRate = _bitterGarnishRate;
		sugarRate = _sugarRate;
		oliveRate = _oliveRate;
		chiliPowderRate = _chiliPowderRate;

		//alcohol
		whiskeyRate = _whiskeyRate;
		ginRate = _ginRate; 
		tequilaRate = _tequilaRate;
		vodkaRate = _vodkaRate;
		rumRate = _rumRate;
		beerRate = _beerRate;
		wineRate = _wineRate;
		brandyRate = _brandyRate;

		//alcohol content
		alcoholicStrength = _alcoholicStrength;
		
		//flavors
		smokiness = _smokiness;
		sweetness = _sweetness;
		sourness = _sourness;
		bitterness = _bitterness;
		spiciness = _spiciness;
		
		
	}


	public static float GetProfileDeviation (DrinkProfile one, DrinkProfile two){
		float _similarity = 0;
		if(one != null && two != null){
			_similarity = 	Mathf.Abs(one.sodaRate - two.sodaRate) 
						+ Mathf.Abs(one.tonicRate - two.tonicRate) 
						+ Mathf.Abs(one.appleJuiceRate - two.appleJuiceRate)
						+ Mathf.Abs (one.orangeJuiceRate - two.orangeJuiceRate)
						+ Mathf.Abs (one.lemonJuiceRate - two.lemonJuiceRate)
						+ Mathf.Abs(one.lemonSliceRate - two.lemonSliceRate)
						+ Mathf.Abs(one.orangePeelRate - two.orangePeelRate)
						+ Mathf.Abs(one.bitterGarnishRate - two.bitterGarnishRate)
						+ Mathf.Abs(one.sugarRate - two.sugarRate)
						+ Mathf.Abs(one.oliveRate - two.oliveRate)
						+ Mathf.Abs(one.chiliPowderRate - two.chiliPowderRate)
						+ Mathf.Abs(one.whiskeyRate - two.whiskeyRate)
						+ Mathf.Abs(one.tequilaRate - two.tequilaRate)
						+ Mathf.Abs(one.rumRate - two.rumRate)
						+ Mathf.Abs(one.ginRate - two.ginRate)
						+ Mathf.Abs(one.beerRate - two.beerRate)
						+ Mathf.Abs(one.wineRate - two.wineRate)
						+ Mathf.Abs(one.brandyRate - two.brandyRate)
						+ Mathf.Abs(one.vodkaRate - two.vodkaRate)
						// + Mathf.Abs(one.alcoholicStrength - two.alcoholicStrength)
						+ Mathf.Abs(one.smokiness - two.smokiness)
						+ Mathf.Abs(one.sweetness - two.sweetness)
						+ Mathf.Abs(one.sourness - two.sourness)
						+ Mathf.Abs(one.bitterness - two.bitterness)
						+ Mathf.Abs(one.spiciness - two.spiciness);
		}

		// DrinkProfile combinedProfile = new DrinkProfile(
		// 	one.sodaRate - two.sodaRate, 
		// 	one.tonicRate - two.tonicRate,
		// 	one.appleJuiceRate - two.appleJuiceRate,
		// 	one.orangeJuiceRate - two.orangeJuiceRate,
		// 	one.lemonJuiceRate - two.lemonJuiceRate,
		// 	one.lemonSliceRate - two.lemonSliceRate,
		// 	one.orangePeelRate - two.orangePeelRate,
		// 	one.bitterGarnishRate - two.bitterGarnishRate,
		// 	one.sugarRate - two.sugarRate,
		// 	one.oliveRate - two.oliveRate,
		// 	one.chiliPowderRate - two.chiliPowderRate,
		// 	one.whiskeyRate - two.whiskeyRate,
		// 	one.tequilaRate - two.tequilaRate,
		// 	one.rumRate - two.rumRate,
		// 	one.ginRate - two.ginRate,
		// 	one.beerRate - two.beerRate,
		// 	one.wineRate - two.wineRate,
		// 	one.brandyRate - two.brandyRate,
		// 	one.vodkaRate - two.vodkaRate,
		// 	one.alcoholicStrength - two.alcoholicStrength,
		// 	one.smokiness - two.smokiness,
		// 	one.sweetness - two.sweetness,
		// 	one.sourness - two.sourness,
		// 	one.bitterness - two.bitterness,
		// 	one.spiciness - two.spiciness
		// 	);

			return _similarity;
	}

	public static float GetABVdeviation(DrinkProfile _drink1, DrinkProfile _drink2){
		float abvDeviation = 0;
		abvDeviation = Mathf.Abs(_drink1.alcoholicStrength - _drink2.alcoholicStrength);
		return abvDeviation;
	}
	public static DrinkProfile AddNewProfilePreferences(DrinkProfile profile, Flavor newFlavor, float newFlavorPref, DrinkBase newBase, float newDrinkBasePref, Mixer newMixer, float newMixerPref, float newAlcoholPref)
	{
		if(newFlavor == Flavor.bitter){
			profile.bitterness = newFlavorPref; 
			return profile;
		} 
		else {
			return profile;
		}
	}

	public static DrinkProfile OrderDrink(	Flavor flavor = Flavor.none, float newFlavorPref = 0, 
											DrinkBase dBase = DrinkBase.none, float newBasePref = 0, 
											Mixer mixer = Mixer.none, float newMixerPref = 0,
											Garnish garnish = Garnish.none, float newGarnishPref = 0){
												
		DrinkProfile order = new DrinkProfile();
		switch(flavor){
			case Flavor.bitter:
				order.bitterness = newFlavorPref;
			break;
			case Flavor.smoky:
				order.smokiness = newFlavorPref;
			break;				
			case Flavor.sour:
				order.sourness = newFlavorPref;
			break;
			case Flavor.spicy:
				order.spiciness = newFlavorPref;
			break;
			case Flavor.sweet:
				order.sweetness = newFlavorPref;
			break;
			default:
			break;
		}

		switch (dBase){
			case DrinkBase.beer:
				order.beerRate = newBasePref;
			break;
			case DrinkBase.brandy:
				order.brandyRate = newBasePref;
			break;
			case DrinkBase.gin:
				order.ginRate = newBasePref;
			break;
			case DrinkBase.rum:
				order.rumRate = newBasePref;
			break;
			case DrinkBase.tequila:
				order.tequilaRate = newBasePref;
			break;
			case DrinkBase.vodka:
				order.vodkaRate = newBasePref;
			break;
			case DrinkBase.whiskey:
				order.whiskeyRate = newBasePref;
			break;
			case DrinkBase.wine:
				order.wineRate = newBasePref;
			break;
			default:
			break;
		}

		switch(mixer){
			case Mixer.apple_juice:
				order.appleJuiceRate = newMixerPref;
			break;
			case Mixer.lemon_juice:
				order.lemonJuiceRate = newMixerPref;
			break;
			case Mixer.orange_juice:
				order.orangeJuiceRate = newMixerPref;
			break;
			case Mixer.soda:
				order.sodaRate = newMixerPref;
			break;
			case Mixer.tonic:
				order.tonicRate = newMixerPref;
			break;	
			default:
			break;
		}
		return order;	
	}

	
}