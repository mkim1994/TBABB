using System.Collections;
using System.Collections.Generic;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;
using Yarn;

public class Coaster : MonoBehaviour
{
	public bool IsOccupied;
	private GameObject _myDetector;
	private DrinkProfile _currentOrder;
	private Dropzone _myDropzone;
	private float _minAcceptableVolume = 25f;

	public NPC MyCustomer;
	[HideInInspector]public bool IsDrinkHere = false;
	public Customer CurrentCustomer;
	public DrinkProfile DrinkOnCoaster;
	public Pickupable MyPickupable;
	public Transform ServedTargetTransform;
	public Vector3 UnservedPos; 
	public List<Pickupable> _pickupablesInMe = new List<Pickupable>();


	void Start()
	{
		EventManager.Instance.Register<DayEndEvent>(ReturnToServePos);
		UnservedPos = transform.position;
		
 		switch (CurrentCustomer)
		{
			case Customer.IvoryDefault:
				MyCustomer = Services.GameManager.CustomerIvory;
				MyCustomer.myCoaster = this;
			break;
			case Customer.SahanaDefault:
				MyCustomer = Services.GameManager.CustomerSahana;
				MyCustomer.myCoaster = this;
			break;
			case Customer.IzzyDefault:
				MyCustomer = Services.GameManager.CustomerIzzy;
				MyCustomer.myCoaster = this;
				break;
			case Customer.JuliaDefault:
				MyCustomer = Services.GameManager.CustomerJulia;
				MyCustomer.myCoaster = this;
				break;
			case Customer.ShayDefault:
				MyCustomer = Services.GameManager.CustomerShay;
				MyCustomer.myCoaster = this;	
				break;
			default:
				break;
		}

		if(DrinkOnCoaster != null){
			IsDrinkHere = true;
		} else {
			IsDrinkHere = false;
		}

		if(!IsDrinkHere){
			MyCustomer.ResetDrinkScore();
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
//				if (_liquid.transform.parent != null)
//				{
//				  if(_liquid.transform.parent.GetComponent<Glass>() != null)
//					  MyPickupable = _liquid.transform.parent.GetComponent<Glass>();			  
//				}
				
				float drinkDeviation = DrinkProfile.GetProfileDeviation(_cocktail, _currentOrder);
				int getIceValue = 0;

				if (_currentOrder == null)
				{
					Debug.Log("WARNING! current order is null!");
				}
				if(_cocktail.ice == _currentOrder.ice && _cocktail.ice == 1 && _currentOrder.ice == 1){ //_cocktail.ice will never be 0, so if they're equal, it's 1 or -1
					getIceValue = 1;
				} else if(_currentOrder.ice == 0 //customer doesn't care, so ice  or no ice, it's fine.
				 || (_cocktail.ice == -1 && _currentOrder.ice == -1)){ //if player successfully fulfills the "no ice" order (-1, -1)
					getIceValue = 0;  
				}

			if (_cocktail.totalVolume >= _minAcceptableVolume)
			{
				if(drinkDeviation <= 0.5f && drinkDeviation > 0){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 1 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars (90, 100);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars(95, 100);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 2 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(90, 50);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars(95, 50);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
						//true when you drop a whole bottle or an empty glass.
//						Debug.Log("Case 3 true!");	
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(90, 0);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars(95, 0);
							MyCustomer.InitiateDialogue();
						}
					} 
				} else if (drinkDeviation > 0.5f && drinkDeviation <= 1.5f){
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 4 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars (80, 100);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars (85, 100);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 5 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(80, 50);
							MyCustomer.InitiateDialogue();
						}
						else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars (85, 100);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 6 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(80, 0);
							MyCustomer.InitiateDialogue();
							StartCoroutine(DelayedInitiateDialogue(0.01f)); 
						} else if (getIceValue == -1 || getIceValue == 1 ){
							MyCustomer.SetCustomerVars(85, 0);
							MyCustomer.InitiateDialogue();
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
							MyCustomer.SetCustomerVars (50, 100);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars (55, 100);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(50, 50);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == 1 || getIceValue == -1){
							MyCustomer.SetCustomerVars(55, 50);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(50, 0);
							MyCustomer.InitiateDialogue();
						}
						else if (getIceValue == 1 || getIceValue == -1){
							Debug.Log(_cocktail.alcoholicStrength);
							MyCustomer.SetCustomerVars(55, 0);
							MyCustomer.InitiateDialogue();
						}
					}    	
				}
				else if (drinkDeviation > 1.95f) //no whiskey at all, acceptable amt of drink
				{
					if(_cocktail.alcoholicStrength >= 0.25f){
//						Debug.Log("Case 7 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars (30, 100);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == -1 || getIceValue == 1){
							MyCustomer.SetCustomerVars (35, 100);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.25f && _cocktail.alcoholicStrength >= 0.10f){
//						Debug.Log("Case 8 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(30, 50);
							MyCustomer.InitiateDialogue();
						}
						else if (getIceValue == -1 || getIceValue == 1){
							MyCustomer.SetCustomerVars (35, 50);
							MyCustomer.InitiateDialogue();
						}
					} else if (_cocktail.alcoholicStrength < 0.10f){
//						Debug.Log("Case 9 true!");
						if(getIceValue == 0){
							MyCustomer.SetCustomerVars(30, 0);
							MyCustomer.InitiateDialogue();
						} else if (getIceValue == -1 || getIceValue == 1){
							MyCustomer.SetCustomerVars(35, 0);
							MyCustomer.InitiateDialogue();
						}
					}
				}
				
				else if (drinkDeviation <= 0)
				{
 					MyCustomer.SetCustomerVars(50, 0);
					MyCustomer.InitiateDialogue();

 				}

				//no whiskey at all in glass, but acceptable amount of drink
			}
			 else if (_cocktail.totalVolume < _minAcceptableVolume && _cocktail.totalVolume > 0) //not enough drink in glass
			 {
				 MyCustomer.SetCustomerVars(10, 0);
				 MyCustomer.InitiateDialogue();
			 }
			  
			 else if (_cocktail.totalVolume <= 0){ //empty glass or bottle
				MyCustomer.SetCustomerVars(0, 0);
				MyCustomer.InitiateDialogue();
			}				 
		} 
 	}

	public void TakeOrder (DrinkProfile _customerOrder)
	{
		_currentOrder = _customerOrder;
	}

	private void CheckDropzoneStatus(){
		if(!GetComponentInChildren<Dropzone>().isOccupied){
			IsDrinkHere = false;
		}
		else
		{
			IsDrinkHere = true;
		}
	}

	IEnumerator DelayedInitiateDialogue(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		MyCustomer.InitiateDialogue();
	}

	public void ReturnToServePos(GameEvent e)
	{
		DayEndEvent dayEndEvent = e as DayEndEvent;
		transform.position = UnservedPos;
	}
	
	
}


