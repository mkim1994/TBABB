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

	public void DoSpawnTaskSequence(int handNum)
	{
		StartCoroutine(SpawnIce(handNum));
	}

	public IEnumerator SpawnIce(int handNum)
	{
		switch(handNum){
			case 0:
				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube.GetComponent<Ice>().TweenToLeftGlass();

				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube1 = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube1.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube1.GetComponent<Ice>().TweenToLeftGlass();

				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube2 = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube2.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube2.GetComponent<Ice>().TweenToLeftGlass();
			break;
			case 1:
				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube3 = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube3.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube3.GetComponent<Ice>().TweenToRightGlass();

				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube4 = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube4.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube4.GetComponent<Ice>().TweenToRightGlass();

				yield return new WaitForSeconds(0.1f);
				GameObject newIcecube5 = Instantiate(Resources.Load("Prefabs/icecube"), _iceMaker.iceSpawnpointPos, Quaternion.identity) as GameObject;
				newIcecube5.transform.position = _iceMaker.iceSpawnpointPos;
				newIcecube5.GetComponent<Ice>().TweenToRightGlass();
			break;
			default:
			break;
		}

	
	}

}
