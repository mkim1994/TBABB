using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using BehaviorTree;
using DG.Tweening;
using Yarn;

public class NonHand : MonoBehaviour {

	private Tree<NonHand> _tree;
	private Player _rewiredPlayer;
	[SerializeField]private LayerMask _layerMask;
	private Camera _myCam;
	private Glass _glass;
	private NPC _npc;
	private Sink _sink;
	private Backdoor _backdoor;
	private IceMaker _iceMaker;
	private LightSwitch _lightSwitch;

	private float _maxInteractionDist;
	private float _maxTalkDist;

	private GameObject SeenInteractable;
	// Use this for initialization
	void Start ()
	{

		_maxInteractionDist = Services.GameManager.playerInput.maxInteractionDist;
		_maxTalkDist = Services.GameManager.playerInput.maxTalkingDist;
		
		_rewiredPlayer = Services.GameManager.playerInput.rewiredPlayer;
		_myCam = Camera.main;
		_tree = new Tree<NonHand>(new Selector<NonHand>(
			new Sequence<NonHand>(
				new IsPlayerLookingAtNpc(),
				new IsPlayerPressingButton(),
				new Talk()
			),
			
			new Sequence<NonHand>(
				new IsPlayerLookingAtLightSwitch(),
				new IsPlayerPressingButton(),
				new EndDay()
			),
			
			new Sequence<NonHand>(
				new IsPlayerLookingAtGlass(),
				//new IsGlassReadyToServe()
				new Serve()
			)
		));
	}
	
	// Update is called once per frame
	void Update () {
		InteractionRay();
		_tree.Update(this);
	}
	
	//WHAT IS PLAYER LOOKING AT LOGIC
	private void InteractionRay(){
		Ray ray = new Ray(_myCam.transform.position, _myCam.transform.forward);
		float rayDist = Mathf.Infinity;
		RaycastHit hit = new RaycastHit();
		
		if(Physics.Raycast(ray, out hit, rayDist, _layerMask)){
			GameObject hitObj = hit.transform.gameObject; //if you're actually looking at something
			if(hitObj.GetComponent<NPC>() != null && Vector3.Distance(transform.position, hitObj.transform.position) <= Services.GameManager.playerInput.maxTalkingDist){ //check if object looked at can be picked up
				_npc = hitObj.GetComponent<NPC>(); //if it's NPC and close enough, assign it to NPC.				  
			} else if (hitObj.GetComponent<NPC>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > Services.GameManager.playerInput.maxTalkingDist){
				_npc = null;
			}
			
			if (hitObj.GetComponent<Sink>() != null &&
			    Vector3.Distance(transform.position, hitObj.transform.position) <= Services.GameManager.playerInput.maxInteractionDist)
			{
				_sink = hitObj.GetComponent<Sink>();
			} else if (hitObj.GetComponent<Sink>() == null || Vector3.Distance(transform.position, hitObj.transform.position) > Services.GameManager.playerInput.maxInteractionDist)
			{
				_sink = null;
			}

			//no distance check
			if (hitObj.GetComponent<Backdoor>() != null &&
			    Vector3.Distance(transform.position, hitObj.transform.position) <= _maxInteractionDist)
			{
				_backdoor = hitObj.GetComponent<Backdoor>();
			} else if (hitObj.GetComponent<Backdoor>() == null || 
			           Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist)
			{
				_backdoor = null;
			}
			if (hitObj.GetComponent<IceMaker>() != null && 
			    Vector3.Distance(transform.position, hit.point) <= _maxInteractionDist)
			{
				_iceMaker = hitObj.GetComponent<IceMaker>();
			} else if (hitObj.GetComponent<IceMaker>() == null || 
			           Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist)
			{
				_iceMaker = null;
			}
			if (hitObj.GetComponent<LightSwitch>() != null &&
			    Vector3.Distance(transform.position, hitObj.transform.position) <= _maxInteractionDist)
			{
				_lightSwitch = hitObj.GetComponent<LightSwitch>();
			} else if (hitObj.GetComponent<LightSwitch>() == null || 
			           Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist)
			{
				_lightSwitch = null;
			}
			
			if(hitObj.GetComponent<Glass>() != null &&
			   Vector3.Distance(transform.position, hitObj.transform.position) <= _maxInteractionDist)
			{
				_glass = hitObj.GetComponent<Glass>();
			} else if (hitObj.GetComponent<Glass>() == null || 
			           Vector3.Distance(transform.position, hitObj.transform.position) > _maxInteractionDist)
			{
				_glass = null;
			}
		}
		else
		{
			_npc = null;
			_lightSwitch = null;
			_sink = null;
			_backdoor = null;
			_iceMaker = null;
			_glass = null;
		}
	}
	//TREE LOGIC
	
	////Conditions
	private class IsPlayerPressingButton : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			return context._rewiredPlayer.GetButtonDown("Talk");
		}
	}

	private class IsPlayerLookingAtNpc : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			return context._npc != null;
		}
	}

	private class IsPlayerLookingAtLightSwitch : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			return context._lightSwitch != null;
		}
	}

	private class IsPlayerLookingAtGlass : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			return context._glass != null;
		}
	}

	////Actions

	private class Talk : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			context._npc.InitiateDialogue();
			return true;
		}
	}

	private class EndDay : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			context._lightSwitch.EndDay();
//			context._backdoor.
			return true;
		}
	}

	private class Serve : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			if (context._rewiredPlayer.GetButtonDown("Talk"))
				Debug.Log("SERVING DRINK!");
				context._glass.Liquid.TalkToCoaster();	
			return true;
		}
	}

	private class PerformAction : Node<NonHand>
	{
		public override bool Update(NonHand context)
		{
			
			return true;
		}
	}
}
