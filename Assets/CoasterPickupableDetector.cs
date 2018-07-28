using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterPickupableDetector : MonoBehaviour
{

	private Coaster _myCoasterParent;

	private float _minTimeToDetect = 1; //should be equal to or greater than Pickupable's _pickupDropTime

	private float _timeInsideMe = 0;
	// Use this for initialization
	void Start ()
	{
		if (transform.parent != null)
		{
			_myCoasterParent = transform.GetComponentInParent<Coaster>();		
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerStay(Collider trigger)
	{
		if (trigger.GetComponent<Pickupable>() != null && !trigger.GetComponent<Pickupable>().pickedUp)
		{
			float distance = Vector3.Distance(trigger.transform.position, transform.parent.position);
			_timeInsideMe += Time.deltaTime;
			if (!_myCoasterParent._pickupablesInMe.Contains(trigger.GetComponent<Pickupable>()) 
			    && _myCoasterParent._pickupablesInMe.Count<1 && distance <= 0.16f
			    && _timeInsideMe > _minTimeToDetect)
			{		
				_myCoasterParent._pickupablesInMe.Add(trigger.GetComponent<Pickupable>());
				_myCoasterParent.IsOccupied = true;
			} else if (_myCoasterParent._pickupablesInMe.Count > 0)
			{
				if (trigger.gameObject.GetComponent<Pickupable>().pickedUp)
				{
					_myCoasterParent.IsOccupied = false;
				}
			}
		}
	}
	
	void OnTriggerExit(Collider exiter)
	{
// 		if (exiter.gameObject.GetComponent<Bottle>() != null || exiter.gameObject.GetComponent<Glass>() != null)
		_timeInsideMe = 0;
		if(exiter.gameObject.GetComponent<Pickupable>() != null)
		{
			if (_myCoasterParent._pickupablesInMe.Contains(exiter.GetComponent<Pickupable>()))
			{
				_myCoasterParent.IsOccupied = false;
				_myCoasterParent._pickupablesInMe.Remove(exiter.GetComponent<Pickupable>());
			}
		}
	}
}
