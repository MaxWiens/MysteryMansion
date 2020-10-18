using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Ghost : MonoBehaviour {
	public float HauntRadius = 1;

	public int Energy = 0;
	private float _energyTimer = 0f;
	private float spookCooldown;

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
	[SerializeField]
	private TriggerColliderScript spookCollider;

	private const float MaxSpookCooldown = 4;
	const int MaxEnergy = 10;

	private void Start() {
		_hauntIndicatorRenderer.enabled = false;
		spookCooldown = 0;
	}

	private void Update() {
		/*if(Energy < 10)
			_energyTimer += Time.deltaTime;
		if(_energyTimer >= 3f){
			_energyTimer -= 3f;
			Energy += 1;
			if(Energy >= 10){
				Energy = 10;
				_energyTimer = 0f;
			}
		}*/
		spookCooldown = Mathf.Clamp(spookCooldown - Time.deltaTime, 0, MaxSpookCooldown);
		if (spookCooldown == 0 && InputManager.Input.Player.Spook.triggered)
		{
			spookCooldown = MaxSpookCooldown;
			for (int i = 0; i < spookCollider.CollidersHit; i++)
			{
				Human human = spookCollider[i].transform.parent.GetComponent<Human>();
				if (human != null)
				{
					human.Spook();
					Energy += 1;
				}
			}

			if (Energy > MaxEnergy)
				Energy = MaxEnergy;
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