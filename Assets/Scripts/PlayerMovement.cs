using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool isFacingRight = true;
    public Animator animator;
    public BoxCollider2D playerCollider;
    public PlayerShooting playerShooting;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Lock & Aim")]
    public bool isLocked = false;               // Lock mode state
    public Vector2 aimDirection = Vector2.right;  // Current aim direction (default to right)
    public Transform weaponTransform;           // Assign your weapon (or gun) transform
    public float aimRotationOffset = 0f;          // If you need an extra rotation offset

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;
    public LayerMask platformLayer;
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
        // Prevent movement while crouching or being locked
        if (!isCrouching && !isLocked)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // While crouching or locked, disable horizontal movement
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); //Stop horizontal movement
        }

        // If locked, update weapon rotation based on aimDirection
        if (isLocked)
        {
            if (aimDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                weaponTransform.rotation = Quaternion.Euler(0, 0, angle + aimRotationOffset);
            }
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
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && playerShooting != null)
        {
            if (!IsGrounded()) // Player is in mid-air (Jump Attack)
            {
                playerShooting.JumpAttack();
                animator.SetTrigger("jumpAttack"); // Play Jump Attack Animation
            }
            else // Normal Shooting on the Ground
            {
                playerShooting.Shoot();
            }
        }
    }
    // Movement input; only update if not crouching and not locked.
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
            jumpsRemaining = maxJumps;
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
        if (jumpsRemaining > 0 && !isCrouching && !isLocked) //Prevent jumping while crouching and locking
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
    // Lock input—when held, the player cannot move and can aim with his weapon.
    public void Lock(InputAction.CallbackContext context)
    {
        if (context.performed && !isCrouching && IsGrounded())
        {
            isLocked = true;
            // Optionally trigger a "locked" animation state.
            animator.SetBool("isLocked", true);
        }
        else if (context.canceled)
        {
            isLocked = false;
            animator.SetBool("isLocked", false);
        }
    }
    // Aim input—only used when locked. Updates aimDirection based on input.
    public void Aim(InputAction.CallbackContext context)
    {
        if (isLocked)
        {
            Vector2 input = context.ReadValue<Vector2>();

            float x = isFacingRight ? 1 : -1; // Default to facing direction
            float y = input.y; // Get vertical aim

            if (y > 0 && input.x == 0) // W Pressed (Straight Up)
            {
                aimDirection = Vector2.up;
            }
            else if (y < 0 && input.x == 0) // S Pressed (Straight Down)
            {
                aimDirection = Vector2.down;
            }
            else if (y > 0 && input.x != 0) // W + A/D (Diagonal Up)
            {
                aimDirection = new Vector2(x, 1).normalized;
            }
            else if (y < 0 && input.x != 0) // S + A/D (Diagonal Down)
            {
                aimDirection = new Vector2(x, -1).normalized;
            }
            else // No vertical input, aim horizontally
            {
                aimDirection = new Vector2(x, 0);
            }
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }
    private void GroundCheck()
    {
        bool onGround = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        bool onPlatform = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, platformLayer);

        if (onGround || onPlatform)
        {
            jumpsRemaining = maxJumps; // Reset jumps when touching ground or platform
            
        }

    }
    public void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            playerShooting.Flip(isFacingRight);

            if (!isLocked)
            {
                aimDirection = isFacingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                // If locked, keep the last vertical aim direction but switch horizontal
                aimDirection = new Vector2(isFacingRight ? Mathf.Abs(aimDirection.x) : -Mathf.Abs(aimDirection.x), aimDirection.y);
            }
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
