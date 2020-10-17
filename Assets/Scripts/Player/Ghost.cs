using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {
  [SerializeField]
  private Collider _triggerCollider;
  //private 

  private void Start() {
    
  }
  private void Update() {
     
     //Collider[] colliders = Physics.OverlapSphere(transform.position, FindInteractableDistance, (1 << LayerMask.NameToLayer("Interactible")) | (1 << LayerMask.NameToLayer("Haunt")));
     //foreach()
  }
  
  private IEnumerator ShowInteractable(){
    yield break;
  }

  
}