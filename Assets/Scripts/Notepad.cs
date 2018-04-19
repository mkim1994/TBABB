using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour {

    public Texture[] notes;
    public Texture notesigned;
    bool onlyOnce;
	// Use this for initialization
	void Start () {
        onlyOnce = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(GetComponent<Material>().GetTexture(0) != notes[Services.GameManager.dayManager.currentDay]){
            if (Services.GameManager.dayManager.currentDay != 0)
            {
                GetComponent<Material>().SetTexture(0, notes[Services.GameManager.dayManager.currentDay]);
            }
        }
        if (Services.GameManager.dayManager.noteSigned && !onlyOnce)
        {
            if (GetComponent<Material>().GetTexture(0) != notesigned){
                GetComponent<Material>().SetTexture(0, notesigned);
                onlyOnce = true;
            }
        }
	}
}
