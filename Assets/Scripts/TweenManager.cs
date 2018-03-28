using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour {

	public bool tweensAreActive;

	private void FixedUpdate()
	{
		if (tweensAreActive)
		{
//			Debug.Log("Tweens are active!");
		}
		else
		{
//			Debug.Log("Tweens are INACTIVE");
		}
	}
}
