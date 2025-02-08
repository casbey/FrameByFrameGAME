using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed;
    public float firstJumpAmount;
    float inputMovement;

    Rigidbody2D rb;

    bool isOnGround;
    public Transform groundCheck;
    public LayerMask GroundMask;

    public float radius;

    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, radius, GroundMask);

        inputMovement = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(inputMovement * speed, rb.linearVelocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = Vector2.up * firstJumpAmount;
        }

        if (isOnGround && inputMovement != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }
}
