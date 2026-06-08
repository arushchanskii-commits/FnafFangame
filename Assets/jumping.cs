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
    private float jumpCooldownTimer;

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
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && jumpCooldownTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCooldownTimer = jumpCooldown;
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
