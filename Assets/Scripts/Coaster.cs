using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;

public class Coaster : MonoBehaviour
{
	public NPC myCustomer;
	[SerializeField]private Dropzone myDropzone;
	private DrinkProfile currentOrder;
	public bool isDrinkHere = false;
	public Customer currentCustomer;
	public DrinkProfile drinkOnCoaster;
	public Glass myDrinkGlass;
	public Transform ServedTargetTransform;
	public Vector3 unservedPos; 
	private float minAcceptableVolume = 25f;

	void Start()
	{
		EventManager.Instance.Register<DayEndEvent>(ReturnToServePos);
		unservedPos = transform.position;
		
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
			case Customer.IzzyDefault:
				myCustomer = Services.GameManager.CustomerIzzy;
				myCustomer.myCoaster = this;
				break;
			case Customer.JuliaDefault:
				myCustomer = Services.GameManager.CustomerJulia;
				myCustomer.myCoaster = this;
				break;
			case Customer.ShayDefault:
				myCustomer = Services.GameManager.CustomerShay;
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

	private void OnDestroy()
	{
		EventManager.Instance.Unregister<DayEndEvent>(ReturnToServePos);
	}

	void Update()
	{
//		CheckDropzoneStatus();
	}

	public void EvaluateDrink(DrinkProfile _cocktail, Liquid _liquid){
		if(_cocktail !=null)
			{
				//assign glass to myDrinkGlass.
				if (_liquid.transform.parent != null)
				{
				  if(_liquid.transform.parent.GetComponent<Glass>() != null)
					  myDrinkGlass = _liquid.transform.parent.GetComponent<Glass>();			  
				}
				
				float drinkDeviation = DrinkProfile.GetProfileDeviation(_cocktail, currentOrder);
				int getIceValue = 0;

				if (currentOrder == null)
				{
					Debug.Log("WARNING! current order is null!");
				}
				if(_cocktail.ice == currentOrder.ice && _cocktail.ice == 1 && currentOrder.ice == 1){ //_cocktail.ice will never be 0, so if they're equal, it's 1 or -1
					getIceValue = 1;
				} else if(currentOrder.ice == 0 //customer doesn't care, so ice  or no ice, it's fine.
				 || (_cocktail.ice == -1 && currentOrder.ice == -1)){ //if player successfully fulfills the "no ice" order (-1, -1)
					getIceValue = 0;  
				}

			if (_cocktail.totalVolume >= minAcceptableVolume)
			{
				if(drinkDeviation <= 0.5f && drinkDeviation > 0){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 1 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars (90, 100);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars(95, 100);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 2 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(90, 50);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars(95, 50);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
						//true when you drop a whole bottle or an empty glass.
//						Debug.Log("Case 3 true!");	
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(90, 0);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars(95, 0);
							myCustomer.InitiateDialogue();
						}
					} 
				} else if (drinkDeviation > 0.5f && drinkDeviation <= 1.5f){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 4 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars (80, 100);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars (85, 100);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 5 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(80, 50);
							myCustomer.InitiateDialogue();
						}
						else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars (85, 100);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 6 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(80, 0);
							myCustomer.InitiateDialogue();
							StartCoroutine(DelayedInitiateDialogue(0.01f)); 
						} else if (getIceValue == -1 || getIceValue == 1 ){
							myCustomer.SetCustomerVars(85, 0);
							myCustomer.InitiateDialogue();
						}
					}
				} 
//				else if (drinkDeviation > 1.5f && drinkDeviation <= 2.5f){
//					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
//						myCustomer.SetCustomerVars (0.5f, 100);
//						StartCoroutine(DelayedInitiateDialogue(0.1f));
//					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
//						myCustomer.SetCustomerVars(0.5f, 50);
//						StartCoroutine(DelayedInitiateDialogue(0.1f));
//					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
//						myCustomer.SetCustomerVars(0.5f, 0);
//						StartCoroutine(DelayedInitiateDialogue(0.1f));
//					}   	
//				}
				else if (drinkDeviation > 1.5f && drinkDeviation <= 1.95f){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars (50, 100);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars (55, 100);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(50, 50);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							myCustomer.SetCustomerVars(55, 50);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(50, 0);
							myCustomer.InitiateDialogue();
						}
						else if (getIceValue == 1 || getIceValue == -1){
							Debug.Log(_cocktail.alcoholicStrength);
							myCustomer.SetCustomerVars(55, 0);
							myCustomer.InitiateDialogue();
						}
					}    	
				}
				else if (drinkDeviation > 1.95f) //no whiskey at all, acceptable amt of drink
				{
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars (30, 100);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == -1 || getIceValue == 1){
							myCustomer.SetCustomerVars (35, 100);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(30, 50);
							myCustomer.InitiateDialogue();
						}
						else if (getIceValue == -1 || getIceValue == 1){
							myCustomer.SetCustomerVars (35, 50);
							myCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						if(getIceValue == 0){
							myCustomer.SetCustomerVars(30, 0);
							myCustomer.InitiateDialogue();
						} else if (getIceValue == -1 || getIceValue == 1){
							myCustomer.SetCustomerVars(35, 0);
							myCustomer.InitiateDialogue();
						}
					}
				}
				
				else if (drinkDeviation <= 0)
				{
 					myCustomer.SetCustomerVars(50, 0);
					myCustomer.InitiateDialogue();

 				}

				//no whiskey at all in glass, but acceptable amount of drink
			}
			 else if (_cocktail.totalVolume < minAcceptableVolume && _cocktail.totalVolume > 0) //not enough drink in glass
			 {
				 myCustomer.SetCustomerVars(10, 0);
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

	IEnumerator DelayedInitiateDialogue(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		myCustomer.InitiateDialogue();
	}

	public void ReturnToServePos(GameEvent e)
	{
		DayEndEvent dayEndEvent = e as DayEndEvent;
		transform.position = unservedPos;
	}
}


