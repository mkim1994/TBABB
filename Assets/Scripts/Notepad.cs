using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : Pickupable {

    public Texture[] notes;
    public Texture notesigned;
    Material mat;
    bool onlyOnce;
	// Use this for initialization
	protected override void Start () {
        base.Start();
        onlyOnce = false;
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	public override void Update () {
        if(mat.GetTexture("_MainTex") != notes[Services.GameManager.dayManager.currentDay]){
            if (Services.GameManager.dayManager.currentDay != 0)
            {
                mat.SetTexture("_MainTex", notes[Services.GameManager.dayManager.currentDay]);
            }
        }
        if (Services.GameManager.dayManager.noteSigned && !onlyOnce)
        {
            if (mat.GetTexture("_MainTex") != notesigned){
                mat.SetTexture("_MainTex", notesigned);
                onlyOnce = true;
            }
        }
	}
}
