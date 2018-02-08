using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterManager : MonoBehaviour {
	public List<Coaster> coasters = new List<Coaster> ();
	void Start(){
		coasters.AddRange(FindObjectsOfType<Coaster>());
	}

	

	
}
