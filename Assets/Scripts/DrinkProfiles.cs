using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public class DrinkProfiles {
	public float smokyVolume;
	public float sweetVolume;
	public float sourVolume;
	public float bitterVolume;
	public float alcoholVolume; 
	public float spicyVolume;
 
	public DrinkProfiles(float _alcoholVol, float _smokyVol, float _sweetVol, float _sourVol, float _bitterVol){
		alcoholVolume = _alcoholVol;
		smokyVolume = _smokyVol;
		sweetVolume = _sweetVol;
		sourVolume = _sourVol;
		bitterVolume = _bitterVol;
	}
}

// public class DrinkProfileClass : MonoBehaviour {

// 	// Use this for initialization
// 	void Start () {
		
// 	}
	
// 	// Update is called once per frame
// 	void Update () {
		
// 	}
// }
