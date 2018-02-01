using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Garnish {
	none,
	lemon_peel,
	orange_peel,
	bitters,
	sweetener,
	salt, 
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
	juice,
	beer,
	wine
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
		{ DrinkBase.whiskey, new DrinkProfile(1, 0, 0, 0, 0, 0, 0, 0, 0.43f, 0.3f, 0.00f, 0f, 0.05f, 0)},
		{ DrinkBase.rum, new DrinkProfile(0, 0, 0, 0, 1, 0, 0, 0, 0.40f, 0.3f, 0.05f, 0f, 0.05f, 0.3f)},
		{ DrinkBase.gin, new DrinkProfile(0, 1, 0, 0, 0, 0, 0, 0, 0.375f, 0.3f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.vodka, new DrinkProfile(0, 0, 0, 1, 0, 0, 0, 0, 0.40f, 0.3f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.tequila, new DrinkProfile(0, 0, 1, 0, 0, 0, 0, 0, 0.40f, 0.3f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.wine, new DrinkProfile(0, 0, 0, 0, 0, 0, 1, 0, 0.125f, 0.3f, 0.05f, 0f, 0.05f, 0)},
		{ DrinkBase.beer, new DrinkProfile(0, 0, 0, 0, 0, 1, 0, 0, 0.05f, 0.3f, 0.05f, 0f, 0.1f, 0)},
		{ DrinkBase.brandy, new DrinkProfile(0, 0, 0, 0, 0, 0, 0, 1, 0.40f, 0, 0.10f, 0f, 0, 0)},
	};
}



	