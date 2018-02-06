using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaster : MonoBehaviour
{
	private NPC myCustomer;

	public enum Customer
	{
		Ivory,
		Sahana
	}

	public Customer currentCustomer;
	
	void Start()
	{
		switch (currentCustomer)
		{
			case Customer.Ivory:
				myCustomer = Services.GameManager.CustomerIvory;
			break;
			case Customer.Sahana:
				myCustomer = Services.GameManager.CustomerSahana;
			break;
			default:
			break;
		}
	}

	void Update()
	{

	}

	public void EvaluateDrink(Liquid _liquid)
	{
//		abv = _liquid.thisCocktail.alcoholRate;
//		spiciness = _liquid.thisCocktail.spicyRate;
//		sweetness = _liquid.thisCocktail.sweetRate;
//		sourness = _liquid.thisCocktail.sourRate;
//		smokiness = _liquid.thisCocktail.smokyRate;
//		bitterness = _liquid.thisCocktail.bitterRate;

//		if (myCustomer == Services.GameManager.CustomerIvory)
//		{
//			if (_liquid.thisCocktail.whiskeyRate >= 0.20f || _liquid.thisCocktail.bitterness >= 0.10f)
//			{
////				myCustomer.SetCustomerVars(0, 0, 0, 0);		
//			}
//		}
	}

	private void DetectCustomer()
	{
		
	}



}
