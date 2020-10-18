using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Haunt : MonoBehaviour {
  public Transform HauntIndicatorLocation = null;
  public bool IsTriggered = false;
  public int EnergyCost = 0;

  public abstract IEnumerator HauntAction();
  
}