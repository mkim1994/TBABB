using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Antlr4.Runtime.Atn;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;
using UnityEngine.UI;

public class DualWield : ActionUI
{
	private PlayerInput _myPlayer;
	private enum MyState
	{
		None,
		Writing,
		Pouring
	}

	[SerializeField]private MyState _myState;

	[SerializeField] private Text dualWieldText;
	private string pouringText = "hold to pour";
	private string writingText = "hold to sign note";
	
	private FSM<DualWield> fsm;

	[SerializeField] private GameObject dualWieldControls; 
	// Use this for initialization
	void Start ()
	{
		_myPlayer = Services.GameManager.playerInput;
		fsm = new FSM<DualWield>(this);
		fsm.TransitionTo<NoDualWield>();
	}
	
	// Update is called once per frame
	void Update () {
		fsm.Update();
	}

	private class DualWieldState : FSM<DualWield>.State
	{
		
	}

	private class NoDualWield : DualWieldState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.dualWieldControls.SetActive(false);
		}

		public override void Update()
		{
			base.Update();
			if (Context._myPlayer.pickupableInLeftHand != null)
			{
				if (Context._myPlayer.pickupableInRightHand != null)
				{
					if ((Context._myPlayer.pickupableInLeftHand.GetComponent<Notepad>() != null &&
						Context._myPlayer.pickupableInRightHand.GetComponent<Pen>() != null) ||
						(Context._myPlayer.pickupableInLeftHand.GetComponent<Pen>() != null &&
						Context._myPlayer.pickupableInRightHand.GetComponent<Notepad>() != null))
					{
						TransitionTo<Writing>();
					}
					else if ((Context._myPlayer.pickupableInLeftHand.GetComponent<Bottle>() != null &&
							Context._myPlayer.pickupableInRightHand.GetComponent<Glass>() != null) ||
							(Context._myPlayer.pickupableInLeftHand.GetComponent<Glass>() != null &&
							Context._myPlayer.pickupableInRightHand.GetComponent<Bottle>() != null))
					{
						TransitionTo<Pouring>();
					}
				}
			}
		}
	}

	private class Writing : DualWieldState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context._myState = MyState.Writing;
			Context.dualWieldControls.SetActive(true);
			Context.dualWieldText.text = Context.writingText;
		}

		public override void Update()
		{
			if (!Context._myPlayer.isDualWield)
			{
				TransitionTo<NoDualWield>();
			}
		}
	}

	private class Pouring : DualWieldState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context._myState = MyState.Pouring;
			Context.dualWieldControls.SetActive(true);
			Context.dualWieldText.text = Context.pouringText;
		}
		
		public override void Update()
		{			
			if (!Context._myPlayer.isDualWield)
			{
				TransitionTo<NoDualWield>();
			}
		}
	}
}
