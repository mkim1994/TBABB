using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ice : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TweenToLeftGlass(float yIncrement){
		if(Services.GameManager.playerInput.pickupableInLeftHand != null){
			Glass thisGlass = Services.GameManager.playerInput.pickupableInLeftHand.GetComponent<Glass>();
			
			transform.SetParent(Services.GameManager.playerInput.pickupableInLeftHand.transform);
			transform.eulerAngles = new Vector3(Random.Range(1,360), Random.Range(1,360), Random.Range(1,360));
				// transform.localPosition = Vector3.zero;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOLocalRotate(transform.localEulerAngles, 0.75f));
			sequence.Append(transform.DOLocalMove(Vector3.zero + new Vector3 (Random.Range(-0.1f,0.1f), 0.25f+yIncrement, Random.Range(-0.1f,0.1f)), 0.5f, false).SetEase(Ease.InSine));
			thisGlass.myIceList.Add(this);
			thisGlass.hasIce = true;
 		}
	}

	public void TweenToRightGlass(float yIncrement){
		if(Services.GameManager.playerInput.pickupableInRightHand != null){
			Glass thisGlass = Services.GameManager.playerInput.pickupableInLeftHand.GetComponent<Glass>();
			transform.SetParent(Services.GameManager.playerInput.pickupableInRightHand.transform);
			transform.eulerAngles = new Vector3(Random.Range(1,360), Random.Range(1,360), Random.Range(1,360));
			Sequence sequence = DOTween.Sequence();
			sequence.Append(transform.DOLocalRotate(transform.localEulerAngles, 0.75f));
			sequence.Append(transform.DOLocalMove(Vector3.zero + new Vector3 (Random.Range(-0.1f,0.1f), 0.25f+yIncrement, Random.Range(-0.1f,0.1f)), 0.5f, false).SetEase(Ease.InSine));
			thisGlass.myIceList.Add(this);
		}
	}

}
