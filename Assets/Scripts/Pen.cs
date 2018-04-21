using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : Pickupable {

	protected override void Start(){
		base.Start();
	}

	public void WriteSomething(){
		Debug.Log("Write something");
	}
}
