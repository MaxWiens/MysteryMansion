using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {
	public float HauntRadius = 1;

	private Haunt _hauntTarget;
	
	[SerializeField]
	private SpriteRenderer _hauntIndicatorRenderer;
	[SerializeField]
	private Transform _hauntIndicatorTransform;

	private void Start() {
		_hauntIndicatorRenderer.enabled = false;
	}

	private void Update() {   
		Collider[] colliders = Physics.OverlapSphere(transform.position, HauntRadius, (1 << LayerMask.NameToLayer("Interactible")) | (1 << LayerMask.NameToLayer("Haunt")));
		

		if(_hauntTarget != null){
			if(_hauntTarget.IsTriggered)
				_hauntTarget = null;
			else if(Game.Input.Player.Interact.triggered){
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