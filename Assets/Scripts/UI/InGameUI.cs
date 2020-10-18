using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
  [SerializeField]
  private Ghost _ghost = null;
  [SerializeField]
  private RectTransform _energyBar = null;
  [SerializeField]
  private TMP_Text _energyText = null;
    [SerializeField]
    private RectTransform spookBar = null;

  private void Update() {
    _energyBar.localScale = new Vector3(_ghost.Energy / (float)Ghost.MaxEnergy, 1, 1);
        //_energyBar.position.Set();
        _energyText.text = $"Energy: {_ghost.Energy}";
        spookBar.localScale = new Vector3((Ghost.MaxSpookCooldown - _ghost.SpookCooldown) / Ghost.MaxSpookCooldown, 1, 1);
  }
}