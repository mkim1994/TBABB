using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : MonoBehaviour
{
	private DayCycleManager dayCycleManager;

	[SerializeField] private GameObject blackout;
	// Use this for initialization
	void Start ()
	{
		dayCycleManager = Services.GameManager.dayManager;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EndDay()
	{
		if (dayCycleManager.dayHasEnded)
		{
			dayCycleManager.switchOff = true;
 		}
	}
}
