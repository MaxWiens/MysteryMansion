using UnityEngine;
using UnityEngine.UI;
public class InGameUI : MonoBehaviour {
  [SerializeField]
  private Ghost _ghost = null;
  [SerializeField]
  private RectTransform _energyBar = null;
  [SerializeField]
  private Text _energyText = null;

  private void Update() {
    _energyBar.localScale = new Vector3(_ghost.Energy/10f, 1, 1);
    //_energyBar.position.Set();
    _energyText.text = _ghost.Energy.ToString();
  }
}