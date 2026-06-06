using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    InputAction jump;

    [SerializeField]
    float jumpForce = 5f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        jump.Enable();
    }

    private void FixedUpdate()
    {
        if (jump.IsPressed())
        {
            Debug.Log("Jump pressed");
            if (transform.position.y < -3.5f)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}