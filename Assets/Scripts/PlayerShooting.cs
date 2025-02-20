using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 10f;
    public GameObject jumpAttack; // Assign in Inspector
    public float jumpAttackDuration = 0.3f; // How long the attack lasts

    private bool isFacingRight = true;

    public GameObject waterEffect;  // Assign Water Effect GameObject in Unity
    private Animator waterAnimator; // Animator for the water effect
    private Animator jumpAttackAnimator;

    private PlayerMovement playerMovement;
    private CircleCollider2D hitboxCollider;

    public float offsetForCrouch = 0.5f;

    // FirePoint Offsets
    private Vector3 standingOffset = new Vector3(2.05f, 1.05f, 0f);
    private Vector3 upShootingOffset = new Vector3(-1.85f, 2.46f, 0f);
    private Vector3 diagonalShootingOffset = new Vector3(-0.1f, 1.5f, 0f);
    private Vector3 crouchingFirePointOffset = new Vector3(1.25f, -1.2f, 0f); // Adjust for crouching

    void Start()
    {
        if (waterEffect != null)
        {
            waterAnimator = waterEffect.GetComponent<Animator>();
            waterEffect.SetActive(false); // Keep hidden until shooting
        }

        playerMovement = GetComponent<PlayerMovement>();  // Get PlayerMovement reference
        jumpAttackAnimator = jumpAttack.GetComponent<Animator>();

        if (jumpAttack)
        {
            hitboxCollider = jumpAttack.GetComponent<CircleCollider2D>();
            jumpAttack.SetActive(false); // Ensure it's disabled at start
        }
    }
    void Update()
    {
        UpdateFirePointPosition();
    }

    void UpdateFirePointPosition()
    {
        if (firePoint == null || playerMovement == null) return;

        Vector3 offset = standingOffset; // Default standing position

        if (playerMovement.isCrouching)
        {
            offset += crouchingFirePointOffset; // Apply crouch offset
        }

        // Apply offsets based on aim direction
        if (playerMovement.aimDirection == Vector2.up)
        {
            offset += upShootingOffset; // Apply up-aiming offset
        }
        else if (playerMovement.aimDirection == new Vector2(1, 1).normalized ||
                 playerMovement.aimDirection == new Vector2(-1, 1).normalized)
        {
            offset += diagonalShootingOffset; // Apply diagonal-aiming offset
        }

        // Apply calculated offset to FirePoint
        firePoint.localPosition = offset;
    }

    public void Shoot()
    {
        if (!playerMovement) return;

        // Determine shooting direction
        Vector2 shootDirection = playerMovement.isLocked ? playerMovement.aimDirection :
                                  (playerMovement.isFacingRight ? Vector2.right : Vector2.left);

        // Only activate WaterEffect if NOT shooting up
        if (waterEffect != null)
        {
            if (playerMovement.aimDirection != Vector2.up && (playerMovement.horizontalMovement == 0 || playerMovement.isLocked)) // Check if not aiming up
            {
                waterEffect.transform.position = firePoint.position; // Match FirePoint position
                waterEffect.SetActive(true); // Show WaterEffect
                waterAnimator.SetTrigger("fire"); // Play animation
                Invoke("HideWaterEffect", 0.3f); // Hide after animation
            }
            else
            {
                waterEffect.SetActive(false); // Ensure it's off when shooting up
            }
        }

        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Apply velocity
        rb.linearVelocity = shootDirection * bulletSpeed;

        // Rotate bullet to match direction
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy the bullet after 3 seconds
        Destroy(bullet, 3f);
    }
    public void JumpAttack()
    {
        if (!playerMovement) return;

        Debug.Log("Jump Melee Attack Executed!");

        // Make sure JumpAttack is active before playing animation
        if (jumpAttack != null && hitboxCollider != null)
        {
            jumpAttack.SetActive(true);  // Activate JumpAttack GameObject
            hitboxCollider.enabled = true; // Enable Hitbox Collider
        }

        // Reset Animator State to force play
        jumpAttackAnimator.Rebind();
        jumpAttackAnimator.Update(0);

        // Play the jump attack animation
        jumpAttackAnimator.SetTrigger("jumpAttack");

        // Deal damage
        DealJumpAttackDamage();
    }
    void DealJumpAttackDamage()
    {
        // Define attack radius
        float attackRadius = hitboxCollider.radius; // Adjust based on your sprite size

        // Get all colliders in the attack radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(jumpAttack.transform.position, attackRadius);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Ensure we only hit enemies
            {
                Debug.Log("Hit enemy: " + enemy.name);

                // Apply damage (Assuming the enemy has a script with a "TakeDamage()" function)
                enemy.GetComponent<Enemy>().TakeDamage(3);
            }
        }
    }
    public void DisableJumpAttackHitbox()
    {
        if (jumpAttack && hitboxCollider != null)
        {
            jumpAttack.SetActive(false);
            hitboxCollider.enabled = false;
            Debug.Log("Jump Melee Attack Disabled!");
        }
    }
    private void HideWaterEffect()
    {
        if (waterEffect != null)
        {
            waterEffect.SetActive(false);
        }
    }
    public void Flip(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}
