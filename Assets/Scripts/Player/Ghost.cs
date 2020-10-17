using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {
	public float HauntRadius = 1;

	private Haunt _hauntTarget;

	private void Start() {
		
	}

	private void Update() {   
		 Collider[] colliders = Physics.OverlapSphere(transform.position, HauntRadius, (1 << LayerMask.NameToLayer("Interactible")) | (1 << LayerMask.NameToLayer("Haunt")));
		 

		foreach(Collider c in colliders){
			Haunt h = c.GetComponent<Haunt>();
			if(h != null){
				_hauntTarget = h;
				ShowInteractable();
				break;
			}
		}
		 
	}
	
	private IEnumerator ShowInteractable(){
		// h.transform
		// yield break;

		yield return new WaitUntil(() =>
		{
				return Vector3.Distance(transform.position, _hauntTarget.transform.position) < HauntRadius;
		});
	}

	
}