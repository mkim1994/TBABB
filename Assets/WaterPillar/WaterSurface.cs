using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterSurface : MonoBehaviour {
 	// Use this for initialization

	public MeshRenderer meshRenderer;
	public Material myMaterial;
	private Color myColor;
	
	void Start()
	{
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		Material matCopy = new Material(myMaterial);
		meshRenderer.sharedMaterial = matCopy;
		myColor = Color.white;
	}

	void Update () {
		transform.rotation = Quaternion.identity;
	}
}
