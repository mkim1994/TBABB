using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaster : MonoBehaviour
{
	private NPC myCustomer;

	private DrinkProfile currentOrder;
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
				myCustomer.myCoaster = this;
			break;
			case Customer.SahanaDefault:
				myCustomer = Services.GameManager.CustomerSahana;
				myCustomer.myCoaster = this;
			break;
			default:
			break;
		}

	}

	void Update()
	{
		if(currentOrder != null){
		}
 	}

	public void EvaluateDrink(DrinkProfile _cocktail){
		if(currentOrder != null){
            Debug.Log("currentOrder");
			float drinkDeviation = DrinkProfile.GetProfileDeviation(_cocktail, currentOrder);
			// float abvSimilarity = DrinkProfile.GetABV
			float abvDeviation = DrinkProfile.GetABVdeviation(_cocktail, currentOrder);
			if(drinkDeviation <= 0.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (1.0f, 100);
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(1.0f, 50);
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(1.0f, 0);
				}
			} else if (drinkDeviation > 0.5f && drinkDeviation <= 1.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (0.8f, 100);
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(0.8f, 50);
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(0.8f, 0);
				}
			} else if (drinkDeviation > 1.5f && drinkDeviation <= 2.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (0.5f, 100);
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(0.5f, 50);
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(0.5f, 0);
				}
			} else if (_cocktail == null){
				myCustomer.SetCustomerVars(0, 0);
			}
		}

		// if()
		// if (currentCustomer == Customer.IvoryDefault){
		// 	DrinkProfile evaluatedProfile = DrinkProfile.CompareProfiles(_liquid.thisCocktail, Services.CustomerDictionary.customerDrinkProfiles[Customer.IvoryDefault]);
		// 	if	(Mathf.Abs(evaluatedProfile.whiskeyRate) <= 0.1f){
		// 			Services.GameManager.CustomerIvory.SetCustomerVars(0.8f, _liquid.thisCocktail.alcoholicStrength);
		// 	}
		// }	
	}

	public void TakeOrder (DrinkProfile _customerOrder){
		currentOrder = _customerOrder; 	
	}

	





}
