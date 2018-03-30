using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilCoroutines {
	
	public static IEnumerator WaitThenSetTweensToInactive(float delay, PlayerInput.TweenManagerDelegate someMethod)
	{
		yield return new WaitForSeconds(delay);
		someMethod();
 	}

	public static IEnumerator WaitThenPour(float delay, PlayerInput.StartPourDelegate pourMethod, Bottle bottle, int num)
	{
		yield return new WaitForSeconds(delay);
		pourMethod(bottle, num);
	}

}
