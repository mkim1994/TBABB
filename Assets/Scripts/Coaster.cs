using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaster : MonoBehaviour
{
	private NPC myCustomer;

	// public enum Customer
	// {
	// 	Ivory,
	// 	Sahana
	// }

	public Customer currentCustomer;
	public DrinkProfile customerOrder;	
	void Start()
	{
 		switch (currentCustomer)
		{
			case Customer.IvoryDefault:
				myCustomer = Services.GameManager.CustomerIvory;
			break;
			case Customer.SahanaDefault:
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
		// if()
		// if (currentCustomer == Customer.IvoryDefault){
		// 	DrinkProfile evaluatedProfile = DrinkProfile.CompareProfiles(_liquid.thisCocktail, Services.CustomerDictionary.customerDrinkProfiles[Customer.IvoryDefault]);
		// 	if	(Mathf.Abs(evaluatedProfile.whiskeyRate) <= 0.1f){
		// 			Services.GameManager.CustomerIvory.SetCustomerVars(0.8f, _liquid.thisCocktail.alcoholicStrength);
		// 	}
		// }	
	}





}
