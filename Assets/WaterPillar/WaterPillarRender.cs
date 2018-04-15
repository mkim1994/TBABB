using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class WaterPillarRender : MonoBehaviour {
	public Transform ReferenceSurface;
	private MeshRenderer meshRenderer;
	[SerializeField] private Material myMaterial;
	
	// Use this for initialization
	void Start () {
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		Material matCopy = new Material(myMaterial);
		meshRenderer.sharedMaterial = matCopy;
	}
	
	private void OnEnable() {
		meshRenderer = gameObject.GetComponent<MeshRenderer>();

	}
	private void OnDisable(){

	}
	// Update is called once per frame
	void Update () {
		meshRenderer.sharedMaterial.SetMatrix("_MatrixToSurface", ReferenceSurface.worldToLocalMatrix);
        meshRenderer.sharedMaterial.SetFloat("_WaterHeight", ReferenceSurface.position.y);
	}

}
