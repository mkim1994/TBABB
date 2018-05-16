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
	vermouth
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
	SaharaDynamic,
	IzzyDefault,
	IzzyDynamic,
	JuliaDefault,
	JuliaDynamic,
	ShayDefault,
	ShayDynamic
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
		{ Mixer.vermouth, new DrinkProfile(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.14f, 0, 0.5f, 0, 0, 0)},
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

public class Util
{
	public static float remapRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax )
	{
		float newValue = 0;
		float oldRange = (oldMax - oldMin);
		float newRange = (newMax - newMin);
		newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
		return newValue;
	}
	
	public static float GetRemappedColorFloat(float color)
	{
		color = remapRange(color, 0, 255, 0, 1);
		return color;
	}
	
	public static Color CombineColors(params Color[] aColors)
	{
		Color result = new Color(0,0,0,1);
		foreach(Color c in aColors)
		{
			result += c;
		}
		result /= aColors.Length;
		return result;
	}

	public static Color AddColors(Color color1, Color color2)
	{
		Color result = (color1 + color2)/2;
		return result;
	}
	
	
}

public class LiquidColors
{
	public static Dictionary<DrinkBase, Vector4> DrinkToColorDictionary = new Dictionary<DrinkBase, Vector4>()
	{
		{ DrinkBase.beer, Color.yellow },
		{ DrinkBase.brandy, new Vector4(Util.GetRemappedColorFloat(209), Util.GetRemappedColorFloat(80), 0, 1)},
		{ DrinkBase.gin, Color.green},
		{ DrinkBase.whiskey, new Vector4(Util.GetRemappedColorFloat(183), Util.GetRemappedColorFloat(97), 0, 1)},
		
		//rgb(139,69,19)
	};

	public static Dictionary<Mixer, Vector4> MixerToColorDictionary = new Dictionary<Mixer, Vector4>()
	{
		{ Mixer.soda, new Vector4(Util.GetRemappedColorFloat(139), Util.GetRemappedColorFloat(69), Util.GetRemappedColorFloat(19), 1)},
		{ Mixer.vermouth, Color.gray }
	};

}



	