using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{

	[SerializeField] private Image _crosshairRight;
	[SerializeField] private Image _crosshairLeft;
	[SerializeField] private Text _leftText;
	[SerializeField] private Text _rightText;
	public FSM<Crosshair> fsm;
	private float noTargetAlpha;
	private float targetSightedAlpha;
	private PlayerInput _player;
	private bool _hasGrown = false;
	private bool _hasShrunken = false;
	private float origScale = 0.1f;
	private float newScale = 0.3f;
	private string sinkText = "empty";
	
	private enum CrosshairState {
		Nothing,
		Actionable,
		Pickupable,
		Npc,
		Dropzone
	}

	[SerializeField] private CrosshairState _xhairState;
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

		if (Services.TweenManager.tweensAreActive)
		{
			_crosshairLeft.enabled = false;
			_crosshairRight.enabled = false;
			_leftText.enabled = false;
			_rightText.enabled = false;							
		}
		else
		{
			_crosshairLeft.enabled = true;
			_crosshairRight.enabled = true;
			_leftText.enabled = true;
			_rightText.enabled = true;
		}
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

	[SerializeField]private bool _hasShrunkenLeft = false;
	[SerializeField] private bool _hasGrownLeft = false;
	[SerializeField]private bool _hasGrownRight = false;
	[SerializeField]private bool _hasShrunkenRight = false;
	
	private void ShowCrosshairLeft()
	{
		_hasShrunkenLeft = false;
		if (!_hasGrownLeft)
		{
			ShowCrosshair(_crosshairLeft);
			_hasGrownLeft = true;
		}
	}

	private void ShowCrosshairRight()
	{
		_hasShrunkenRight = false;
		if (!_hasGrownRight)
		{
			ShowCrosshair(_crosshairRight);
			_hasGrownRight = true;
		}
	}

	private void HideCrosshairLeft()
	{
		_hasGrownLeft = false;
		if (!_hasShrunkenLeft)
		{
			HideCrosshair(_crosshairLeft);
			Debug.Log("Hiding left crosshair!");
			_hasShrunkenLeft = true;
		}

	}

	private void HideCrosshairRight()
	{
		_hasGrownRight = false;
		if (!_hasShrunkenRight)
		{
			HideCrosshair(_crosshairRight);
			Debug.Log("Hiding right crosshair!");
			_hasShrunkenRight = true;
		}
	}

	private void ShowCrosshair(Image crosshair)
	{
		crosshair.DOColor(new Color(1, 1, 1, 1), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(crosshair.transform.DOScaleX(newScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(crosshair.transform.DOScaleY(newScale, 0.25f));
	}
	
	public void HideCrosshair(Image crosshair)
	{
		crosshair.DOColor(new Color(1, 1, 1, 0), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(crosshair.transform.DOScaleX(origScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(crosshair.transform.DOScaleY(origScale, 0.25f));	
	}

	private class LookingAtState : FSM<Crosshair>.State
	{
	}

	private class LookingAtNothing : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
// 			Context.ChangeCrosshairAlphaOnTargetLost();
			Context._leftText.text = "";
			Context._rightText.text = "";
			Context.HideCrosshairLeft();
			Context.HideCrosshairRight();
			Context._xhairState = CrosshairState.Nothing;
//			Context.HideCrosshair(Context._crosshairLeft);
//			Context.HideCrosshair(Context._crosshairRight);
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
			} else if (Context._player.targetDropzone != null &&
			           (Context._player.pickupableInLeftHand != null || Context._player.pickupableInRightHand != null))
			{
				TransitionTo<LookingAtDropzone>();
			}
		}
	}

	private class LookingAtPickupable : LookingAtState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context._xhairState = CrosshairState.Pickupable;
			Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
			Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
			Context.ShowCrosshairLeft();
			Context.ShowCrosshairRight();
//			Context.ChangeCrosshairAlphaOnTargetSighted();
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
				Context._rightText.text = "pour";

				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowCrosshair(Context._crosshairLeft);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
					Context._leftText.text = "pick up";
				}
				else
				{
					Context.HideCrosshair(Context._crosshairLeft);
					Context._leftText.text = "";
				}
			} else if (Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				Context.ShowCrosshair(Context._crosshairLeft);
				Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
				Context._leftText.text = "pour";
//				Context.HideCrosshair(Context._crosshairRight);

				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowCrosshair(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
					Context._rightText.text = "pick up";
				}
				else
				{
					Context.HideCrosshair(Context._crosshairRight);
					Context._rightText.text = "pick up";
				}
			}
			else if (!Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowCrosshair(Context._crosshairLeft);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");	
					Context._leftText.text = "pick up";
				}
				else
				{
					Context.HideCrosshair(Context._crosshairLeft);
					Context._leftText.text = "";

				} 
				
				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowCrosshair(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
					Context._rightText.text = "pick up";

				}
				else
				{
					Context.HideCrosshair(Context._crosshairRight);
					Context._rightText.text = "";
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
			Context._xhairState = CrosshairState.Npc;
//			Context._crosshairRight.sprite = UIControls.GetSprite("talk_right");
//			Context._crosshairLeft.sprite = UIControls.GetSprite("talk_left");
//			Context.ChangeCrosshairAlphaOnTargetSighted();
			
 
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
			Context._xhairState = CrosshairState.Actionable;
			if (Context._player.pickupableInLeftHand != null && Context._player.pickupableInRightHand == null)
			{
				if (Context._player.pickupableInLeftHand.GetComponent<Glass>() != null)
				{
					Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
					Context._leftText.text = Context.sinkText;
					Context.ShowCrosshairLeft();
				}
			} 
			
			else if (Context._player.pickupableInRightHand != null && Context._player.pickupableInLeftHand == null)
			{
				if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context._rightText.text = Context.sinkText;
					Context.ShowCrosshairRight();
				}
			}
			
			else if (Context._player.pickupableInLeftHand != null && Context._player.pickupableInRightHand != null)
			{
				//both glass
				if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null && Context._player.pickupableInLeftHand.GetComponent<Glass>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
					Context.ShowCrosshairLeft();
					Context.ShowCrosshairRight();
					Context._leftText.text = Context.sinkText;
					Context._rightText.text = Context.sinkText;
				} else if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null &&
				           Context._player.pickupableInLeftHand.GetComponent<Bottle>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
 					Context.HideCrosshairLeft();
					Context.ShowCrosshairRight();
					Context._leftText.text = "";
					Context._rightText.text = Context.sinkText;
				} else if (Context._player.pickupableInLeftHand.GetComponent<Glass>() != null &&
				           Context._player.pickupableInRightHand.GetComponent<Bottle>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context.HideCrosshairRight();
					Context.ShowCrosshairLeft();
					Context._leftText.text = Context.sinkText;
					Context._rightText.text = "";
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

		public override void OnExit()
		{
			base.OnExit();
 		}
	}

	private class LookingAtDropzone : LookingAtState
	{
		/*
		 * conditions needed for this:
		 * 1. Has to be holding something
		 * 2. looking at a dropzone
		 */
		public override void OnEnter()
		{
			base.OnEnter();
			Context._xhairState = CrosshairState.Dropzone;
			Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
			Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
			if (Context._player.pickupableInLeftHand != null && !Context._player.targetDropzone.isOccupied)
			{
				Context.ShowCrosshairLeft();		
				Context._leftText.text = "drop";
			}
			
			if (Context._player.pickupableInRightHand != null && !Context._player.targetDropzone.isOccupied)
			{
				Context.ShowCrosshairRight();	
				Context._rightText.text = "drop";
			}

		}
		
		public override void Update()
		{
			base.Update();
			if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			}
			else if (Context._player.targetDropzone == null && Context._player.isLookingAtNothing)
			{
				TransitionTo<LookingAtNothing>();
			} 
			else if (Context._player.targetDropzone != null)
			{
				if (Context._player.targetDropzone.isOccupied)
				{
//					if(Context._player.pickupableInLeftHand != null){
//						if(Context._player.pickupableInLeftHand)
//						Context.HideCrosshairLeft();
//					}
				}
			}
		}
	}
}
