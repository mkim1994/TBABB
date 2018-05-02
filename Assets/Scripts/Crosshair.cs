using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.XR.WSA.WebCam;

public class Crosshair : MonoBehaviour
{

	[SerializeField] private Image _crosshairRight;
	[SerializeField] private Image _crosshairLeft;
	public FSM<Crosshair> fsm;
	private float noTargetAlpha;
	private float targetSightedAlpha;
	private PlayerInput _player;
	private bool _hasGrown = false;
	private bool _hasShrunken = false;
	private float origScale = 0.1f;
	private float newScale = 0.3f;
	
	// Use this for initialization
	void Start ()
	{
		fsm = new FSM<Crosshair>(this);
		fsm.TransitionTo<LookingAtNothing>();
		_player = Services.GameManager.playerInput;
 	}
	
	// Update is called once per frame
	void Update () {
		fsm.Update();
	}

	public void ChangeCrosshairAlphaOnTargetSighted()
	{
		_hasShrunken = false;

		if (!_hasGrown)
		{
			_crosshairRight.DOColor(new Color(1, 1, 1, 1), 0.1f);
			_crosshairLeft.DOColor(new Color(1, 1, 1, 1), 0.1f);

			Sequence a = DOTween.Sequence();
			a.Append(_crosshairRight.transform.DOScaleX(newScale, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(_crosshairRight.transform.DOScaleY(newScale, 0.25f));
			
			Sequence c = DOTween.Sequence();
			c.Append(_crosshairLeft.transform.DOScaleX(newScale, 0.25f));
			
			Sequence d = DOTween.Sequence();
			d.Append(_crosshairLeft.transform.DOScaleY(newScale, 0.25f));
			_hasGrown = true;
		}
 	}
	
	public void ChangeCrosshairAlphaOnTargetLost()
	{
		_hasGrown = false;

		if (!_hasShrunken)
		{
			_crosshairRight.DOColor(new Color(1, 1, 1, 0), 0.1f);
			_crosshairLeft.DOColor(new Color(1, 1, 1, 0), 0.1f);
			
			Sequence a = DOTween.Sequence();
			a.Append(_crosshairRight.transform.DOScaleX(origScale, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(_crosshairRight.transform.DOScaleY(origScale, 0.25f));
			
			Sequence c = DOTween.Sequence();
			c.Append(_crosshairLeft.transform.DOScaleX(origScale, 0.25f));
			
			Sequence d = DOTween.Sequence();
			d.Append(_crosshairLeft.transform.DOScaleY(origScale, 0.25f));
 
			_hasShrunken = true;
		}
	}
	
	private void ShowCrosshair(Image crosshair)
	{
		_hasShrunken = false;

		if (!_hasGrown)
		{
			crosshair.DOColor(new Color(1, 1, 1, 1), 0.1f);
			
			Sequence a = DOTween.Sequence();
			a.Append(crosshair.transform.DOScaleX(newScale, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(crosshair.transform.DOScaleY(newScale, 0.25f));
			
			_hasGrown = true;
		}
	}
	
	public void HideCrosshair(Image crosshair)
	{
		_hasGrown = false;

		if (!_hasShrunken)
		{
			crosshair.DOColor(new Color(1, 1, 1, 0), 0.1f);
 			
			Sequence a = DOTween.Sequence();
			a.Append(crosshair.transform.DOScaleX(origScale, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(crosshair.transform.DOScaleY(origScale, 0.25f));

			_hasShrunken = true;
		}
	}

	private class LookingAtState : FSM<Crosshair>.State
	{
	}

	private class LookingAtNothing : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
 			Context.ChangeCrosshairAlphaOnTargetLost();
//			Context.HideCrosshair(Context._crosshairLeft);
//			Context.HideCrosshair(Context._crosshairRight);
			Debug.Log("Looking at nothing!");

		}

		public override void Update()
		{
			base.Update();
			if (Context._player.iceMaker != null || Context._player.sink != null)
			{
				TransitionTo<LookingAtActionable>();
			} else if (Context._player.canPourWithLeft)
			{
				TransitionTo<LookingAtPickupable>();
			} else if (Context._player.canPourWithRight)
			{
				TransitionTo<LookingAtPickupable>();
			} else if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			} 
		}
	}

	private class LookingAtPickupable : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context._crosshairRight.sprite = UIControls.GetSprite("pickup");
			Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
			Context.ChangeCrosshairAlphaOnTargetSighted();
			Debug.Log("Looking at pickupable!");
		}

		public override void Update()
		{
			base.Update();
			if (Context._player.canPourWithRight && Context._player.canPourWithLeft)
			{
				Context._crosshairRight.sprite = UIControls.GetSprite("action_right");
				Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");	
			} else if (Context._player.canPourWithRight && !Context._player.canPourWithLeft)
			{
				Context.ShowCrosshair(Context._crosshairRight);
				Context._crosshairRight.sprite = UIControls.GetSprite("action_right");

				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowCrosshair(Context._crosshairLeft);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
				}
				else
				{
					Context.HideCrosshair(Context._crosshairLeft);
				}
			} else if (Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				Context.ShowCrosshair(Context._crosshairLeft);
				Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
//				Context.HideCrosshair(Context._crosshairRight);

				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowCrosshair(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup");
				}
				else
				{
					Context.HideCrosshair(Context._crosshairRight);
				}
			}
			else if (!Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowCrosshair(Context._crosshairLeft);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");								
				}
				else
				{
					Context.HideCrosshair(Context._crosshairLeft);
				} 
				
				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowCrosshair(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup");
				}
				else
				{
					Context.HideCrosshair(Context._crosshairRight);
				}

			} 

			if (Context._player.iceMaker != null || Context._player.sink != null)
			{
				if (Context._player.pickupable == null)
				{
					TransitionTo<LookingAtActionable>();				
				}
			}
			else if (Context._player.pickupable == null)
			{
				TransitionTo<LookingAtNothing>();
			}
		}
	}

	private class LookingAtNPC : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
//			Context._crosshairRight.sprite = UIControls.GetSprite("talk_right");
//			Context._crosshairLeft.sprite = UIControls.GetSprite("talk_left");
//			Context.ChangeCrosshairAlphaOnTargetSighted();
			
			Debug.Log("Looking at NPC!");

		}
		
		public override void Update()
		{
			base.Update();
			if (Context._player.iceMaker != null || Context._player.sink != null)
			{
				TransitionTo<LookingAtActionable>();
			} else if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			}
			else if (Context._player.npc == null)
			{
				TransitionTo<LookingAtNothing>();
			}
		}
	}

	private class LookingAtActionable : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			if (Context._player.pickupableInLeftHand != null && Context._player.pickupableInRightHand == null)
			{
				if (Context._player.pickupableInLeftHand.GetComponent<Glass>() != null)
				{
					Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
					Context.ShowCrosshair(Context._crosshairLeft);
				}
			} 
			
			else if (Context._player.pickupableInRightHand != null && Context._player.pickupableInLeftHand == null)
			{
				if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context.ShowCrosshair(Context._crosshairRight);
				}
			}
			
			else if (Context._player.pickupableInLeftHand != null && Context._player.pickupableInRightHand != null)
			{
				if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null && Context._player.pickupableInLeftHand.GetComponent<Glass>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
					Context.ChangeCrosshairAlphaOnTargetSighted();
				}
			}

		}
		
		public override void Update()
		{
			base.Update();
			if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			}
			else if (Context._player.iceMaker == null && Context._player.sink == null)
			{
				TransitionTo<LookingAtNothing>();
			}
		}
	}

	private class LookingAtDropzone : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context._crosshairRight.sprite = UIControls.GetSprite("pickup");
			Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
		}
		
		public override void Update()
		{
			base.Update();
			if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			}
			else if (Context._player.isLookingAtNothing)
			{
				TransitionTo<LookingAtNothing>();
			}
		}
	}
}
