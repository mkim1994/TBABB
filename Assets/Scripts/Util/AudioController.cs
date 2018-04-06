using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioSource bgmIvory, bgmJulia, bgmSahana, bgmYun, bgmIzzy;
    public AudioSource signhum, dooropen, doorclose, doorbell, spotlightsfx;

    [HideInInspector]
    public AudioSource currentlyPlayingBgm;
    public bool muteAudio;
	// Use this for initialization
	void Start () {
        if(muteAudio){
            bgmIvory.volume = 0f;
            bgmJulia.volume = 0f;
            bgmSahana.volume = 0f;
            bgmYun.volume = 0f;
            bgmIzzy.volume = 0f;
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
