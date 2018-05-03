using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
  
public class ActionUI : MonoBehaviour
{
	private float newScale = 0.4f;
	private float origScale = 0.1f;
	private FSM<ActionUI> fsm;
	private PlayerInput player;
	private enum ActionState
	{
		Nothing,
		NPC,
		Fixture
	}

	[SerializeField] private ActionState _actionState;
	[SerializeField] private Text text;
	[SerializeField] private Image image;
	private string talkText = "talk";
	private string lightText = "end the day";
	private string doorText = "start the day";
	
	// Use this for initialization
	void Start ()
	{
		player = Services.GameManager.playerInput;
		fsm = new FSM<ActionUI>(this);
		fsm.TransitionTo<Nothing>();
 	}
	
	// Update is called once per frame
	void Update () {
		fsm.Update();

		if (!Services.GameManager.dayManager.dayHasEnded)
		{
			lightText = "end the day\n(there are still customers to serve)";
		}
	}
	
	private void ShowImage()
	{
		image.DOColor(new Color(1, 1, 1, 1), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(image.transform.DOScaleX(newScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(image.transform.DOScaleY(newScale, 0.25f));
	}
	
	public void HideImage()
	{
		image.DOColor(new Color(1, 1, 1, 0), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(image.transform.DOScaleX(origScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(image.transform.DOScaleY(origScale, 0.25f));	
	}

	
	private class ActionUiState : FSM<ActionUI>.State {
	}

	private class Nothing : ActionUiState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.text.text = "";
			Context.HideImage();
			Context._actionState = ActionState.Nothing;
		}

		public override void Update()
		{
			base.Update();
			if (Context.player.npc != null)
			{
				TransitionTo<NpcState>();
			} else if (Context.player.backdoor != null || Context.player.lightSwitch != null)
			{
				TransitionTo<FixtureState>();
			} 
		}
	}

	private class NpcState : ActionUiState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.text.text = Context.talkText;
			Context.ShowImage();
			Context._actionState = ActionState.NPC;
		}

		public override void Update()
		{
			if (Context.player.npc == null)
			{
				TransitionTo<Nothing>();
			} 
		}
	}

	private class FixtureState : ActionUiState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.ShowImage();
			Context._actionState = ActionState.Fixture;

			if (Context.player.backdoor != null)
			{
				Context.text.text = Context.doorText;				
			}

			if (Context.player.lightSwitch != null)
			{
				Context.text.text = Context.lightText;

			}
		}

		public override void Update()
		{
			base.Update();
			if (Context.player.backdoor == null && Context.player.lightSwitch == null)
			{
				TransitionTo<Nothing>();
			}
		}
	}

}
