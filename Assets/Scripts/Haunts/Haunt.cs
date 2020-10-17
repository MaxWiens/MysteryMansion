using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Haunt : MonoBehaviour {
  public Transform HauntIndicatorLocation;
  public bool IsTriggered = false;
  
  public abstract IEnumerator HauntAction();
  
}