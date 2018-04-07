using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Liquid : MonoBehaviour {

	public bool hasIce;

	private int ice = 0;
 	private DrinkProfile myDrinkProfile;
	private float previousAlcoholVolume;
	private float prevABV;
 	public float height;
	private SkinnedMeshRenderer myLiquid;
	private float myLiquidVolume = 100;
	[SerializeField]private float pourRate = 1;
	public bool isEvaluated = false;
	public float totalVolume;
	[HideInInspector]public DrinkBase myDrinkBase;
	[HideInInspector]public Mixer myMixer;
	public List<Coaster> coasters = new List<Coaster> ();
// 	DrinkBase baseBeingPoured;
	Garnish garnishBeingApplied;
//	Mixer mixerBeingPoured;
	[SerializeField]float sodaVolume, tonicVolume, vermouthVolume, orangeJuiceVolume, lemonJuiceVolume;
	[SerializeField]float whiskeyVolume, tequilaVolume, rumVolume, ginVolume, beerVolume, wineVolume, brandyVolume, vodkaVolume;
 	[SerializeField]float smokiness, sweetness, sourness, bitterness, spiciness;
	[SerializeField]float alcoholVolume, abv;

	public List<float> drinkParts = new List<float>();

    private float originalX, originalZ;
	
	public DrinkProfile thisCocktail;
	public GameObject otherLiquid;
	public GameObject otherLiquidMask;
	public GameObject myMask;

	public bool isBeingPoured;
 
	// Use this for initialization
	void Start ()
	{
		drinkParts.Add(sodaVolume);
		drinkParts.Add(tonicVolume);
		drinkParts.Add(whiskeyVolume);
		
		if (gameObject.GetComponent<SkinnedMeshRenderer>() != null)
		{
			myLiquid = GetComponent<SkinnedMeshRenderer>();
 		} else if (gameObject.GetComponent<SkinnedMeshRenderer>() == null)
		{
		}



		DetectCoasters ();
 		thisCocktail = new DrinkProfile (sodaVolume/height, tonicVolume/height, vermouthVolume/height, lemonJuiceVolume/height, 0, 0, 0, 0, 0, 0, 0, 
		whiskeyVolume/height, ginVolume/height, tequilaVolume/height, vodkaVolume/height, rumVolume/height, beerVolume/height, 
		wineVolume/height, brandyVolume/height, abv, 
		smokiness, sweetness, sourness, bitterness, spiciness, ice);
        originalX = transform.localScale.x;
        originalZ = transform.localScale.z;
        if (GetComponent<Bottle>() == null)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
	}

	// Update is called once per frame
	void Update () {
		
//		Debug.Log(gameObject.name + " isBeingPoured is " + isBeingPoured);
 		
		totalVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume 
					+ sodaVolume + tonicVolume + vermouthVolume + orangeJuiceVolume + lemonJuiceVolume;
 		
//		alcoholVolume = whiskeyVolume + ginVolume + brandyVolume + vodkaVolume + wineVolume + beerVolume + tequilaVolume + rumVolume; 
		// abv = alcoholVolume/height;
		abv = GetAlcoholicStrength();
		
		if (thisCocktail != null)
		{
			thisCocktail.totalVolume = totalVolume;
			if(hasIce){
				thisCocktail.ice = 1;
			} else {
				thisCocktail.ice = -1;
			}
 		}

		if (isBeingPoured)
		{
			GrowVertical();		
			if(myDrinkBase == DrinkBase.none){
				AddMixer(myMixer);
				switch (myMixer){
					case Mixer.soda:
						sodaVolume = height - totalVolume + sodaVolume;
						IncrementFlavor(myDrinkProfile, sodaVolume);
						break;
					case Mixer.tonic:
						tonicVolume = height - totalVolume + tonicVolume;
						IncrementFlavor(myDrinkProfile, tonicVolume);
						break;
					case Mixer.vermouth:
						vermouthVolume = height - totalVolume + vermouthVolume;
						IncrementFlavor(myDrinkProfile, vermouthVolume);
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

			if(myDrinkBase != DrinkBase.none){
				AddIngredient(myDrinkBase);
				switch (myDrinkBase){
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
			}

//			EvaluateDrinkInCoaster ();
			
			
		}
		
//		if (Input.GetKeyDown(KeyCode.CapsLock))
//		{
//			isBeingPoured = false;
//		}

 		
		EvaluateDrinkInCoaster ();

 	}

	public void GrowVertical(){
//		height -= 1 * Time.deltaTime;
		height = remapRange(myLiquidVolume, 0, 100, 100, 0);
//		height = Mathf.Clamp(height, 0, 100f);
		height = Mathf.Clamp(height, 0, 100);
		transform.localScale = new Vector3(1, 1, 1);
 		myLiquidVolume -= pourRate * Time.deltaTime;
		myLiquid.SetBlendShapeWeight(0, myLiquidVolume);
//        transform.localScale = new Vector3 (originalX, height, originalZ);
		thisCocktail = new DrinkProfile (sodaVolume/totalVolume, tonicVolume/totalVolume, vermouthVolume/totalVolume, lemonJuiceVolume/totalVolume, 0, 0, 0, 0, 0, 0, 0, 
		whiskeyVolume/totalVolume, ginVolume/totalVolume, tequilaVolume/totalVolume, vodkaVolume/totalVolume, rumVolume/totalVolume, beerVolume/totalVolume, 
		wineVolume/totalVolume, brandyVolume/totalVolume, abv, 
		smokiness, sweetness, sourness, bitterness, spiciness);
//		thisCocktail.totalVolume = totalVolume;
	}

	public void LetItPour()
	{
		isBeingPoured = true;
	}

	public void EmptyLiquid()
	{
		height = 0;
		sodaVolume = 0;
		tonicVolume = 0;
		vermouthVolume = 0;
		orangeJuiceVolume = 0;
		lemonJuiceVolume = 0;
		whiskeyVolume = 0;
		tequilaVolume = 0;
		rumVolume = 0;
		ginVolume = 0;
		beerVolume = 0;
		wineVolume = 0;
		brandyVolume = 0;
		vodkaVolume = 0;

		smokiness = 0;
		sweetness = 0;
		sourness = 0;
		bitterness = 0;
		spiciness = 0;
		alcoholVolume = 0;
		abv = 0;
		myLiquidVolume = 100;
		myLiquid.SetBlendShapeWeight(0, myLiquidVolume);
	}

	float remapRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax )
	{
		float newValue = 0;
		float oldRange = (oldMax - oldMin);
		float newRange = (newMax - newMin);
		newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
		return newValue;
	}

	public void AddIngredient(DrinkBase _drinkBase){
//		GrowVertical();
//		LetItPour();
		if (Services.DrinkDictionary.drinkBases.ContainsKey(_drinkBase))
		{
			Debug.Log(_drinkBase);
			myDrinkProfile = Services.DrinkDictionary.drinkBases[_drinkBase];		
			myDrinkBase = _drinkBase;
		}
//		baseBeingPoured = _drinkBase;
		
		// alcoholVolume = myDrinkProfile.alcoholVolume * height;
		// abv = alcoholVolume/height;		
	}

	public void AddMixer(Mixer _mixer){
		if (Services.MixerDictionary.mixers.ContainsKey(_mixer))
		{
			myDrinkProfile = Services.MixerDictionary.mixers[_mixer];
	//		mixerBeingPoured = _mixer;
			myMixer = _mixer;
		}

	}

	private float GetSmokiness(){
		float _smokyRate = 0;
		_smokyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].smokiness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].smokiness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].smokiness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].smokiness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].smokiness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].smokiness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].smokiness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].smokiness*vodkaVolume;
		return _smokyRate/height;	
 	}

	private float GetSweetness(){
		float _sweetRate = 0;
		_sweetRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sweetness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sweetness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sweetness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sweetness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sweetness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sweetness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sweetness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sweetness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sweetness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sweetness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.vermouth].sweetness * vermouthVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sweetness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sweetness * orangeJuiceVolume;

		return _sweetRate/height;	
 	}

	private float GetBitterness(){
		float _bitterRate = 0;
		_bitterRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].bitterness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].bitterness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].bitterness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].bitterness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].bitterness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].bitterness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].bitterness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].bitterness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].bitterness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].bitterness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.vermouth].bitterness * vermouthVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].bitterness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].bitterness * orangeJuiceVolume;
		return _bitterRate/height;
	}

	private float GetSourness(){
		float _sourRate = 0;
		_sourRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].sourness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].sourness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].sourness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].sourness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].sourness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].sourness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].sourness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].sourness*vodkaVolume
					+ Services.MixerDictionary.mixers[Mixer.soda].sourness * sodaVolume 
					+ Services.MixerDictionary.mixers[Mixer.tonic].sourness * tonicVolume 
					+ Services.MixerDictionary.mixers[Mixer.vermouth].sourness * vermouthVolume
					+ Services.MixerDictionary.mixers[Mixer.lemon_juice].sourness * lemonJuiceVolume
					+ Services.MixerDictionary.mixers[Mixer.orange_juice].sourness * orangeJuiceVolume;
		return _sourRate/height;	
 	}
	private float GetSpiciness(){
		float _spicyRate = 0;		
		_spicyRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].spiciness*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].spiciness*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].spiciness*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].spiciness*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].spiciness*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].spiciness*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].spiciness*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].spiciness*vodkaVolume;
		return _spicyRate/height;	
 	}

	private float GetAlcoholicStrength(){
		float _alcoholRate = 0;
		_alcoholRate = Services.DrinkDictionary.drinkBases[DrinkBase.whiskey].alcoholicStrength*whiskeyVolume			
					+ Services.DrinkDictionary.drinkBases[DrinkBase.tequila].alcoholicStrength*tequilaVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.rum].alcoholicStrength*rumVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.gin].alcoholicStrength*ginVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.beer].alcoholicStrength*beerVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.wine].alcoholicStrength*wineVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.brandy].alcoholicStrength*brandyVolume	
					+ Services.DrinkDictionary.drinkBases[DrinkBase.vodka].alcoholicStrength*vodkaVolume;
		return _alcoholRate/height;
	}

 	private void IncrementFlavor(DrinkProfile _myDrinkProfile, float _drinkVolume)
    {
		smokiness = GetSmokiness();
		bitterness = GetBitterness();
		spiciness = GetSpiciness();
		sourness = GetSourness();
		sweetness = GetSweetness();
    }

	private void DetectCoasters(){
		coasters.AddRange (FindObjectsOfType<Coaster> ());
	}

	public void EvaluateDrinkInCoaster(){
		foreach (var coaster in coasters) {
			// Debug.Log("Distance to coaster " + Vector3.Distance (coaster.gameObject.transform.position, transform.position));
			if (Vector3.Distance (coaster.gameObject.transform.position, transform.position) <= 0.01f) {
				if(!isEvaluated && !GetComponentInParent<Pickupable>().pickedUp)
				{
 					coaster.EvaluateDrink (this.thisCocktail, this);	
					isEvaluated = true;
				}        
			}
		}
	}

	public void SetEvaluatedToFalse()
	{
		isEvaluated = false;
	}
	
}
