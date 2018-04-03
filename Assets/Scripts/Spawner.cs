using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
 
public class Spawner : MonoBehaviour
{
	public enum Spawnables
	{
		Glass,
		Ice,
		Garnish
	}

	private IceMaker _iceMaker;

	public Spawnables _spawnables;

	private MR.Task[] _spawnTask;
 
	[SerializeField]private float timer;
	[SerializeField]private float timeBeforeNextIce;
	
	private readonly TaskManager _tm = new TaskManager();
	
	// Use this for initialization
	void Start ()
	{
//		_spawnTask[0] = new ActionTask(SpawnIce);
//		_spawnTask[1] = new Wait(0.1f);
		_iceMaker = FindObjectOfType<IceMaker>();
	}
	
	// Update is called once per frame
	void Update ()
	{
//		timer += Time.deltaTime;
//		if (timer >= timeBeforeNextIce)
//		{
//			SpawnIce();
//			timer = 0;
//		}
	}

	public void DoSpawnTaskSequence()
	{
		Debug.Log("Started task sequence!");
		_tm.Do(new ActionTask(SpawnIce))
			.Then(new Wait(0.1f))
			.Then(new ActionTask(SpawnIce))
			.Then(new Wait(0.1f))
			.Then(new ActionTask(SpawnIce))
			.Then(new Wait(0.1f))
			.Then(new ActionTask(SpawnIce))
			.Then(new Wait(0.1f))
			.Then(new ActionTask(SpawnIce))
			.Then(new Wait(0.1f))
			.Then(new ActionTask(SpawnIce));
	}

	public void SpawnIce()
	{
		switch (_spawnables)
		{
			case Spawnables.Garnish:
//				List<GameObject> leftCreeps = new List<GameObject>();
//				leftCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(-13.83f, 4.7f, 0), Quaternion.identity) as GameObject);
//				leftCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(-12.83f, 4.7f, 0), Quaternion.identity) as GameObject);
//				leftCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(-11.83f, 4.7f, 0), Quaternion.identity) as GameObject);
//				foreach (var creep in leftCreeps)
//				{
////					creep.GetComponent<Ice>().direction = 1f;
//					creep.layer = 10;
////					creep.GetComponent<MeshRenderer>().material = creep.GetComponent<Ice>().blueMat;
////					creep.GetComponent<Ice>().teamNum = 0;
//				}
				break;
			
			case Spawnables.Glass:
//				List<GameObject> rightCreeps = new List<GameObject>();
//				rightCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(15.21f, -4.969f, 0), Quaternion.identity) as GameObject);
//				rightCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(14.21f, -4.969f, 0), Quaternion.identity) as GameObject);
//				rightCreeps.Add(Instantiate(Resources.Load("Prefabs/enemy"), new Vector3(13.21f, -4.969f, 0), Quaternion.identity) as GameObject);
//				foreach (var creep in rightCreeps)
//				{
////					creep.GetComponent<Ice>().direction = -1f;
//					creep.layer = 11;
//					creep.GetComponent<MeshRenderer>().material = creep.GetComponent<Ice>().pinkMat;
//					creep.GetComponent<Ice>().teamNum = 1;
//				}
				break;
			
			case Spawnables.Ice:
				List<GameObject> icecubes = new List<GameObject>();
				icecubes.Add(Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawner.transform.position, Quaternion.identity) as GameObject);
 				break;
			default:
				break;
		}
	}

}
