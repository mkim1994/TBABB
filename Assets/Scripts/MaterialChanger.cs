using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{

	[SerializeField]private DrinkBase _drinkBase;
	
	
	// Use this for initialization
	void Start ()
	{
		SetMaterial(_drinkBase);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetMaterial(DrinkBase drinkBase)
	{
		switch (drinkBase)
		{
			
		}
	}
}
