using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMaker : Fixture {

	public Spawner iceSpawner;

	public GameObject iceSpawnpoint;
	public Vector3 iceSpawnpointPos;
	public GameObject glassDropPoint;
	public Vector3 glassDropPos;
	
 	// Use this for initialization
	void Start ()
	{
		MyName = "ice maker";
		glassDropPos = glassDropPoint.transform.position;
		iceSpawnpointPos = iceSpawnpoint.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnIce(int handNum)
	{
		iceSpawner.DoSpawnTaskSequence(handNum);
	}
}
