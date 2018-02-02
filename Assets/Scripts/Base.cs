using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Garnish {
	none,
	lemon_slice,
	orange_peel,
	bitters,
	sugar,
 	olive,
	chili_powder
	
}
public enum DrinkBase {
	none,
	whiskey,
	gin,
	tequila,
	vodka,
	rum,
	beer,
	wine,
	brandy
}

public enum Mixer{
	none,
	soda,
	tonic,
	orange_juice,
	lemon_juice,
	apple_juice
}

public enum Flavor {
	smoky,
	sweet,
	sour, 
	bitter,
	spicy
}

public class DrinkDictionary {	
	// DrinkProfile(float _whiskeyVol, float _ginVol, float _tequilaVol, float _vodkaVol, float _rumVol, float _beerVol, float _wineVol, float _brandyVol, 
	// 	float _alcoholVol, float _smokyVol, float _sweetVol, float _sourVol, float _bitterVol, float _spicyVol)
	public Dictionary<DrinkBase, DrinkProfile> drinkBases = new Dictionary<DrinkBase, DrinkProfile>(){
		{ DrinkBase.whiskey, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0.43f, 0.3f, 0.00f, 0f, 0.05f, 0)},
		{ DrinkBase.rum, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0.40f, 0.1f, 0.05f, 0f, 0.05f, 0.3f)},
		{ DrinkBase.gin, new DrinkProfile(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.375f, 0.4f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.vodka, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0.40f, 0.05f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.tequila, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0.40f, 0.3f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.wine, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0.125f, 0.2f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.beer, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0.05f, 0.02f, 0.05f, 0f, 0.1f, 0)},
		{ DrinkBase.brandy, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0.40f, 0.25f, 0.10f, 0f, 0, 0)},
		// { Mixer.soda , new DrinkProfile()}
	};
}

public class MixerDictionary {
	public Dictionary<Mixer, DrinkProfile> mixers = new Dictionary<Mixer, DrinkProfile>(){
		
	}
}

public class DrinkInfo {

}



	