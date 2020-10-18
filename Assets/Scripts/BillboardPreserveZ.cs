﻿using UnityEngine;

public class BillboardPreserveZ : Billboard {
	public override void LateUpdate(){
		base.LateUpdate();
		Vector3 t = transform.rotation.eulerAngles;
		t.z = 90;
		transform.rotation = Quaternion.Euler(t);
	} 
}
