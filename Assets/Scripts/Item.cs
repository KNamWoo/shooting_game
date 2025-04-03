using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;
    Rigidbody2D rbody;

    private void Awake() {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        rbody.linearVelocity=Vector2.down*3f;
    }
}
