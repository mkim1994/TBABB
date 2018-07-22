using BehaviorTree;
using Rewired;
using UnityEngine;
using DG.Tweening;

//ISSUE: It is possible to pick up an object as it's tweening. 
//Effect is the other hand thinks it has picked something up.
//Solution: needs a reference to the other hand to say it's tweening

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
	}

	//references to objects for pickup
	[HideInInspector] public Pickupable SeenPickupable;

	public Bottle HeldBottle;
	
	public Pickupable HeldPickupable;

	public Rag HeldRag;
	public Glass HeldGlass;
	public Vector3 DropPos;
	
	//reference to other hand

	[SerializeField]private Hand _otherHand;

	//behavior tree
	private Tree<Hand> _tree;
	private FSM<Hand> _fsm;

	//misc bools
	[SerializeField] private bool _canPickup;
//	[SerializeField] private bool _canDrop;

//	public bool CanDrop
//	{
//		get { return _canDrop; }
//		set { _canDrop = value; }
//	} 
	
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
		_handManager = Services.HandManager;

		//We get reference to the OtherHands. 
		if (_myHand == MyHand.Left)
		{
			_otherHand = _handManager.RightHand;
		}
		else
		{
			_otherHand = _handManager.LeftHand;
		}

		_tree = new Tree<Hand>(new Selector<Hand>(
			
			//EMPTY behavior (hand is not holding anything)
			//// Pick up object
			new Sequence<Hand>(
				new IsEmpty(),
				new IsLookingAtPickupable(),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
				new PickupAction()
//				new CanDrop()
			),

			//HELD behavior

			////Holding Bottle
			new Sequence<Hand>(
				new IsHoldingBottle(),
				new Not<Hand>(new IsTweenActive()),
				new IsInDropRange(),
				new Not<Hand>(new IsLookingAtGlass()),
				new DropAction()
//				new PourAction()
			),
			
			new Sequence<Hand>(
				new IsHoldingBottle(),
				new IsLookingAtGlass(),
				new PourAction()	
			),

			//Holding glass
			new Sequence<Hand>(
				new IsHoldingGlass(),
				new Not<Hand>(new IsTweenActive()),
//				new DisallowPickup(), //can't pick up if holding something
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

//		if (_myHand == MyHand.Left)
//		{
//			LeftHandInteractions();
//		}
//		else
//		{
//			RightHandInteractions();
//		}
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
			Debug.Log("Long Press RIGHT is held down!");

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
			SeenPickupable.transform.SetParent(_handManager.FirstPersonCharacter.transform);
			SeenPickupable.transform.localRotation = _pickupMarker.localRotation;
			Pickupable _myPickupable = SeenPickupable;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(SeenPickupable.transform.DOLocalMove(newPos, _pickupDropTime));
			sequence.AppendCallback(() => HeldPickupable = _myPickupable);
//			sequence.AppendCallback(() => HeldPickupable.ChangeToFirstPersonLayer(_pickupDropTime));
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
//			dropSequence.AppendCallback(() => HeldPickupable.ChangeToWorldLayer(_pickupDropTime));
			dropSequence.AppendCallback(() => HeldPickupable = null);
			dropSequence.OnComplete(() => _isTweening = false);
			
		}
	}

	public void Pour(Bottle bottleInHand)
	{
		//left is 0, right is 1
		//the one on the right is always first????
		
//			Debug.Log(_handManager.SeenGlass);
//		_handManager.SeenGlass.ReceivePourFromBottle(bottleInHand, handNum);	

		//		if (bottleInHand.myDrinkBase != DrinkBase.none)
//		{
//			_handManager.SeenGlass.Liquid.AddIngredient(bottleInHand.myDrinkBase);		
//		}
//		
//		if (bottleInHand.myMixer != Mixer.none)
//		{
//			_handManager.SeenGlass.Liquid.AddMixer(bottleInHand.myMixer);		
//		}
//		Debug.Log(bottleInHand.myDrinkBase);
		if(bottleInHand != null)
			_handManager.SeenGlass.Liquid.AddIngredient(bottleInHand.myDrinkBase);
	}

	//conditions

	private class IsOtherHandTweening : Node<Hand>
	{
		public override bool Update(Hand context)
		{
 			return context._otherHand.IsTweening;
		}
	}

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
			return context._handManager.IsLookingAtGlass;
		}
	}

//Action
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
				}
			}
			else
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
				{
					context.PickupObject(context._pickupMarker.localPosition);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
				{
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
 				}
			}
			else
			{
				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
				{
					context.DropObject(context.DropPos);
				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
				{
 				}
			}
			return true;
		}
	}

	private class PourAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			switch (context._myHand)
			{
				case MyHand.Left:
					if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
					{
 						context.Pour(context.HeldBottle);     
					}
					break;
				case MyHand.Right:
					if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
					{
 						context.Pour(context.HeldBottle);
					}
					break;
			}
			return true;
		}

	}

	private class CanDrop: Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.HeldPickupable == null)
			{
				return false;
			}
			return true;
		}
	}

	private class IsTweenActive : Node<Hand>
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

