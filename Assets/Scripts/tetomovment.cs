using UnityEngine;

public class tetomovment : MonoBehaviour
{
    public float jumpingpower = 16f;

    [SerializeField] private Transform groundcheck;
    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingpower);
            Debug.Log("spacegedrückt");
        }
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.2f, groundlayer);
    }
}
