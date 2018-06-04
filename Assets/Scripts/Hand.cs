using BehaviorTree;
using Rewired;
using UnityEngine;
using DG.Tweening;

public class Hand : MonoBehaviour
{
	// tween values
	[SerializeField]private Transform _pickupMarker;
	private float _pickupDropTime = 0.75f;
	private HandManager _handManager;
	private bool _isTweening;
	
	public bool IsTweening
	{
		get { return _isTweening; }
		set { _isTweening = value; }
	}
	
	//references to objects for pickup
	[HideInInspector]public Pickupable SeenPickupable;
	public Pickupable HeldPickupable;
	public Bottle HeldBottle;
	public Rag HeldRag;
	public Glass HeldGlass;
	public Vector3 DropPos;
	
	//behavior tree
	private Tree<Hand> _tree;
	
	//misc bools
	[SerializeField]private bool _canPickup;
	[SerializeField]private bool _canDrop;
	
	public bool CanDrop
	{
		get { return _canDrop; }
		set { _canDrop = value; }
	}


	private enum MyHand
	{
		Left,
		Right
	}
	
	[SerializeField]private MyHand _myHand;

	private Player _rewiredPlayer;

	// Use this for initialization
	void Start ()
	{
		_rewiredPlayer = Services.GameManager.playerInput.rewiredPlayer;
		_handManager = GetComponent<HandManager>();

		_tree = new Tree<Hand>(new Selector<Hand>(

			//EMPTY behavior (hand is not holding anything)
			new Sequence<Hand>(
				new IsEmpty(),
				new Not<Hand>(new TweenIsActive()),
				new AllowPickup(),
				new DisallowDrop()
			),

			//HELD behavior
			new Sequence<Hand>(
				new IsHolding(),
				new Not<Hand>(new TweenIsActive()),
				new DisallowPickup(), //can't pick up if holding something
				new IsInDropRange(),
				new AllowDrop()
			)
			
		));
	}
	
	// Update is called once per frame
	void Update () {
		
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
		if (_rewiredPlayer.GetButtonShortPressDown("Use Left"))
		{
			//Pick Up with left hand
			PickupObject(_pickupMarker.localPosition);
			DropObject(DropPos);
			
		} else if (_rewiredPlayer.GetButtonLongPress("Use Left"))
		{
			Debug.Log("Long Press LEFT is held down!");
		}
	}

	void RightHandInteractions()
	{
		if (_rewiredPlayer.GetButtonShortPressDown("Use Right"))
		{
			
		} else if (_rewiredPlayer.GetButtonLongPress("Use Right"))
		{
			Debug.Log("Long Press RIGHT is held down!");
		}
	}
	
	public virtual void PickupObject(Vector3 newPos){
		if (SeenPickupable != null && _canPickup)
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
		if (_canDrop && HeldPickupable != null)
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

	//conditions
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

	private class IsHolding : Node<Hand>
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

	private class IsDropAllowed : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			return Vector3.Distance(context.DropPos, context.transform.position) <= context._handManager._maxInteractionDist;
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

	private class AllowPickup : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._canPickup = true;
			return true;
		}
	}

	private class AllowDrop : Node<Hand>
	{
		public override bool Update(Hand context)
		{
			context._canDrop = true;
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

	
}

//NODES

