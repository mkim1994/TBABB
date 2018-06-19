using BehaviorTree;
using Rewired;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework.Constraints;

public class Hand : MonoBehaviour
{
	// tween values
	[SerializeField] private Transform _pickupMarker;
	private float _pickupDropTime = 0.75f;
	private float _shortPressTime = 0.5f;
	private float _longPressTime = 1f;
	private HandManager _handManager;
	private bool _isTweening;

	public bool IsTweening
	{
		get { return _isTweening; }
		set { _isTweening = value; }
	}

	//references to objects for pickup
	[HideInInspector] public Pickupable SeenPickupable;
	public Pickupable HeldPickupable;
	public Bottle HeldBottle;
	public Rag HeldRag;
	public Glass HeldGlass;
	public Vector3 DropPos;

	//behavior tree
	private Tree<Hand> _tree;
	private FSM<Hand> _fsm;

	//misc bools
	[SerializeField] private bool _canPickup;
	[SerializeField] private bool _canDrop;
	[SerializeField] private bool _canPour;

	public bool CanDrop
	{
		get { return _canDrop; }
		set { _canDrop = value; }
	} 
	
	public bool CanPour
	{
		get { return _canPour; }
		set { _canPour = value; }
	}


	private enum MyHand
	{
		Left,
		Right
	}

	[SerializeField] private MyHand _myHand;

	private Player _rewiredPlayer;

	// Use this for initialization
	void Start()
	{
		_rewiredPlayer = Services.GameManager.playerInput.rewiredPlayer;
		_handManager = GetComponent<HandManager>();

		_tree = new Tree<Hand>(new Selector<Hand>(
			
			//EMPTY behavior (hand is not holding anything)
			new Sequence<Hand>(
				new IsEmpty(),
				new IsLookingAtPickupable(),
				new Not<Hand>(new TweenIsActive()),
				new PickupAction(),
				new DisallowDrop()
			),

			//HELD behavior

			//Holding Bottle
			new Sequence<Hand>(
				new IsHoldingBottle(),
				new Not<Hand>(new TweenIsActive()),
				new IsInDropRange(),
				new DropAction(),
				new IsLookingAtGlass(),
				new PourAction()
			),

			//Holding glass
			new Sequence<Hand>(
				new IsHoldingGlass(),
				new Not<Hand>(new TweenIsActive()),
				new DisallowPickup(), //can't pick up if holding something
				new IsInDropRange(),
				new DropAction()
			)


		));
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnUpdate()
	{
		_tree.Update(this);

		if (_myHand == MyHand.Left)
		{
			LeftHandInteractions();
		}
		else
		{
			RightHandInteractions();
		}
	}

	void LeftHandInteractions()
	{
		if (_rewiredPlayer.GetButtonShortPressUp("Use Left"))
		{
			//Pick Up with left hand
//			PickupObject(_pickupMarker.localPosition);
//			DropObject(DropPos);
 		}
		else if (_rewiredPlayer.GetButtonLongPress("Use Left"))
		{
 		}
	}

	void RightHandInteractions()
	{
		if (_rewiredPlayer.GetButtonShortPressUp("Use Right"))
		{

		}
		else if (_rewiredPlayer.GetButtonLongPress("Use Right"))
		{
			Debug.Log("Long Press RIGHT is held down!");
		}
	}

	public virtual void PickupObject(Vector3 newPos)
	{
		if (SeenPickupable != null)
		{
			_isTweening = true;
			SeenPickupable.transform.SetParent(transform.GetChild(0));
			SeenPickupable.transform.localRotation = _pickupMarker.localRotation;
			Pickupable _myPickupable = SeenPickupable;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(SeenPickupable.transform.DOLocalMove(newPos, _pickupDropTime));
			sequence.AppendCallback(() => HeldPickupable = _myPickupable);
			sequence.OnComplete(() => _isTweening = false);
		}
	}

	public virtual void DropObject(Vector3 newPos)
	{
		if (HeldPickupable != null)
		{
			_isTweening = true;
			HeldPickupable.transform.SetParent(null);
			HeldPickupable.transform.rotation = Quaternion.identity;
			Sequence dropSequence = DOTween.Sequence();
			dropSequence.Append(HeldPickupable.transform.DOMove(DropPos, _pickupDropTime));
			dropSequence.AppendCallback(() => HeldPickupable = null);
			dropSequence.OnComplete(() => _isTweening = false);
			
		}
	}

	public void Pour(Bottle bottleInHand, int handNum)
	{
		//left is 0, right is 1
		if (bottleInHand != null)
		{
			_handManager.SeenGlass.ReceivePourFromBottle(bottleInHand, handNum);		
		}
	}

	//conditions
	private class IsLookingAtPickupable : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.SeenPickupable != null)
			{
				return true;
			}
			return false;
		}
	}

	private class IsEmpty : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.HeldPickupable == null)
			{
				return true;
			}

			return false;
		}
	}

	private class IsHoldingBottle : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.HeldPickupable != null)
			{
				if (context.HeldPickupable.GetComponent<Bottle>() != null)
					return true;
			}
			return false;
		}
	}

	private class IsHoldingGlass : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.HeldPickupable != null)
			{
				if (context.HeldPickupable.GetComponent<Glass>() != null)
					return true;
			}
			return false;
		}
	}

	private class IsDropAllowed : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return Vector3.Distance(context.DropPos, context.transform.position) <= context._handManager._maxInteractionDist;
		}
	}

	private class IsLookingAtGlass : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._handManager.IsLookingAtGlass)
			{
				return true;
			}
			return false;
		}
	}

//Action
	private class DisallowPickup : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._canPickup = false;
			return true;
		}
	}

	private class PickupAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Left", 0f, context._shortPressTime))
				{
					context.PickupObject(context._pickupMarker.localPosition);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
				{
					Debug.Log("PICKUP ACTION NODE: LEFT HAND");	
				}
			}
			else
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
				{
					context.PickupObject(context._pickupMarker.localPosition);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
				{
					Debug.Log("PICKUP ACTION NODE: RIGHT HAND");	
				}
			}
			return true;
		}
	}

	private class DropAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Left", 0f, context._shortPressTime))
				{
					context.DropObject(context.DropPos);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
				{
					Debug.Log("DROP ACTION NODE: Do nothing (left hand)");	
				}
			}
			else
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
				{
					context.DropObject(context.DropPos);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
				{
					Debug.Log("DROP ACTION NODE: Do nothing (right hand)");	
				}
			}
			return true;
		}
	}

	private class PourAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
			{
				context.Pour(context.HeldBottle, 0);	
				Debug.Log("Pouring with " + context._myHand);
			} else if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
			{
				context.Pour(context.HeldBottle, 1);
				Debug.Log("Pouring with " + context._myHand);
			}
			return true;
		}
	}

	private class DisallowDrop: Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._canDrop = false;
			return true;
		}
	}

	private class TweenIsActive : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._isTweening;
		}
	}

	private class IsInDropRange : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._canDrop = context._handManager.IsInDropRange;
			return context._handManager.IsInDropRange;
		}
	}
	
//	private class IsInPickupRange 

	//Finite State Machine
	
	private class HandState : FSM<Hand>.State
	{
		
	}

	private class EmptyState : HandState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			base.Update();
		}
	}

	private class HoldingState : HandState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			base.Update();
		}
	}



}

//NODES

