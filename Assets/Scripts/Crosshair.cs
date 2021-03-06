﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Rewired.Utils.Libraries.TinyJson;

public class Crosshair : MonoBehaviour
{
	public Image _rButton, _lButton,_cButton;
	public Text _r1text, _l1text;
	public Image _crosshairCenter, _crosshairRight,_crosshairLeft;
	public Text _leftText, _rightText;
	private FSM<Crosshair> fsm;
	private float noTargetAlpha;
	private float targetSightedAlpha;
	private PlayerInput _player;
	private bool _hasGrown = false;
	private bool _hasShrunken = false;
	private float origScale = 0f;
	private float newScale = 0.3f;
	private string iceMakerText = "hold to get ice";
	private string sinkText = "hold to wash";
	private string pourText = "hold to pour";

	private HandManager _handManager;
	
	private enum CrosshairState {
		Nothing,
		Actionable,
		Pickupable,
		Npc,
		Dropzone
	}

	[SerializeField] private CrosshairState _xhairState;
	
	[SerializeField]private bool _hasShrunkenLeft = false;
	[SerializeField]private bool _hasGrownLeft = false;
	[SerializeField]private bool _hasGrownRight = false;
	[SerializeField]private bool _hasShrunkenRight = false;
	
	// Use this for initialization
	void Start()
	{
		_handManager = Services.HandManager;
		fsm = new FSM<Crosshair>(this);
//		fsm.TransitionTo<LookingAtNothing>();
		_player = Services.GameManager.playerInput;
		if (_player.isUsingController)
		{
			SetUiToController();
		}
		else
		{
			SetUiToMouseAndKeyboard();
		}

//		ChangeUiOnControllerConnect();
//		ChangeUiOnControllerDisconnect();
	}

	// Update is called once per frame
	void Update () {
//		fsm.Update();

		if (Services.TweenManager.tweensAreActive)
		{
			_crosshairCenter.enabled = false;
			_crosshairLeft.enabled = false;
			_crosshairRight.enabled = false;
			_leftText.enabled = false;
			_rightText.enabled = false;
			_lButton.enabled = false;
			_rButton.enabled = false;
			_r1text.enabled = false;
			_l1text.enabled = false;
		}
		else
		{
			_crosshairCenter.enabled = true;
			_crosshairLeft.enabled = true;
			_crosshairRight.enabled = true;
			_leftText.enabled = true;
			_rightText.enabled = true;
			_rButton.enabled = true;
			_lButton.enabled = true;
			_r1text.enabled = true;
			_l1text.enabled = true;
		}
	}
	
	public void SetUiToController()
	{
		_lButton.sprite = UIControls.GetSprite("icon_trigger1");
		_rButton.sprite = UIControls.GetSprite("icon_trigger1");
		_cButton.sprite = UIControls.GetSprite("icon_x");
//		botCenterImg.GetComponent<Image>().sprite = GetSprite("icon_x");
	}

	public void SetUiToMouseAndKeyboard()
	{
 		_lButton.sprite = UIControls.GetSprite("icon_lmb");
		_rButton.sprite = UIControls.GetSprite("icon_rmb");
		_cButton.sprite = UIControls.GetSprite("icon_rect_key");
//		_.GetComponent<Image>().sprite = UIControls.GetSprite("icon_key");
	}
		
	public void ShowCrosshairLeft()
	{
		_hasShrunkenLeft = false;
		if (!_hasGrownLeft)
		{
			ShowImage(_crosshairLeft);
			_hasGrownLeft = true;
		}
	}

	public void ShowCrosshair(Hand.MyHand myHand)
	{
		if (myHand == Hand.MyHand.Left)
		{
			ShowCrosshairLeft();
		}
		else
		{
			ShowCrosshairRight();
		}
	}

	public void ShowCrosshairRight()
	{
		_hasShrunkenRight = false;
		if (!_hasGrownRight)
		{
			ShowImage(_crosshairRight);
			_hasGrownRight = true;
		}
	}

	public void HideCrosshairLeft()
	{
		_hasGrownLeft = false;
		if (!_hasShrunkenLeft)
		{
			HideImage(_crosshairLeft);
			_hasShrunkenLeft = true;
		}
	}

	public void HideCrosshairRight()
	{
		_hasGrownRight = false;
		if (!_hasShrunkenRight)
		{
			HideImage(_crosshairRight);
			_hasShrunkenRight = true;
		}
	}

	public void ShowImage(Image imgToShow)
	{
		imgToShow.DOColor(new Color(1, 1, 1, 1), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(imgToShow.transform.DOScaleX(newScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(imgToShow.transform.DOScaleY(newScale, 0.25f));
	}
	
	public void HideImage(Image imgToHide)
	{
		imgToHide.DOColor(new Color(1, 1, 1, 0), 0.1f);
		
		Sequence a = DOTween.Sequence();
		a.Append(imgToHide.transform.DOScaleX(origScale, 0.25f));
		
		Sequence b = DOTween.Sequence();
		b.Append(imgToHide.transform.DOScaleY(origScale, 0.25f));	
	}
	
	public void HideUi(Hand.MyHand myHand)
	{
		if (myHand == Hand.MyHand.Left)
		{
			_leftText.text = "";
			HideCrosshairLeft();
			HideImage(_lButton);
		}
		else
		{
			_rightText.text = "";
			HideCrosshairRight();
			HideImage(_rButton);
		}
	}

	public void ShowPourUi(Hand.MyHand myHand)
	{
		ShowCrosshair(myHand);
		if (myHand == Hand.MyHand.Left)
		{
			_leftText.text = "pour" + " " + _handManager.LeftHand.HeldBottle.MyName;
			ShowImage(_lButton);
		}
		else
		{
			_rightText.text = "pour " + _handManager.RightHand.HeldBottle.MyName;
			ShowImage(_rButton);	
		}
	}

	public void ShowDropUi(Hand.MyHand myHand)
	{
		if (myHand == Hand.MyHand.Left)
		{
			ShowCrosshairLeft();		
			_leftText.text = "put back " + _handManager.LeftHand.HeldPickupable.MyName;
			ShowImage(_lButton);
		}
		else
		{
			ShowCrosshairRight();	
			_rightText.text = "put back " + _handManager.RightHand.HeldPickupable.MyName;
			ShowImage(_rButton);	
		}
	}

	public void ShowPickupUi(Hand.MyHand myHand)
	{
		if (myHand == Hand.MyHand.Left)
		{
			_leftText.text = "pick up " + _handManager.SeenPickupable.MyName;
			_crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
			ShowImage(_lButton);
			ShowCrosshairLeft();
		}
		else
		{
			_rightText.text = "pick up " + _handManager.SeenPickupable.MyName;
			_crosshairRight.sprite = UIControls.GetSprite("pickup_right");
			ShowImage(_rButton);
			ShowCrosshairRight();
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
			Context._leftText.text = "";
			Context._rightText.text = "";
			Context.HideCrosshairLeft();
			Context.HideCrosshairRight();
			Context.HideImage(Context._rButton);
			Context.HideImage(Context._lButton);
			Context._xhairState = CrosshairState.Nothing;

 		}

		public override void Update()
		{
			base.Update();

			if (Context._player.iceMaker != null || Context._player.sink != null)
			{
				TransitionTo<LookingAtActionable>();
			} 
//			else if (Context._player.canPourWithLeft)
//			{
//				TransitionTo<LookingAtPickupable>();
//			} 
//			else if (Context._player.canPourWithRight)
//			{
//				TransitionTo<LookingAtPickupable>();
//			} 
			else if (Context._player.pickupable != null)
			{
				TransitionTo<LookingAtPickupable>();
			} 
			else if (Context._player.targetDropzone != null &&
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
			Context.ShowImage(Context._rButton);
			Context.ShowImage(Context._lButton);
			Context.ShowCrosshairLeft();
			Context.ShowCrosshairRight();
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
				Context.ShowImage(Context._crosshairRight);
				Context._crosshairRight.sprite = UIControls.GetSprite("action_right");
				Context._rightText.text = Context.pourText;

				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowImage(Context._crosshairLeft);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");
					Context._leftText.text = "pick up";
				}
				else
				{
					Context.HideImage(Context._crosshairLeft);
					Context.HideImage(Context._lButton);
					Context._leftText.text = "";
				}
			} else if (Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				Context.ShowImage(Context._crosshairLeft);
				Context._crosshairLeft.sprite = UIControls.GetSprite("action_left");
				Context._leftText.text = Context.pourText;
 
				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowImage(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
					Context._rightText.text = "pick up";
				}
				else
				{
					Context.HideImage(Context._crosshairRight);
					Context._rightText.text = "pick up";
					Context.HideImage(Context._rButton);
				}
			}
			else if (!Context._player.canPourWithLeft && !Context._player.canPourWithRight)
			{
				if (Context._player.pickupableInLeftHand == null)
				{
					Context.ShowImage(Context._crosshairLeft);
					Context.ShowImage(Context._lButton);
					Context._crosshairLeft.sprite = UIControls.GetSprite("pickup_left");	
					Context._leftText.text = "pick up";
				}
				else
				{
					Context.HideImage(Context._crosshairLeft);
					Context._leftText.text = "";
					Context.HideImage(Context._lButton);
				} 
				
				if (Context._player.pickupableInRightHand == null)
				{
					Context.ShowImage(Context._crosshairRight);
					Context._crosshairRight.sprite = UIControls.GetSprite("pickup_right");
					Context._rightText.text = "pick up";

				}
				else
				{
					Context.HideImage(Context._crosshairRight);
					Context._rightText.text = "";
					Context.HideImage(Context._rButton);
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
					if (Context._player.sink != null)
					{
						Context._leftText.text = Context.sinkText;					
					} else if (Context._player.iceMaker != null)
					{
						Context._leftText.text = Context.iceMakerText;
					}
					Context.ShowCrosshairLeft();
					Context.ShowImage(Context._lButton);
				}
			} 
			
			else if (Context._player.pickupableInRightHand != null && Context._player.pickupableInLeftHand == null)
			{
				if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					if (Context._player.sink != null)
					{
						Context._rightText.text = Context.sinkText;					
					} else if (Context._player.iceMaker != null)
					{
						Context._rightText.text = Context.iceMakerText;
					}
					Context.ShowCrosshairRight();
					Context.ShowImage(Context._rButton);
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
					Context.ShowImage(Context._rButton);
					Context.ShowImage(Context._lButton);
					if (Context._player.sink != null)
					{
						Context._leftText.text = Context.sinkText;
						Context._rightText.text = Context.sinkText;
					} else if (Context._player.iceMaker != null)
					{
						Context._leftText.text = Context.iceMakerText;
						Context._rightText.text = Context.iceMakerText;
					}

				} else if (Context._player.pickupableInRightHand.GetComponent<Glass>() != null &&
				           Context._player.pickupableInLeftHand.GetComponent<Bottle>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
 					Context.HideImage(Context._lButton);
					Context.HideCrosshairLeft();
					Context.ShowCrosshairRight();
					Context.ShowImage(Context._rButton);
					Context._leftText.text = "";
					if (Context._player.sink != null)
					{
						Context._rightText.text = Context.sinkText;					
					} else if (Context._player.iceMaker != null)
					{
						Context._rightText.text = Context.iceMakerText;
					}
				} else if (Context._player.pickupableInLeftHand.GetComponent<Glass>() != null &&
				           Context._player.pickupableInRightHand.GetComponent<Bottle>() != null)
				{
					Context._crosshairRight.sprite = UIControls.GetSprite("action_right");			
					Context.HideCrosshairRight();
					Context.HideImage(Context._rButton);
					Context.ShowCrosshairLeft();
					Context.ShowImage(Context._lButton);
					Context._rightText.text = "";
					if (Context._player.sink != null)
					{
						Context._leftText.text = Context.sinkText;					
					} else if (Context._player.iceMaker != null)
					{
						Context._leftText.text = Context.iceMakerText;
					}
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
				Context.ShowImage(Context._lButton);
			}
			
			if (Context._player.pickupableInRightHand != null && !Context._player.targetDropzone.isOccupied)
			{
				Context.ShowCrosshairRight();	
				Context._rightText.text = "drop";
				Context.ShowImage(Context._rButton);
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
