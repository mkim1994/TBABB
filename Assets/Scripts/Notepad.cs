using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour {

    public Texture[] notes;
    public Texture notesigned;
    Material mat;
    bool onlyOnce;
	// Use this for initialization
	void Start () {
        onlyOnce = false;
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
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
