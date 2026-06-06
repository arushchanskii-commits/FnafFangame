using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class jumping : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float jumpCooldown = 0.5f;
    public LayerMask groundLayer;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private float nextJumpTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (groundCheckPoint == null)
        {
            groundCheckPoint = transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && Time.time >= nextJumpTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer) != null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
