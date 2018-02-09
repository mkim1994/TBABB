using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioSource bgm, signhum, dooropen, doorclose, doorbell, spotlightsfx;

    public bool muteAudio;
	// Use this for initialization
	void Start () {
        if(muteAudio){
            bgm.volume = 0f;
            signhum.volume = 0f;
            dooropen.volume = 0f;
            doorclose.volume = 0f;
            doorbell.volume = 0f;
            spotlightsfx.volume = 0f;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
