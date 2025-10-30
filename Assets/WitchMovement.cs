using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <-- Add this
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 6f;

    [Header("Inventory UI")]
    public Transform inventoryPanel;
    public GameObject itemSlotPrefab;
    [SerializeField] public int totalSlots = 12;

    [Header("Game Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float originalScaleX;

    private List<GameObject> slots = new List<GameObject>();
    private int collectedItems = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScaleX = transform.localScale.x;

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryPanel);
            Image img = newSlot.GetComponent<Image>();
            if (img != null)
                img.color = new Color(1, 1, 1, 0);
            slots.Add(newSlot);
        }
         
       
    }

    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScaleX), transform.localScale.y, transform.localScale.z);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        anim.SetBool("run", moveInput != 0);
        anim.SetBool("grounded", isGrounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Touched: " + collision.name + " | Tag: " + collision.tag);

        if (collision.CompareTag("Collectible"))
        {
            Sprite sprite = collision.GetComponent<SpriteRenderer>()?.sprite;
            if (sprite != null)
            {
                AddItemToInventory(sprite);
                Destroy(collision.gameObject);
                Debug.Log("Collected: " + collision.name);
                if (collectedItems == 5 )
                {
                    winPanel.SetActive(true);
                    Invoke(nameof(RestartGame), 2f);
                }
            }
        }

        if (collision.CompareTag("BadItem"))
        {
            TriggerGameOver();
            Destroy(collision.gameObject);
            Debug.Log("Touched bad item! Game Over triggered.");
        }

        if (collision.CompareTag("FinalItem"))
        {
            Triggerwinpanel();
            Destroy(collision.gameObject);
            Debug.Log("Touched bad item! Game Over triggered.");
        }
    }

    private void AddItemToInventory(Sprite itemSprite)
    {
        foreach (GameObject slot in slots)
        {
            Image img = slot.GetComponent<Image>();
            if (img != null && img.color.a == 0)
            {
                img.sprite = itemSprite;
                img.color = Color.white;
                collectedItems++;
                Debug.Log(collectedItems);
                break;
            }
        }

       
    }

    private void TriggerGameOver()
    {
        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        // Restart the game after 2 seconds
        Invoke(nameof(RestartGame), 2f);
    }

    private void Triggerwinpanel()
    {
        if (winPanel)
            winPanel.SetActive(true);

        // Restart the game after 2 seconds
        Invoke(nameof(RestartGame), 2f);
    }

    private void RestartGame()
    {
        // Reloads the active scene completely
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetInventory()
    {
        foreach (GameObject slot in slots)
        {
            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0);
            }
        }
        collectedItems = 0;
        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }
}