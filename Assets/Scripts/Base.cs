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
	public Dictionary<DrinkBase, DrinkProfile> drinkBases = new Dictionary<DrinkBase, DrinkProfile>(){
		{ DrinkBase.whiskey, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0.43f, 0.3f, 0, 0, 0.1f, 0)},
		{ DrinkBase.tequila, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0.40f, 0.2f, 0f, 0f, 0.6f, 0)},
		{ DrinkBase.rum, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0.40f, 0.1f, 0.1f, 0f, 0.2f, 0.3f)},
		{ DrinkBase.gin, new DrinkProfile(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.375f, 0f, 0f, 0f, 0.5f, 0)},
		{ DrinkBase.vodka, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0.40f, 0f, 0, 0f, 0.25f, 0)},
		{ DrinkBase.wine, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0.125f, 0f, 0.25f, 0f, 0, 0)},
		{ DrinkBase.beer, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0.05f, 0f, 0f, 0f, 0.2f, 0)},
		{ DrinkBase.brandy, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0.40f, 0f, 0.35f, 0f, 0.1f, 0)},
 	};
}

public class MixerDictionary {
	public Dictionary<Mixer, DrinkProfile> mixers = new Dictionary<Mixer, DrinkProfile>(){
		{ Mixer.soda, new DrinkProfile(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.75f, 0, 0, 0)},
		{ Mixer.tonic, new DrinkProfile(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)},
		{ Mixer.apple_juice, new DrinkProfile(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.5f, 0, 0, 0)},
		{ Mixer.orange_juice, new DrinkProfile(0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.4f, 0f, 0, 0)},
		{ Mixer.lemon_juice, new DrinkProfile(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.10f, 0.5f, 0, 0)},
	};
}

public class DrinkInfo {

}



	