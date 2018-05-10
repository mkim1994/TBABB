using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Security.AccessControl;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
	public GameObject aura;
	public bool isOccupied;
	// Use this for initialization
	Pickupable[] pickupables;
	public bool playerIsLooking;
	public bool isForServing;
	public Coaster myCoaster;
	private List<Coaster> coasters = new List<Coaster>();
	[HideInInspector]public List<GameObject> objectsInMe = new List<GameObject>();

	void Start ()
	{
		playerIsLooking = false;
		aura = transform.GetChild(0).gameObject; 
		aura.SetActive(false);
		pickupables = FindObjectsOfType<Pickupable>();
		if (isForServing)
		{
			coasters.AddRange(FindObjectsOfType<Coaster>());
			myCoaster = MyCoaster();
		}
	}
	
	// Update is called once per frame

	void Update()
	{
		
	}

	public Coaster MyCoaster(){
		Coaster nearest = coasters[0];
		float shortestDist = Vector3.Distance(coasters[0].transform.position, transform.position);
		for(int i = 0; i < coasters.Count; i++){			
			if(Vector3.Distance(coasters[i].transform.position, transform.position) <= shortestDist){
				shortestDist = Vector3.Distance(coasters[i].transform.position, transform.position);
				nearest = coasters[i];
			}
		}
		return nearest;
	}

	[SerializeField]private float _distance; 	
	
	void OnTriggerStay(Collider trigger)
	{
		if (trigger.gameObject.GetComponent<Pickupable>() != null && !trigger.gameObject.GetComponent<Pickupable>().isForDropzoneOnly)
		{
			float distance = Vector3.Distance(trigger.transform.position, transform.parent.position);
			_distance = distance;
//			Debug.Log(trigger.name + " " + distance);
			if (!objectsInMe.Contains(trigger.gameObject) && objectsInMe.Count<1 && distance <= 0.16f)
			{
				objectsInMe.Add(trigger.gameObject);
				if (objectsInMe[0].GetComponent<Glass>() != null && isForServing)
				{
					objectsInMe[0].GetComponent<Glass>().isInServeZone = true;
					objectsInMe[0].GetComponent<Glass>().myServiceDropzone = this;
				}
				isOccupied = true;
			}
		}

		if (trigger.gameObject == null)
		{
			isOccupied = false;
		}
	}

	void OnTriggerExit(Collider exiter)
	{
// 		if (exiter.gameObject.GetComponent<Bottle>() != null || exiter.gameObject.GetComponent<Glass>() != null)
		if(exiter.gameObject.GetComponent<Pickupable>() != null)
		{
			if (objectsInMe.Contains(exiter.gameObject))
			{
				objectsInMe.Remove(exiter.gameObject);
				if (exiter.gameObject.GetComponent<Glass>() != null)
				{
					exiter.gameObject.GetComponent<Glass>().isInServeZone = false;
//					exiter.GetComponent<Glass>().myServiceDropzone = null;
				}
				isOccupied = false;
			}
		}
	}

	public void ShowAura()
	{
		if(!aura.activeInHierarchy)
			aura.SetActive(true);
	}

	private void HideAura()
	{
		if (aura.activeInHierarchy)
			aura.SetActive(false);
	}
}
