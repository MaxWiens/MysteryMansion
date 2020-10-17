using UnityEngine;
using System.Collections;

public class BookshelfHaunt : Haunt {
  private const float _TIME_TO_FALL = 0.5F;
  [SerializeField]
  private Collider _bookshelfCollider;
  public override IEnumerator HauntAction(){
    if(IsTriggered) yield break;
    IsTriggered = true;
    
    Quaternion q = transform.rotation;
    float time = 0f;
    // set collision to trigger
    _bookshelfCollider.isTrigger = true;

    while(time <= _TIME_TO_FALL+1){
      //if collides with a human kill them here
      time += Time.deltaTime;
      transform.rotation = Quaternion.Lerp(q, Quaternion.Euler(-90f,0f,0f), time/(_TIME_TO_FALL+1));
      yield return null;  
    }
    transform.rotation = Quaternion.Euler(-90f,0f,0f);

    // set collision back
    _bookshelfCollider.isTrigger = false;

    enabled = false;
  }
}