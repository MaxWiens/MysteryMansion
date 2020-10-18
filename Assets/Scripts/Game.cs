using UnityEngine;

public class Game : MonoBehaviour {
  public static Game Instance = null;

  public static InputActions Input = null;
  private void Awake() {
    if(Instance != null)
      Destroy(gameObject);
    Instance = this;
    DontDestroyOnLoad(this);

    Input = new InputActions();
    Input.Enable();
  }
}