using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class IntroControls : MonoBehaviour
{
	private int playerId = 0;
	private Player player;

	private Intro intro;
	
	// Use this for initialization
	void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		player = ReInput.players.GetPlayer(playerId);
	}

	void Start()
	{
		intro = FindObjectOfType<Intro>();
	}

	// Update is called once per frame
	void Update () {
		if (player.GetButtonDown("StartGame"))
		{
			intro.StartGame();
		}
	}
}
