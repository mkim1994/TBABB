using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class WaterPillarRender : MonoBehaviour {
	public Transform ReferenceSurface;
	[SerializeField] private WaterSurface waterSurface;
	private MeshRenderer meshRenderer;
	[SerializeField] private Material myMaterial;
//	[SerializeField]private Liquid liquid;

	[SerializeField] private Vector4 myColor;
	private LiquidColors _colors;

	[SerializeField]private float _r;
	[SerializeField]private float _g;
	[SerializeField]private float _b;
	[SerializeField]private float _a;

 	
	// Use this for initialization
	void Start () {
		_colors = new LiquidColors();
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		Material matCopy = new Material(myMaterial);
		meshRenderer.sharedMaterial = matCopy;
		myColor = Color.white;
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

	public void SetMaterialColorOnPour(DrinkBase _drinkBase = DrinkBase.none, Mixer _mixer = Mixer.none)
	{
		if (_mixer == Mixer.none)
		{
			myColor = Util.CombineColors(LiquidColors.DrinkToColorDictionary[_drinkBase], myColor);
			meshRenderer.sharedMaterial.color = myColor;
//			waterSurface.meshRenderer.material.color = myColor;
			Debug.Log("changing color to drinkbase!");
			_r = myColor.x;
			_g = myColor.y;
			_b = myColor.z;
		}
		else if (_drinkBase == DrinkBase.none)
		{
			myColor = Util.CombineColors(LiquidColors.MixerToColorDictionary[_mixer], myColor);
			meshRenderer.sharedMaterial.color = myColor;
//			waterSurface.meshRenderer.material.color = myColor;
			Debug.Log("Changing color to mixer!");
			_r = myColor.x;
			_g = myColor.y;
			_b = myColor.z;
		}
	}

}
