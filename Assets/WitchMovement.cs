using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck;        // Leeg object onderaan speler
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;        // Layer voor grond

    private Rigidbody2D body;
    private Animator anim;
    private float originalScaleX;
    private bool isGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;
    }

    private void Update()
    {
        // --- BEWEGING ---
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocityY);

        // Spiegelen afhankelijk van bewegingsrichting, behoud originele schaal
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);

        // --- GRONDCHECK ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- SPRINGEN ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            body.linearVelocity = new Vector2(body.linearVelocityX, jumpForce);
        }

        // --- ANIMATIE ---
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded);
    }

    // Laat de ground check zien in de Scene view (handig voor debuggen)
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
