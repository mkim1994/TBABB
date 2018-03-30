using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaster : MonoBehaviour
{
	public NPC myCustomer;
	private Dropzone myDropzone;
	private DrinkProfile currentOrder;

	public bool isDrinkHere = false;

	public Customer currentCustomer;
	public DrinkProfile drinkOnCoaster;	

	private float minAcceptableVolume = 15f;

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

		if(drinkOnCoaster != null){
			isDrinkHere = true;
		} else {
			isDrinkHere = false;
		}

		if(!isDrinkHere){
			myCustomer.ResetDrinkScore();
		}
	}

	void Update()
	{
		CheckDropzoneStatus();
	}

	public void EvaluateDrink(DrinkProfile _cocktail, Liquid _liquid){
  		if(_cocktail !=null){
			float drinkDeviation = DrinkProfile.GetProfileDeviation(_cocktail, currentOrder);
			// float abvSimilarity = DrinkProfile.GetABV
//			float abvDeviation = DrinkProfile.GetABVdeviation(_cocktail, currentOrder);
//			Debug.Log("drink deviation is " + drinkDeviation);
			drinkOnCoaster = _cocktail;
			if (_cocktail.totalVolume >= minAcceptableVolume)
			{
				if(drinkDeviation <= 0.5f && drinkDeviation > 0){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 1 true!");
						myCustomer.SetCustomerVars (1.0f, 100);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 2 true!");
						myCustomer.SetCustomerVars(1.0f, 50);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.10f){
						//true when you drop a whole bottle or an empty glass.
//						Debug.Log("Case 3 true!");
						myCustomer.SetCustomerVars(1.0f, 0);
						myCustomer.InitiateDialogue();
					}
				} else if (drinkDeviation > 0.5f && drinkDeviation <= 1.5f){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 4 true!");
						myCustomer.SetCustomerVars (0.8f, 100);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 5 true!");
						myCustomer.SetCustomerVars(0.8f, 50);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 6 true!");
						myCustomer.SetCustomerVars(0.8f, 0);
						myCustomer.InitiateDialogue();
					}
				} 
//				else if (drinkDeviation > 1.5f && drinkDeviation <= 2.5f){
//					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
//						myCustomer.SetCustomerVars (0.5f, 100);
//						myCustomer.InitiateDialogue();
//					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
//						myCustomer.SetCustomerVars(0.5f, 50);
//						myCustomer.InitiateDialogue();
//					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
//						myCustomer.SetCustomerVars(0.5f, 0);
//						myCustomer.InitiateDialogue();
//					}   	
//				}
				else if (drinkDeviation > 1.5f && drinkDeviation <= 1.95f){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
						myCustomer.SetCustomerVars (0.5f, 100);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						myCustomer.SetCustomerVars(0.5f, 50);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						myCustomer.SetCustomerVars(0.5f, 0);
						myCustomer.InitiateDialogue();
					}   	
				}
				else if (drinkDeviation > 1.95f) //no whiskey at all, acceptable amt of drink
				{
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
						myCustomer.SetCustomerVars (0.3f, 100);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						myCustomer.SetCustomerVars(0.3f, 50);
						myCustomer.InitiateDialogue();
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						myCustomer.SetCustomerVars(0.3f, 0);
						myCustomer.InitiateDialogue();
					}
				}
				
				else if (drinkDeviation <= 0)
				{
 					myCustomer.SetCustomerVars(0.5f, 0);
					myCustomer.InitiateDialogue();
				}

				//no whiskey at all in glass, but acceptable amount of drink
			}
			 else if (_cocktail.totalVolume < minAcceptableVolume && _cocktail.totalVolume > 0) //not enough drink in glass
			 {
				 myCustomer.SetCustomerVars(0.1f, 0);
				 myCustomer.InitiateDialogue();
			 }
			  
			 else if (_cocktail.totalVolume <= 0){ //empty glass or bottle
				myCustomer.SetCustomerVars(0, 0);
				myCustomer.InitiateDialogue();
			}				 
		} 
 	}

	public void TakeOrder (DrinkProfile _customerOrder)
	{
		currentOrder = _customerOrder;
		
	}

	private void CheckDropzoneStatus(){
		if(!GetComponentInChildren<Dropzone>().isOccupied){
			isDrinkHere = false;
		}
		else
		{
			isDrinkHere = true;
		}
	}
	
}
