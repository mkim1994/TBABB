using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Glass : Pickupable
{
	public enum GlassType
	{
		Highball,
		Shot,
		Square,
		Wine_glass,
		Beer_mug
	}

	[SerializeField]private GlassType glassType;

	public enum GlassServeState {
		ReadyToServe,
		NotReadyToServe,
		Served
	}

	public GlassServeState glassServeState;

	private FSM<Glass> fsm;
	[HideInInspector]public GameObject focalPoint;
	[HideInInspector]public List<Ice> myIceList = new List<Ice>();
	public bool isFull;
 	public bool hasIce;
	public bool isDirty;
	public bool isInServeZone = false;
	private Coaster coaster;
	public bool CanBePouredInto;
	public Dropzone myServiceDropzone;
	private Vector3 unservedPos;
	private Vector3 servedPos;
	[HideInInspector]public Liquid Liquid;

	[HideInInspector]public GameObject liquidSurfaceParent;
	[HideInInspector]public GameObject liquidSurfaceChild;

	private Vector3 leftHandPourRot = new Vector3(80f, 0, 6.915f);
	private Vector3 rightHandPourRot = new Vector3(80f, 0, 6.915f);
	public Vector3 leftHandPourPos = new Vector3(-0.14f, -0.5f, 1.75f);
	public Vector3 rightHandPourPos = new Vector3(0.14f, -0.5f, 1.75f);

	public FSM<Glass>.State CurrentState;
	public FSM<Glass>.State ReadyToServeState;
	public FSM<Glass>.State NotReadyToServeState;
	public FSM<Glass>.State ServedState;
	
	private Vector2 _playerLookVec2;
	private float _playerLookSens;
	private float _playerLookX;

	protected override void Start()
	{
		base.Start();
		CreateDropzone();
		origPos = transform.position;
		fsm = new FSM<Glass>(this);
		fsm.TransitionTo<NotReadyToServe>();
		_playerLookSens = Services.GameManager.playerInput.lookSensitivity;
		Liquid = GetComponentInChildren<Liquid>();
		if (Liquid != null)
		{
			Liquid._glassType = glassType;		
		}
	}
	
	public override void Update()
	{	
//		_playerLookVec2 = Services.GameManager.playerInput.lookVector;
		fsm.Update();
		
 		if(pickedUp && !Services.TweenManager.tweensAreActive){
//			transform.rotation = Quaternion.identity;
//			_playerLookX -= _playerLookVec2.y * _playerLookSens;
//			_playerLookX = Mathf.Clamp (_playerLookX, -75f, -25f);
//			transform.localRotation = Quaternion.Euler (_playerLookX, 0, 0);	 
		}

		if(myIceList.Count >= 3){
			Liquid.hasIce = true;
		} else {
			Liquid.hasIce = false;
		}
	
	}


	public bool IsFull
	{
		get
		{
			if (Liquid.totalVolume >= 100f)
			{
				isFull = true;
			}
			else
			{
				isFull = false;
			}

			return isFull;
		}
	}

	public override void InteractLeftHand(){
		if(!pickedUp && CurrentState != ServedState){
			//pick up with left hand
			transform.SetParent(Services.GameManager.player.transform.GetChild(0));
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = this;
			PickupTween(leftHandPos, Vector3.zero);			
		} else if(pickedUp){
			transform.SetParent(null);
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInLeftHand = null;
 
			if(targetDropzone != null){
				DropTween(dropPos, dropOffset, targetDropzone);
			}
		}
	}

	public override void InteractRightHand(){
		if(!pickedUp && CurrentState != ServedState){
			transform.SetParent(Services.GameManager.player.transform.GetChild(0));
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = this;
			PickupTween(rightHandPos, Vector3.zero);			
		} else if(pickedUp){
			transform.SetParent(null);
			Services.GameManager.player.GetComponent<PlayerInput>().pickupableInRightHand = null;
            
			if(targetDropzone != null){
				DropTween(dropPos, dropOffset, targetDropzone);
			}
		}
	}
	
	public void ReceivePourFromBottle(Bottle bottleInHand, int handNum)
	{
		//left hand is 0, right hand is 1		
		if (bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none)
		{
			Debug.Log(bottleInHand.myDrinkBase);
			Liquid.AddIngredient(bottleInHand.myDrinkBase);
			if (pickedUp)
			{
//				base.RotateTween(leftHandPourRot);
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);
				}
				else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);
				}
			}
		}
		else if (bottleInHand.myMixer != Mixer.none && bottleInHand.myDrinkBase == DrinkBase.none)
		{
			Liquid.AddMixer(bottleInHand.myMixer);
			if (pickedUp)
			{
				if (handNum == 0)
				{
					StartPourTween(leftHandPourPos);
				}
				else if (handNum == 1)
				{
					StartPourTween(rightHandPourPos);
				}
			}
		}
	}

	public void StartInHandPourTween(Vector3 pourPos)
	{
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(()=>DeclareActiveTween());
		sequence.Append(transform.DOLocalMove(pourPos, tweenTime, false));
//		sequence.OnComplete(() => DeclareInactiveTween());	
	}

	public void ReceiveInHandPour(Bottle bottleInHand, int handNum)
	{
		if (bottleInHand.myDrinkBase != DrinkBase.none && bottleInHand.myMixer == Mixer.none)
		{
			Liquid.AddIngredient(bottleInHand.myDrinkBase);
		}
		else if (bottleInHand.myMixer != Mixer.none && bottleInHand.myDrinkBase == DrinkBase.none)
		{
			Liquid.AddMixer(bottleInHand.myMixer);
		}
	}

	public override void DropTween(Vector3 dropPos, Vector3 dropOffset, Dropzone _targetDropzone)
	{
		if (_targetDropzone.GetComponentInParent<Coaster>() != null)
		{
		}
		else
		{
			dropOffset = new Vector3(0, 0, 0);
		}

		DeclareActiveTween();
		_targetDropzone.isOccupied = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(dropPos + dropOffset, pickupDropTime, false));
		transform.DOLocalRotate(Vector3.zero, pickupDropTime, RotateMode.Fast);
//		sequence.AppendCallback(() => GetComponent<Collider>().enabled = true);
		sequence.OnComplete(() => DeclareInactiveTween());
		StartCoroutine(ChangeToWorldLayer(pickupDropTime));
		pickedUp = false;
	}

	public override void StartPourTween(Vector3 moveToPos)
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(moveToPos, tweenTime, false));
		sequence.OnComplete(() => DeclareInactiveTween());
	}

	public void EndPourFromBottle()
	{
//		Liquid liquid = GetComponentInChildren<Liquid>();
		Liquid.isBeingPoured = false;
		Liquid.isEvaluated = false;
//		liquid.EvaluateDrinkInCoaster();
//		liquid.isPouring = false;
	}

	public override void EndPourTween()
	{
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalMove(startPos, tweenTime, false));
		sequence.OnComplete(() => DeclareInactiveTween());
	}

	public override void UseLeftHand()
	{
		if (Services.GameManager.playerInput.pickupable.GetComponent<Glass>() != null)
		{
			base.RotateTween(leftHandPourRot);
//			StartPourTween(Vector3.forward + new Vector3(-0.482f, 0, 0.5f));
		}
	}

	public void GenericEmpty()
	{
		Liquid.EmptyLiquid();
		ClearIce();
	}

	public void LeftHandEmptyGlass()
	{	
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		Vector3 moveToPos = Vector3.forward + new Vector3(-0.482f, 0, 0.5f);
		sequence.Append(transform.DOLocalMove(leftHandPourPos, 0.5f, false));
		sequence.Append(transform.DOLocalMove(startPos, 0.5f, false));
		sequence.OnComplete(() => DeclareInactiveTween());
		
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DOLocalRotate(leftHandPourRot, 0.5f, RotateMode.Fast));
		rotateSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast));
		rotateSequence.OnComplete(() => Liquid.EmptyLiquid()); 
		// rotateSequence.OnComplete(()=>ClearIce());
		ClearIce();
 	}
	
	public void RightHandEmptyGlass()
	{	
		DeclareActiveTween();
		Sequence sequence = DOTween.Sequence();
		Vector3 moveToPos = Vector3.forward + new Vector3(-0.482f, 0, 0.5f);
		sequence.Append(transform.DOLocalMove(rightHandPourPos, 0.5f, false));
		sequence.Append(transform.DOLocalMove(startPos, 0.5f, false));
		sequence.OnComplete(() => DeclareInactiveTween());
		
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DOLocalRotate(rightHandPourRot, 0.5f, RotateMode.Fast));
		rotateSequence.Append(transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast));
		rotateSequence.OnComplete(() => Liquid.EmptyLiquid());
		// rotateSequence.OnComplete(()=>ClearIce());
		ClearIce();
//		liquid.empty;
	}
	
	//if has accepted drink is false && isReadyToTalk is true && if coaster is 

	public void ClearIce(){
		foreach (var ice in myIceList){
			//Do stuff
			Destroy(ice.gameObject);
		}
		myIceList.Clear();
	}

	public override IEnumerator ChangeToFirstPersonLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        int children = transform.childCount;
		// Debug.Log("Children: " + children);
        startPos = transform.localPosition;

        if (gameObject.GetComponentInChildren<Liquid>() != null)
        {
             Liquid _liquid = gameObject.GetComponentInChildren<Liquid>();
            _liquid.isEvaluated = false;
        }

		transform.GetChild(0).gameObject.layer = 13;
		transform.GetChild(1).gameObject.layer = 13;
		transform.GetChild(2).gameObject.layer = 13;
		transform.GetChild(3).gameObject.layer = 13;
		transform.GetChild(3).GetChild(0).gameObject.layer = 14;
    }
	public override IEnumerator ChangeToWorldLayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        startPos = transform.localPosition;
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            transform.GetChild(i).gameObject.layer = 0;
        }
		transform.GetChild(3).GetChild(0).gameObject.layer = 0;
    }

	public void Serve()
	{
// 		if (myServiceDropzone != null)
//		{
//			if (myServiceDropzone.MyCoaster().myCustomer.IsCustomerPresent() && CurrentState == ReadyToServeState
//			    && !Services.GameManager.dialogue.isDialogueRunning)
//			{
//				unservedPos = myServiceDropzone.transform.parent.position;
//				servedPos = myServiceDropzone.MyCoaster().ServedTargetTransform.position;		
//				DeclareActiveTween();
//				Sequence sequence = DOTween.Sequence();
//				myServiceDropzone.MyCoaster().transform.DOLocalMove(servedPos, 0.5f);
//				sequence.Append(transform.DOLocalMove(servedPos + dropOffset, 0.5f, false));
//				sequence.AppendCallback(() => fsm.TransitionTo<Served>());
//				sequence.AppendCallback(()=>GetComponent<Collider>().enabled = false);
//				sequence.OnComplete(() => DeclareInactiveTween());
////				GetComponent<Collider>().enabled = false;
//			}
//		}
		
		if (coaster != null)
		{
			Debug.Log("setting " + coaster.name + " as parent");
			transform.SetParent(coaster.gameObject.transform);
//				GetComponent<Collider>().enabled = false;	
		}
		
		
 	}

	public void UnServe(GameEvent e)
	{
		if (CurrentState == ServedState)
		{
			DrinkRejectedEvent drinkRejectedEvent = (DrinkRejectedEvent) e;	
			Sequence sequence = DOTween.Sequence();
			myServiceDropzone.MyCoaster().transform.DOLocalMove(unservedPos, 0.5f);
			sequence.AppendCallback(() => Services.TweenManager.tweensAreActive = true);
			sequence.Append(transform.DOLocalMove(unservedPos + dropOffset, 0.5f, false));
			sequence.AppendCallback(() => fsm.TransitionTo<ReadyToServe>());
			sequence.AppendCallback(()=>GetComponent<Collider>().enabled = true);
			myServiceDropzone.isOccupied = true;
			sequence.OnComplete(() => DeclareInactiveTween());
//			GetComponent<Collider>().enabled = true;
		}
	}
	
//	public void ReturnHome(GameEvent e){
//		DayEndEvent dayEndEvent = e as DayEndEvent;
	
	public override void ReturnHome(GameEvent e){
		DayEndEvent dayEndEvent = e as DayEndEvent;
		transform.position = origPos;
		pickedUp = false;
		transform.eulerAngles = Vector3.zero;
		StartCoroutine(ChangeToWorldLayer(1f));
		ClearIce();
		fsm.TransitionTo<NotReadyToServe>();
		hasIce = false;
		Liquid.isEvaluated = false;
		Liquid.EmptyLiquid();
//		glassServeState = Glass.GlassServeState.NotReadyToServe;
		Liquid.myDrinkBase = DrinkBase.none;
		Liquid.myMixer = Mixer.none;
		// if (GetComponent<Bottle>() == null)
		// {
		//     glass.liquid.transform.localScale = new Vector3(0, 0, 0);
		// }
	}

	/*
	 * STATES *
	 */
	private class GlassState : FSM<Glass>.State
	{
		
	}

	private class NotReadyToServe : GlassState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Context.CanBePouredInto = true;
			Context.NotReadyToServeState = this;
			Context.CurrentState = this;
			Context.glassServeState = GlassServeState.NotReadyToServe;
		}

		public override void Update()
		{
			base.Update();
			if (!Context.pickedUp && Context.isInServeZone)
			{
				TransitionTo<ReadyToServe>();
			}
		}
	}

	private class ReadyToServe : GlassState
	{
		public override void OnEnter()
		{
			Debug.Log("Ready to serve!");
			base.OnEnter();
			Context.CanBePouredInto = true;
			Context.ReadyToServeState = this;
			Context.CurrentState = this;
			Context.glassServeState = GlassServeState.ReadyToServe;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
	
	private class Served : GlassState
	{
		public override void OnEnter()
		{
			Debug.Log("Served!");
			base.OnEnter();			
			Context.Liquid.TalkToCoaster();
			Context.CanBePouredInto = false;
			Context.ServedState = this;
			Context.CurrentState = this;
			Context.glassServeState = GlassServeState.Served;
			EventManager.Instance.Register<DrinkRejectedEvent>(Context.UnServe);
			Context.myServiceDropzone.GetComponent<Collider>().enabled = false;
		}

		public override void OnExit()
		{
			base.OnExit();			
			EventManager.Instance.Unregister<DrinkRejectedEvent>(Context.UnServe);
			Context.Liquid.isEvaluated = false;
			Debug.Log("Exiting Served State!");
			Context.myServiceDropzone.GetComponent<Collider>().enabled = true;
		}
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.GetComponent<Coaster>() != null)
		{
			coaster = hit.gameObject.GetComponent<Coaster>();
		}
	}
}

public class DrinkRejectedEvent : GameEvent
{
	
}


