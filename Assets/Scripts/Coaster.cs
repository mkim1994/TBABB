﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coaster : MonoBehaviour
{
	[SerializeField] float abv = 0;
	[SerializeField] float spiciness = 0;
	[SerializeField] float sweetness = 0;
	[SerializeField] float smokiness = 0;
	[SerializeField] float sourness = 0;
	[SerializeField] float bitterness = 0;
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

		if (myCustomer == Services.GameManager.CustomerIvory)
		{
			if (_liquid.thisCocktail.whiskeyRate >= 0.20f || _liquid.thisCocktail.bitterRate >= 0.10f)
			{
				myCustomer.SetCustomerVars(0, 0, 0, 0);		
			}
		}
	}

	private void DetectCustomer()
	{
		
	}



}
