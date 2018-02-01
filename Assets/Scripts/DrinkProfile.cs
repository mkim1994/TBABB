using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public class DrinkProfile {

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
 
	public DrinkProfile(float _whiskeyVol, float _ginVol, float _tequilaVol, float _vodkaVol, float _rumVol, float _beerVol, float _wineVol, float _brandyVol, 
		float _alcoholVol, float _smokyVol, float _sweetVol, float _sourVol, float _bitterVol, float _spicyVol){
		whiskeyVolume = _whiskeyVol;
		ginVolume = _ginVol; 
		tequilaVolume = _tequilaVol;
		vodkaVolume = _vodkaVol;
		rumVolume = _rumVol;
		beerVolume = _beerVol;
		wineVolume = _wineVol;
		brandyVolume = _brandyVol;
		alcoholVolume = _alcoholVol;
		smokyVolume = _smokyVol;
		sweetVolume = _sweetVol;
		sourVolume = _sourVol;
		bitterVolume = _bitterVol;
		spicyVolume = _spicyVol;
	}
}