using BehaviorTree;
using Boo.Lang;
using Rewired;
using UnityEngine;
using DG.Tweening;

//ISSUE: It is possible to pick up an object as it's tweening. 
//Effect is the other hand thinks it has picked something up.
//Solution: needs a reference to the other hand to say it's tweening

public class Hand : MonoBehaviour
{
	// tween values
	private List<Sequence> _startPourSeqList = new List<Sequence>();
	private List<Sequence> _endPourSeqList = new List<Sequence>();
	
	[SerializeField] private Transform _pickupMarker;
	[SerializeField]private float _pickupDropTime = 0.75f;
	private float _shortPressTime = 0.5f;
	private float _longPressTime = 1f;
	private HandManager _handManager;
	private bool _isTweening;
	private bool _isPouring = false;
	private bool _isDropping;
	
	public bool IsTweening
	{
		get { return _isTweening; }
	}

	//references to objects for pickup
	public Pickupable SeenPickupable;

	public Bottle HeldBottle;
	
	public Pickupable HeldPickupable;

	public Rag HeldRag;
	public Glass HeldGlass;
	public Vector3 DropPos;
	
	//reference to other hand

	[SerializeField]private Hand _otherHand;

	//behavior tree
	private Tree<Hand> _actionTree;
	private Tree<Hand> _uiTree;
	private FSM<Hand> _fsm;
	
	//reference to crosshair
	private Crosshair _crosshair;

	//misc bools
	[SerializeField] private bool _canPickup;
//	[SerializeField] private bool _canDrop;

//	public bool CanDrop
//	{
//		get { return _canDrop; }
//		set { _canDrop = value; }
//	} 
	
	public enum MyHand
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
		_crosshair = Services.GameManager.player.GetComponent<Crosshair>();

		//We get reference to the OtherHands. 
		if (_myHand == MyHand.Left)
		{
			_otherHand = _handManager.RightHand;
		}
		else
		{
			_otherHand = _handManager.LeftHand;
		}

		//BEHAVIOR TREE NODES
		_actionTree = new Tree<Hand>(new Selector<Hand>(
			
			//EMPTY behavior (hand is not holding anything)

			new Sequence<Hand>(
				new IsUseButtonDown(),
				new IsEmpty(),
				new IsLookingAtPickupable(),
//				new IsPickupableOnCoaster(),
				new Not<Hand>(new IsPickupableServed()),
//				new Not<Hand>(new IsCustomerTalking()),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
				new PickupAction()
//				new CanDrop()
			),

			//HELD behavior
			
			//1. Holding glass
			new Sequence<Hand>(
				new IsUseButtonDown(),
				new IsHoldingPickupable(),
				new IsHoldingGlass(),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
//				new Not<Hand>(new Is),
//				new DisallowPickup(), //can't pick up if holding something
				new IsInInteractionRange(),
				new Not<Hand>(new IsLookingAtCoaster()),
				new DropAction()
			),
			//2. Holding Bottle
			
			// 2.1. Drop Bottle
			new Sequence<Hand>(
				new IsUseButtonDown(),
				new IsHoldingPickupable(),
				new IsHoldingBottle(),
				new Not<Hand>(new IsPouring()),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
				new IsInInteractionRange(),
				new Not<Hand>(new IsLookingAtGlass()),
				new Not<Hand>(new IsLookingAtCoaster()),
				new DropAction()
			),
			
			// 2.2. Pouring
			new Sequence<Hand>(
				new IsUseButtonHeldDown(),
				new IsHoldingBottle(),
				new IsLookingAtGlass(),
//				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsDropping()),
				new Not<Hand>(new IsPouring()), 
				new PourTween()
			),
			
			//POUR won't end if you let go of Use Left/Right when IsPouring is still false, or before pouring actually starts.
			//So EndPourTween() is never called. 
			new Sequence<Hand>(
				new DidPourTweenStart(),
				new Not<Hand>(new IsDropping()),
				new IsUseButtonUp(),
				new EndPourTween()
			),
					
			new Sequence<Hand>(
				new IsPouring(),
				new IsLookingAtGlass(),
				new AddIngredientToGlass()
			),
			
			//Looking At Coaster
			//Bottle to Coaster Drop
			new Sequence<Hand>(
				new IsUseButtonDown(),
				new IsHoldingBottle(),
				new Not<Hand>(new IsPouring()),
				new Not<Hand>(new IsTweenActive()),
				new IsInInteractionRange(),
				new Not<Hand>(new IsLookingAtGlass()),
				new IsLookingAtCoaster(),
				new Not<Hand>(new IsCoasterOccupied()),
				new Not<Hand>(new IsOtherHandTweening()),
				new CoasterDropAction()				
			),
			
			//Glass to Coaster Drop
			new Sequence<Hand>(
				new IsUseButtonDown(),
				new IsHoldingGlass(),
				new Not<Hand>(new IsTweenActive()),
				new IsInInteractionRange(),
				new IsLookingAtCoaster(),
				new Not<Hand>(new IsCoasterOccupied()),
				new Not<Hand>(new IsOtherHandTweening()),
//				new Not<Hand>(new IsCoasterPreOccupied()),
				new CoasterDropAction()
			)
		));
		
		_uiTree = new Tree<Hand>(new Selector<Hand>(
			//Show UI
			
			//Hand is empty, can pick up.
			new Sequence<Hand>(
				new IsEmpty(),
				new IsLookingAtPickupable(),
				new Not<Hand>(new IsPickupableServed()),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
				new ShowPickupUi()
			),	
			
			//Hand is holding glass, can drop
			new Sequence<Hand>(
				new IsHoldingPickupable(),
				new IsHoldingGlass(),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
//				new DisallowPickup(), //can't pick up if holding something
				new IsInInteractionRange(),
				new Not<Hand>(new IsLookingAtCoaster()),
				new ShowDropUi()
			),
			
			//Hand is holding bottle, pour possible
			new Sequence<Hand>(
//				new IsHoldingPickupable(),
				new IsHoldingBottle(),
				new IsLookingAtGlass(),
				new ShowPourUi()
//				new EndPourAction()
			),
			
			//Hand is holding bottle, pour impossible, drop possible
			new Sequence<Hand>(
				new IsHoldingPickupable(),
				new IsHoldingBottle(),
				new Not<Hand>(new IsTweenActive()),
				new Not<Hand>(new IsOtherHandTweening()),
				new IsInInteractionRange(),
				new Not<Hand>(new IsLookingAtGlass()),
				new Not<Hand>(new IsLookingAtCoaster()),
				new ShowDropUi()
//				new PourAction()
			),
			
			// if all of the above fail, hide UI.
			new Sequence<Hand>(
				new HideUi()
			)
		));
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnUpdate()
	{
		_actionTree.Update(this);
		_uiTree.Update(this);

//		if (_myHand == MyHand.Left)
//		{
//			LeftHandInteractions();
//		}
//		else
//		{
//			RightHandInteractions();
//		}
	}

	public virtual void PickupObject(Vector3 newPos)
	{
		if (SeenPickupable != null)
		{
			Debug.Log("Picking up object!");
			_isTweening = true;
			SeenPickupable.transform.SetParent(_handManager.FirstPersonCharacter.transform);
			SeenPickupable.transform.localRotation = _pickupMarker.localRotation;
			Pickupable _myPickupable = SeenPickupable;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(SeenPickupable.transform.DOLocalMove(newPos, _pickupDropTime));
			sequence.AppendCallback(() => HeldPickupable.PickedUp = true);
			sequence.AppendCallback(() => HeldPickupable = _myPickupable);
//			sequence.AppendCallback(() => HeldPickupable.ChangeToFirstPersonLayer(_pickupDropTime));
			sequence.OnComplete(() => _isTweening = false);
		}
	}

	public virtual void DropObject(Vector3 newPos)
	{
		if (HeldPickupable != null)
		{
//			Debug.Log("Dropping object to " + newPos);
			_isTweening = true;
			_isDropping = true;
			HeldPickupable.transform.SetParent(null);
			HeldPickupable.transform.rotation = Quaternion.identity;
			Sequence dropSequence = DOTween.Sequence();
			dropSequence.AppendCallback(() => HeldPickupable.PickedUp = false);
			dropSequence.Append(HeldPickupable.transform.DOMove(newPos, _pickupDropTime));
//			dropSequence.AppendCallback(() => HeldPickupable.ChangeToWorldLayer(_pickupDropTime));
			dropSequence.AppendCallback(() => HeldPickupable = null);
			dropSequence.AppendCallback(() => _isDropping = false);
			dropSequence.OnComplete(() => _isTweening = false);
			
		}
	}
	
	public void CompletePourTween()
	{
		_isPouring = true;
		_isTweening = false;
//		Debug.Log("Pouring begins!");
	}

	public void Pour(Bottle bottleInHand)
	{
		if (_endPourSeqList.Count > 0)
		{
			foreach (var sequence in _endPourSeqList)
			{
				sequence.Kill();
			}
//			Debug.Log("Killing end tweeens!");
		}
		else
		{
//			Debug.LogError("No end pour tweens to kill!");
		}
		_endPourSeqList.Clear();
		if (_myHand == MyHand.Left)
		{
//			_handManager.SeenGlass.Liquid.AddIngredient(bottleInHand.myDrinkBase);
			if (bottleInHand != null
			    && _startPourSeqList.Count == 0)
			{
				_isTweening = true;
				Sequence moveSequence = DOTween.Sequence();
				if (!_startPourSeqList.Contains(moveSequence))
				{
					_startPourSeqList.Add(moveSequence);
				}
				moveSequence.Append(bottleInHand.transform.DOLocalMove(bottleInHand.leftHandPourPos, 0.75f));
				moveSequence.OnComplete(() => CompletePourTween());
				Sequence rotateSequence = DOTween.Sequence();
				if (!_startPourSeqList.Contains(rotateSequence))
				{
					_startPourSeqList.Add(rotateSequence);				
				}
				rotateSequence.Append(bottleInHand.transform.DOLocalRotate(bottleInHand.leftHandPourRot, 0.75f));
//				Debug.Log("Pour tween created!");
			}
		}
		else
		{
//			_handManager.SeenGlass.Liquid.AddIngredient(bottleInHand.myDrinkBase);
			if (bottleInHand != null
			    && _startPourSeqList.Count == 0)
			{
				_isTweening = true;
				Sequence moveSequence = DOTween.Sequence();
				if (!_startPourSeqList.Contains(moveSequence))
				{
					_startPourSeqList.Add(moveSequence);
				}
				moveSequence.Append(bottleInHand.transform.DOLocalMove(bottleInHand.rightHandPourPos, 0.75f));
				moveSequence.OnComplete(() => CompletePourTween());
				Sequence rotateSequence = DOTween.Sequence();
				if (!_startPourSeqList.Contains(rotateSequence))
				{
					_startPourSeqList.Add(rotateSequence);				
				}
				rotateSequence.Append(bottleInHand.transform.DOLocalRotate(bottleInHand.rightHandPourRot, 0.75f));
//				Debug.Log("Pour tween created!");
			}
		}
		Debug.Log("Pouring!");
	}

	public void EndPour(Bottle bottleInHand)
	{
		if (bottleInHand != null)
		{
			//first, kill all previous tweens.
			if (_startPourSeqList.Count > 0)
			{
				foreach (var sequence in _startPourSeqList)
				{
					sequence.Kill();
				}
			}
			else
			{
				Debug.LogError("No start pour tweens to kill!");
			}
			_startPourSeqList.Clear();
			_isPouring = false;
			_isTweening = false;
			Sequence moveSequence = DOTween.Sequence();
			if (!_endPourSeqList.Contains(moveSequence))
			{
				_endPourSeqList.Add(moveSequence);			
			}
			moveSequence.OnPlay(() => _isTweening = true);
			moveSequence.Append(bottleInHand.transform.DOLocalMove(transform.localPosition, 0.75f));
			moveSequence.OnComplete(() => _isTweening = false);
			Sequence rotateSequence = DOTween.Sequence();
			if (!_endPourSeqList.Contains(rotateSequence))
			{
				_endPourSeqList.Add(rotateSequence);
			}
			rotateSequence.Append(bottleInHand.transform.DOLocalRotate(transform.localEulerAngles, 0.75f));
			Debug.Log("Pour tween ending!");
		}
		
	}
	
	//

	//conditions
	private class IsDropping : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._isDropping;
		}
	}

	private class DidPourTweenStart : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._startPourSeqList.Count > 0;
		}
	}

	private class IsPouring : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._isPouring;
		}
	}

	private class IsPickupableServed : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._handManager.SeenPickupable.IsServed;
		}
	}

	/*private class IsPickupableOnCoaster : Node<Hand>
	{
		public override bool Update(Hand context)
		{
//			return context._handManager.SeenPickupable
//			Debug.Log("Is SeenPickupable on coaster? " + context._handManager.SeenPickupable.IsOnCoaster);
			return context._handManager.SeenPickupable.IsOnCoaster;
		}
	}

	private class IsCustomerTalking : Node<Hand>
	{
		public override bool Update(Hand context)
		{
//			Debug.Log("Is Customer Talking? " + Services.GameManager.dialogue.isDialogueRunning);
			return Services.GameManager.dialogue.isDialogueRunning;
		}
	}*/

	private class IsCoasterOccupied : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._handManager.Coaster.GetComponent<Coaster>().IsOccupied;
		}
	}

	private class IsOtherHandTweening : Node<Hand>
	{
		public override bool Update(Hand context)
		{
 			return context._otherHand.IsTweening;
		}
	}

	private class IsLookingAtCoaster : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._handManager.IsLookingAtCoaster;
		}
	}

	private class IsLookingAtPickupable : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.SeenPickupable != null)
			{
//				context._crosshair.ShowPickupUi(context._myHand);
				return true;
			}
//			context._crosshair.HideUi(context._myHand);
			return false;
		}
	}
	
	private class IsLookingAtGlass : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._handManager.IsLookingAtGlass)
			{
//				context._crosshair.ShowPourUi(context._myHand);
			}
			else
			{
//				context._crosshair.HideUi(context._myHand);
			}
			return context._handManager.IsLookingAtGlass;
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

	private class IsHoldingPickupable : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context.HeldPickupable != null)
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
				{
					return true;			
				}
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
				{
					return true;
				} 
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
	
	private class IsTweenActive : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return context._isTweening;
		}
	}

	private class IsInInteractionRange : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._handManager.IsInDropRange)
			{
//				context._crosshair.ShowDropUi(context._myHand);
			}
			else
			{
//				context._crosshair.HideUi(context._myHand);	
			}
			return context._handManager.IsInDropRange;
		}
	}

	private class IsUseButtonDown : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
//				return context._rewiredPlayer.GetButtonTimedPressUp("Use Left", 0f, context._shortPressTime);
				return context._rewiredPlayer.GetButtonDown("Use Left");
			}
			return context._rewiredPlayer.GetButtonDown("Use Right");
//			return context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime);
		}
	}
	
	private class IsUseButtonHeldDown : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
				return context._rewiredPlayer.GetButton("Use Left");
			}
			return context._rewiredPlayer.GetButton("Use Right");
		}
	}

	private class IsUseButtonUp : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
//				return context._rewiredPlayer.GetButtonUp("Use Left");
				return context._rewiredPlayer.GetButtonUp("Use Left");		
			}
//			return context._rewiredPlayer.GetButtonUp("Use Right");
			return context._rewiredPlayer.GetButtonUp("Use Right");			
		}
	}
	
	private class IsLongPressingUseButton : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
				return context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime);		
			}
			return context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime);	
		}
	}

	private class IsLongPressingUseButtonUp : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			if (context._myHand == MyHand.Left)
			{
//				return context._rewiredPlayer.GetButtonUp("Use Left");
				return context._rewiredPlayer.GetButtonTimedPressUp("Use Left", context._longPressTime);		
			}
//			return context._rewiredPlayer.GetButtonUp("Use Right");
			return context._rewiredPlayer.GetButtonTimedPressUp("Use Right", context._longPressTime);			
		}
	}
	


//UI Nodes

	private class ShowDropUi : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._crosshair.ShowDropUi(context._myHand);
			return true;
		}
	}

	private class ShowPickupUi : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._crosshair.ShowPickupUi(context._myHand);
			return true;
		}
	}

	private class ShowPourUi : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._crosshair.ShowPourUi(context._myHand);
			return true;
		}
	}

	private class HideUi : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._crosshair.HideUi(context._myHand);
			return true;
		}
	}
	
//Action
	
	private class PickupAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context.PickupObject(context._pickupMarker.localPosition);
			context.SeenPickupable.ChangeToFirstPersonLayer(context._pickupDropTime);					
			if (context.SeenPickupable.IsOnCoaster)
			{
				context.SeenPickupable.IsOnCoaster = false;
				context.SeenPickupable.IsServed = false;
			}
			return true;

//			if (context._myHand == MyHand.Left)
//			{
//				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Left", 0f, context._shortPressTime))
//				{
//					context.PickupObject(context._pickupMarker.localPosition);
//					context.SeenPickupable.ChangeToFirstPersonLayer(context._pickupDropTime);					
//		
//					if (context.SeenPickupable.IsOnCoaster)
//					{
//						context.SeenPickupable.IsOnCoaster = false;
//						context.SeenPickupable.IsServed = false;
//					}
//
//				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
//				{
//				}
//			}
//			else
//			{
//				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
//				{
//					context.PickupObject(context._pickupMarker.localPosition);
//					context.SeenPickupable.ChangeToFirstPersonLayer(context._pickupDropTime);
//								
//					if (context.SeenPickupable.IsOnCoaster)
//					{
//						context.SeenPickupable.IsOnCoaster = false;
//						context.SeenPickupable.IsServed = false;
//					}
//				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
//				{
//				}
//			}
//			return true;
		}
	}

	private class CoasterDropAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context.DropObject(context._handManager.CoasterPosition);
			context._handManager.Coaster.GetComponent<Coaster>().IsOccupied = true;
			if (context._myHand == MyHand.Left)
			{
				context._handManager.LeftHand.HeldPickupable.IsOnCoaster = true;
			}
			else
			{
				context._handManager.RightHand.HeldPickupable.IsOnCoaster = true;
			}
			context.HeldPickupable.ChangeToWorldLayer(context._pickupDropTime);
			return true;
			
//			if (context._myHand == MyHand.Left)
//			{
//				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Left", 0f, context._shortPressTime))
//				{
// 					context.DropObject(context._handManager.CoasterPosition);
//					context._handManager.Coaster.GetComponent<Coaster>().IsOccupied = true;
//					context._handManager.LeftHand.HeldPickupable.IsOnCoaster = true;
//					context.HeldPickupable.ChangeToWorldLayer(context._pickupDropTime);
//				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
//				{
//				}
//			}
//			else
//			{
//				if (context._rewiredPlayer.GetButtonTimedPressUp("Use Right", 0f, context._shortPressTime))
//				{
//					context.DropObject(context._handManager.CoasterPosition);
//					context._handManager.Coaster.GetComponent<Coaster>().IsOccupied = true;
//					context._handManager.RightHand.HeldPickupable.IsOnCoaster = true;
//					context.HeldPickupable.ChangeToWorldLayer(context._pickupDropTime);
//				} else if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
//				{
//				}
//			}
//			return true;
		}
	}
	
	private class DropAction : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context.DropObject(context.DropPos);
			context.HeldPickupable.ChangeToWorldLayer(context._pickupDropTime);
			Debug.Log("Dropping object!");
			return true;
		}
	}

	private class PourTween : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context.Pour(context.HeldBottle);     
			return true;
			
//			switch (context._myHand)
//			{
//				case MyHand.Left:
//					if (context._rewiredPlayer.GetButtonTimedPress("Use Left", context._longPressTime))
//					{
// 						context.Pour(context.HeldBottle);     
//					}
//					break;
//				case MyHand.Right:
//					if (context._rewiredPlayer.GetButtonTimedPress("Use Right", context._longPressTime))
//					{
// 						context.Pour(context.HeldBottle);
//					}
//					break;
//			}
//			return true;
		}
	}

	private class EndPourTween : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context.EndPour(context.HeldBottle);
			return true;
		}
	}

	private class AddIngredientToGlass : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._handManager.SeenGlass.Liquid.AddIngredient(context.HeldBottle.myDrinkBase);
			return true;
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

