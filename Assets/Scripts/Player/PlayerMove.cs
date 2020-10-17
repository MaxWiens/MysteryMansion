using UnityEngine;

public class PlayerMove : MonoBehaviour {
  public float Speed = 4f;
  [SerializeField]
  private Rigidbody _rigidBody;
  [SerializeField]
  private Transform _modelTransform;

  private void Update() {
    Vector2 v = Game.Input.Player.Move.ReadValue<Vector2>();
    if(v.x != 0 || v.y != 0){
      Debug.Log(v);
      Vector3 camForward = Camera.main.transform.forward;
      camForward.y = 0;
      Vector3 camRight = new Vector3(-camForward.z, 0, camForward.x);

      Vector3 moveVec = (camForward * v.y + camRight * -v.x).normalized *4;

      Quaternion newDir = Quaternion.LookRotation(moveVec, -Vector3.Cross(camForward, camRight));
      //_modelTransform.rotation = Quaternion.Lerp(_modelTransform.rotation, newDir, Time.deltaTime * 20);
      
      if(_rigidBody.velocity.magnitude < Speed)
        _rigidBody.velocity = new Vector3(moveVec.x, _rigidBody.velocity.y < 0?_rigidBody.velocity.y:0, moveVec.z);
        //_rigidBody.AddForce(new Vector3(moveVec.x, _rigidBody.velocity.y < 0?_rigidBody.velocity.y:0, moveVec.z));
    }
  }
}