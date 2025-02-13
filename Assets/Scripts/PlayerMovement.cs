using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isFacingRight = true;
    public Animator animator;
    public BoxCollider2D playerCollider;
    public PlayerShooting playerShooting;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;
    bool isOnPlatform;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Crouching")]
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;
    bool isCrouching = false;

    // Update is called once per frame
    void Update()
    {
        //Prevent movement while crouching
        if (!isCrouching)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); //Stop horizontal movement
        }
        
        GroundCheck();
        Gravity();
        Flip();

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
    }
    private void Gravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //Fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {   
        if (!isCrouching)
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
        }
        else
        {
            horizontalMovement = 0;
        }
    }
    
    public void Descend(InputAction.CallbackContext context)
    {
        if (context.performed && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.50f));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 && !isCrouching) //Prevent jumping while crouching
        {
            if (context.performed)
            {
                //Holding down the jump button for full power
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("jump");
            }
            else if (context.canceled)
            {
                //Lightly tapping the jump button
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
                jumpsRemaining--;
                animator.SetTrigger("jump");
            }
        }
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            animator.SetBool("crouching", true);

            //Disable movement while crouching
            horizontalMovement = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            //Disable standing collider, enable crouching collider
            if (standingCollider && crouchingCollider)
            {
                standingCollider.enabled = false;
                crouchingCollider.enabled = true;
            }
        }
        else if (context.canceled)
        {
            isCrouching = false;
            animator.SetBool("crouching", false);

            //Re-enable standing collider
            if (standingCollider && crouchingCollider)
            {
                standingCollider.enabled = true;
                crouchingCollider.enabled = false;
            }
        }
    }
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
        }
    }
    public void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            playerShooting.Flip(isFacingRight);
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
