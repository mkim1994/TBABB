using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SpriteActor : MonoBehaviour {


  //  public DialogueRunner dr;

    // Use this for initialization
    void Start () {

    }
    
    // Update is called once per frame
    void Update () {
        RotateSpriteTowardPlayer();
    }

    void RotateSpriteTowardPlayer(){
        transform.LookAt(Services.GameManager.player.transform);
        transform.eulerAngles += new Vector3(0, 180f, 0);
    }

}
