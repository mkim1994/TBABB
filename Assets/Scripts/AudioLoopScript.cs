using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoopScript : MonoBehaviour
{
	
	private PlayerInput player;

	public bool isPlayerPouring;
	public bool playerAttackPour;
	public bool playerSustainPour;
	public bool playerReleasePour;

	double delay = 0.00001f;
	public AudioClip[] clips;
	private AudioSource[] sources;
	int loops = 1;
	double lastTime;	
	bool nowInSustain;
	double attackStartTime;

	[SerializeField] private bool attack;
	[SerializeField] private bool sustain;
	[SerializeField] private bool release;


	// Use this for initialization
	void Start ()
	{
		player = Services.GameManager.playerInput;
		lastTime = -1;

		sources = new AudioSource[3];
		
		for (int i = 0; i < 3; i++) {
			sources[i] = gameObject.AddComponent<AudioSource>();
			
			sources[i].clip = clips[i];
			print("Clip" + i + " : " + clips[i].length);
		}

		sources[0].loop = false;
		sources[1].loop = true;
		sources[2].loop = false;

		sources[0].playOnAwake = false;
		sources[1].playOnAwake = false;
		sources[2].playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if(player.)

		PourAudioLooper();

	}

	private void PourAudioLooper()
	{
		if(!sources[0].isPlaying && !sources[1].isPlaying && !sources[2].isPlaying){}
		int myInt = 0;
		for (int i = 0; i < 1000000; i++)
		{
			myInt += i;
		}

//		if (sources[2].isPlaying)
//		{
//			sustain = false;
//			release = true;
//		}
		
		if ((player.i_startUseLeft && player.pickupableInLeftHand.GetComponent<Bottle> () != null) || (player.i_startUseRight 
			&& player.pickupableInRightHand.GetComponent<Bottle> () != null))
		{
//			attackStartTime = AudioSettings.dspTime + delay + 0.75f;
			attackStartTime = AudioSettings.dspTime + delay;
			nowInSustain = true;
			sources[0].PlayScheduled(attackStartTime);
			sources[1].PlayScheduled(attackStartTime + sources[0].clip.length);
			sources[1].volume = 1;
 			sources[1].loop = true;
		}

		if (((player.i_useLeft && player.pickupableInLeftHand.GetComponent<Bottle> () != null) || 
		    (player.i_useRight && player.pickupableInRightHand.GetComponent<Bottle> () != null)))
		{
			if (-(attackStartTime - AudioSettings.dspTime) <= sources[0].clip.length)
			{
				attack = true;
			}
			else
			{
				attack = false;
			}

			if (-(attackStartTime - AudioSettings.dspTime) > sources[0].clip.length)
			{
				sustain = true;
			}

			if (-(attackStartTime - AudioSettings.dspTime) > sources[0].clip.length + (sources[1].clip.length * loops)) ;
			{
				sustain = false;
				release = true;
			}
		}

		if (((player.i_endUseLeft && player.pickupableInLeftHand.GetComponent<Bottle> () != null) || 
		(player.i_endUseRight && player.pickupableInRightHand.GetComponent<Bottle> () != null)))
		{
			sources[1].loop = false;

//		LOOP COUNT METHOD
			if (sustain)
			{
				sources[2].PlayScheduled(attackStartTime + sources[0].clip.length + (sources[1].clip.length * loops));
			}
			else if (attack && !sustain)
			{
				sources[1].volume = 0;
				sources[2].PlayScheduled(attackStartTime + sources[0].clip.length);
			}

			loops = 0;
		}

		if (nowInSustain && lastTime > sources[1].time)
		{
			loops += 1;
		}

		lastTime = sources[1].time;
	}
}