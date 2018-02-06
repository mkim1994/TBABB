using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DrinkProfile {

	 //mixers 
	public float 	sodaRate, tonicRate, appleJuiceRate, orangeJuiceRate, lemonJuiceRate; 
	
	//garnish
	public float 	lemonSliceRate, orangePeelRate, bitterGarnishRate, sugarRate, 
					oliveRate, chiliPowderRate;
	
	//mixers
	public float whiskeyRate, tequilaRate, rumRate, ginRate, beerRate, wineRate, brandyRate, vodkaRate;
	public float smokiness, sweetness, sourness, bitterness, alcoholicStrength, spicyRate;
 
	public DrinkProfile(float _sodaRate, float _tonicRate, float _appleJuiceRate, float _orangeJuiceRate,
						float _lemonJuiceRate, //mixers
						
						float _lemonSliceRate, float _orangePeelRate, float _bitterGarnishRate,
						float _sugarRate, float _oliveRate, float _chiliPowderRate, //garnish

						float _whiskeyRate, float _ginRate, float _tequilaRate, float _vodkaRate, float _rumRate, 
						float _beerRate, float _wineRate, float _brandyRate, 
						
						float _alcoholicStrength, 
						
						float _smokiness, float _sweetness, float _sourness, float _bitterness, float _spiciness){ //mixers
		
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
		spicyRate = _spiciness;
		
		
	}
}