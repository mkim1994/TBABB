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
	none,
	smoky,
	sweet,
	sour, 
	bitter,
	spicy
}

public enum Customer{
	IvoryDefault,
	IvoryDynamic,
	SahanaDefault,
	SaharaDynamic
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

public class CustomerDictionary{
	public Dictionary<Customer, DrinkProfile> customerDrinkProfiles = new Dictionary<Customer, DrinkProfile>(){
		{ Customer.IvoryDefault, new DrinkProfile (
			0, //soda
			0.9f, //tonic
			0, //apple juice
			0, //orange juice
			0.1f, //lemon juice
			0.1f, // lemon slice
			0, // orange peel
			0.7f, //bitters
			0, //sugar
			0.1f, //olive
			0.1f, // chili powder
			0.6f, // whiskey
			0, // tequila
			0, //rum
			0, //gin
			0.3f, //beer
			0, //wine
			0, //brandy
			0.1f, //vodka
			0.5f,// alcohol level
			0.1f, //smokiness
			0, //sweetness
			0,//sourness
			0.6f,//bitterness
			0.3f//spiciness
		)},

		{ Customer.IvoryDynamic, new DrinkProfile (
			0, //soda
			0.9f, //tonic
			0, //apple juice
			0, //orange juice
			0.1f, //lemon juice
			0.1f, // lemon slice
			0, // orange peel
			0.7f, //bitters
			0, //sugar
			0.1f, //olive
			0.1f, // chili powder
			0.6f, // whiskey
			0, // tequila
			0, //rum
			0, //gin
			0.3f, //beer
			0, //wine
			0, //brandy
			0.1f, //vodka
			0.5f,// alcohol level
			0.1f, //smokiness
			0, //sweetness
			0,//sourness
			0.6f,//bitterness
			0.3f//spiciness
		)},
		{ Customer.SahanaDefault, new DrinkProfile(
			0.4f,
			0,
			0.3f,
			0.2f,
			0.1f,
			0.2f,
			0.2f,
			0,
			0.3f,
			0,
			0.3f,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0.1f,
			0,
			0.8f,
			0,
			0,
			0.2f
		)},

		{ Customer.SaharaDynamic, new DrinkProfile(
			0.4f,
			0,
			0.3f,
			0.2f,
			0.1f,
			0.2f,
			0.2f,
			0,
			0.3f,
			0,
			0.3f,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0.1f,
			0,
			0.8f,
			0,
			0,
			0.2f
		)}
	};

}



	