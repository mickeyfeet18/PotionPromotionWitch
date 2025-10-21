using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    private Animator anim;
    private float originalScaleX;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Sla de originele schaal van de speler op
        originalScaleX = transform.localScale.x;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocityY);

        // Spiegelen afhankelijk van bewegingsrichting, maar behoud originele schaal
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);

        if (Input.GetKeyDown(KeyCode.Space))
            body.linearVelocity = new Vector2(body.linearVelocityX, speed);

        // Animator-parameter bijwerken
        anim.SetBool("run", horizontalInput != 0);
    }
}