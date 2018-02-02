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
	public float smokyRate, sweetRate, sourRate, bitterRate, alcoholRate, spicyRate;
 
	public DrinkProfile(float _sodaRate, float _tonicRate, float _appleJuiceRate, float _orangeJuiceRate,
						float _lemonJuiceRate, //mixers
						
						float _lemonSliceRate, float _orangePeelRate, float _bitterGarnishRate,
						float _sugarRate, float _oliveRate, float _chiliPowderRate, //garnish

						float _whiskeyVol, float _ginVol, float _tequilaVol, float _vodkaVol, float _rumVol, 
						float _beerVol, float _wineVol, float _brandyVol, 
						
						float _alcoholVol, 
						
						float _smokyVol, float _sweetVol, float _sourVol, float _bitterVol, float _spicyVol){ //mixers
		
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
		whiskeyRate = _whiskeyVol;
		ginRate = _ginVol; 
		tequilaRate = _tequilaVol;
		vodkaRate = _vodkaVol;
		rumRate = _rumVol;
		beerRate = _beerVol;
		wineRate = _wineVol;
		brandyRate = _brandyVol;
		//flavors
		smokyRate = _smokyVol;
		sweetRate = _sweetVol;
		sourRate = _sourVol;
		bitterRate = _bitterVol;
		spicyRate = _spicyVol;
		alcoholRate = _alcoholVol;
	}
}