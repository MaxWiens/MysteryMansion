using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Ghost : MonoBehaviour {
	public float HauntRadius = 1;

	public int Energy = 0;
	private float _energyTimer = 0f;

	private Haunt _hauntTarget = null;
	
	[SerializeField]
	private SpriteRenderer _hauntIndicatorRenderer = null;
	[SerializeField]
	private Transform _hauntIndicatorTransform = null;
	[SerializeField]
	private Sprite activeSprite = null;
	[SerializeField]
	private Sprite notEnoughEnergySprite = null;
	[SerializeField]
	private TMP_Text costText;

	private void Start() {
		_hauntIndicatorRenderer.enabled = false;
	}

	private void Update() {
		if(Energy < 10)
			_energyTimer += Time.deltaTime;
		if(_energyTimer >= 3f){
			_energyTimer -= 3f;
			Energy += 1;
			if(Energy >= 10){
				Energy = 10;
				_energyTimer = 0f;
			}
		}

		Collider[] colliders = Physics.OverlapSphere(transform.position, HauntRadius, (1 << LayerMask.NameToLayer("Interactible")) | (1 << LayerMask.NameToLayer("Haunt")));

		if(_hauntTarget != null){
			if(_hauntTarget.IsTriggered)
				_hauntTarget = null;
			else if(InputManager.Input.Player.Interact.triggered && Energy >= _hauntTarget.EnergyCost)
			{
				Energy -= _hauntTarget.EnergyCost;
				StartCoroutine(_hauntTarget.HauntAction());
			}
		}

		Haunt h = null;
		foreach(Collider c in colliders){
			if((h = c.GetComponent<Haunt>()) != null && !h.IsTriggered) break;
			else h = null;
		}
		if(h != null){
			_hauntTarget = h;
			_hauntIndicatorRenderer.enabled = true;
			if (Energy >= _hauntTarget.EnergyCost)
				_hauntIndicatorRenderer.sprite = activeSprite;
			else
				_hauntIndicatorRenderer.sprite = notEnoughEnergySprite;
			costText.alpha = 1;
			costText.text = $"(-{_hauntTarget.EnergyCost})";
			_hauntIndicatorTransform.position = _hauntTarget.HauntIndicatorLocation.position;
		}else{
			_hauntIndicatorRenderer.enabled = false;
			costText.alpha = 0;
			_hauntTarget = null;
		}
	}
}