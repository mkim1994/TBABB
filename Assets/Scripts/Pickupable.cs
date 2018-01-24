using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Pickupable : MonoBehaviour {
    public void TweenToPlayer(){
		transform.SetParent(Services.GameManager.player.transform);
		transform.DOLocalMove(Vector3.forward, 0.25f, false); 
	}

    public void DropThis(){

    }
	
}
