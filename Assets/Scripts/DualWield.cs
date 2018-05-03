using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Antlr4.Runtime.Atn;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;
using UnityEngine.UI;

public class DualWield : ActionUI
{
	private PlayerInput player;
	private enum MyState
	{
		None,
		Writing,
		Pouring
	}

	[SerializeField]private MyState _myState;

	[SerializeField] private Text dualWieldText;
	private string pouringText = "to pour";
	private string writingText = "to sign note";
	
	private FSM<DualWield> fsm;

	[SerializeField] private GameObject dualWieldControls; 
	// Use this for initialization
	void Start ()
	{
		player = Services.GameManager.playerInput;
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
			if (Context.player.pickupableInLeftHand != null)
			{
				if (Context.player.pickupableInRightHand != null)
				{
					if ((Context.player.pickupableInLeftHand.GetComponent<Notepad>() != null &&
						Context.player.pickupableInRightHand.GetComponent<Pen>() != null) ||
						(Context.player.pickupableInLeftHand.GetComponent<Pen>() != null &&
						Context.player.pickupableInRightHand.GetComponent<Notepad>() != null))
					{
						TransitionTo<Writing>();
					}
					else if ((Context.player.pickupableInLeftHand.GetComponent<Bottle>() != null &&
							Context.player.pickupableInRightHand.GetComponent<Glass>() != null) ||
							(Context.player.pickupableInLeftHand.GetComponent<Glass>() != null &&
							Context.player.pickupableInRightHand.GetComponent<Bottle>() != null))
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
			if (!Context.player.isDualWield)
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
			if (!Context.player.isDualWield)
			{
				TransitionTo<NoDualWield>();
			}
		}
	}
}
