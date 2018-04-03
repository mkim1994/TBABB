using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMaker : MonoBehaviour {

	public Spawner iceSpawner;

	public GameObject iceSpawnpoint;
	
 	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnIce()
	{
		iceSpawner.SpawnIce();
	}
}
