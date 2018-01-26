using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour {

	public delegate void OnMessageReceived(); 
	public event OnMessageReceived onComplete;

	public bool tweensAreActive;


	// Use this for initialization
	void Start () {
  	}
	
	// Update is called once per frame
	void Update () {		
 	}
	
}
