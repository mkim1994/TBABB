using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
	public GameObject aura;
	public bool isOccupied;
	// Use this for initialization
	Pickupable[] pickupables;
	public bool playerIsLooking;
	[SerializeField]List<GameObject> objectsInMe = new List<GameObject>();

	void Start ()
	{
		playerIsLooking = false;
		aura = transform.GetChild(0).gameObject; 
		aura.SetActive(false);
		pickupables = FindObjectsOfType<Pickupable>();
//		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, Random.Range(0, 359), transform.eulerAngles.z);
//		foreach(var pickupable in pickupables){
// 			if(Vector3.Distance(pickupable.transform.position, this.transform.position) <= 1f){
//				isOccupied = true;				
//			}
//			else
//			 {
//				 isOccupied = false;
//			 }
//		}
		// isOccupied = false;
	}
	
	// Update is called once per frame

	void Update()
	{
	}

	void OnTriggerStay(Collider trigger)
	{
		if (trigger.gameObject.GetComponent<Pickupable>() != null)
		{
			float distance = Vector3.Distance(trigger.transform.position, transform.position);
//			Debug.Log(trigger.name + " " + distance);
			if (!objectsInMe.Contains(trigger.gameObject) && objectsInMe.Count<1 && distance <= 0.16f)
			{
				objectsInMe.Add(trigger.gameObject);		
	  			isOccupied = true;
			}

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
