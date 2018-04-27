using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{

	[SerializeField] private Image _crosshair;

	private float noTargetAlpha;

	private float targetSightedAlpha;

	private PlayerInput _player;

	private bool _hasGrown = false;

	private bool _hasShrunken = false;
	// Use this for initialization
	void Start ()
	{
		_player = Services.GameManager.playerInput;
 	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeCrosshairAlphaOnTargetSighted()
	{
		_hasShrunken = false;

		if (!_hasGrown)
		{
			_crosshair.DOColor(new Color(255, 255, 255, 1), 0.1f);
			Sequence a = DOTween.Sequence();
			a.Append(_crosshair.transform.DOScaleX(0.1f, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(_crosshair.transform.DOScaleY(0.1f, 0.25f));
			_hasGrown = true;
		}
 	}
	
	public void ChangeCrosshairAlphaOnTargetLost()
	{
		_hasGrown = false;

		if (!_hasShrunken)
		{
			_crosshair.DOColor(new Color(255, 255, 255, 0.2f), 0.1f);
			Sequence a = DOTween.Sequence();
			a.Append(_crosshair.transform.DOScaleX(0.05f, 0.25f));
			
			Sequence b = DOTween.Sequence();
			b.Append(_crosshair.transform.DOScaleY(0.05f, 0.25f));
			_hasShrunken = true;
		}
	}
	
}
