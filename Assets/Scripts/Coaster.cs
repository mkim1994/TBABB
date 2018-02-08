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
		if(currentOrder != null && _cocktail !=null){
			float drinkDeviation = DrinkProfile.GetProfileDeviation(_cocktail, currentOrder);
			// float abvSimilarity = DrinkProfile.GetABV
			float abvDeviation = DrinkProfile.GetABVdeviation(_cocktail, currentOrder);
			if(drinkDeviation <= 0.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (1.0f, 100);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(1.0f, 50);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(1.0f, 0);
					myCustomer.InitiateDialogue();
				}
			} else if (drinkDeviation > 0.5f && drinkDeviation <= 1.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (0.8f, 100);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(0.8f, 50);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(0.8f, 0);
					myCustomer.InitiateDialogue();
				}
			} else if (drinkDeviation > 1.5f && drinkDeviation <= 2.5f){
				if(_cocktail.alcoholicStrength >= 0.25f){
					myCustomer.SetCustomerVars (0.5f, 100);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
					myCustomer.SetCustomerVars(0.5f, 50);
					myCustomer.InitiateDialogue();
				} else if (_cocktail.alcoholicStrength < 0.10f){
					myCustomer.SetCustomerVars(0.5f, 0);
					myCustomer.InitiateDialogue();
				}
			} else if (_cocktail == null){
				myCustomer.SetCustomerVars(0, 0);
				myCustomer.InitiateDialogue();
			}
		} else {
			myCustomer.InitiateDialogue();
			myCustomer.SetCustomerVars(0, 0);
		}	
	}

	public void TakeOrder (DrinkProfile _customerOrder){
		currentOrder = _customerOrder; 	
	}

	





}
