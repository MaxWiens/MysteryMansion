using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {
	public float HauntRadius = 1;

	public int Energy = 0;
	private float _energyTimer = 0f;

	private Haunt _hauntTarget = null;
	
	[SerializeField]
	private SpriteRenderer _hauntIndicatorRenderer = null;
	[SerializeField]
	private Transform _hauntIndicatorTransform = null;

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
			else if(Game.Input.Player.Interact.triggered){
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
			_hauntIndicatorTransform.position = _hauntTarget.HauntIndicatorLocation.position;
		}else{
			_hauntIndicatorRenderer.enabled = false;
			_hauntTarget = null;
		}
	}
}