using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 10f;
    public GameObject jumpAttackHitbox; // Assign in Inspector
    public float jumpAttackDuration = 0.3f; // How long the attack lasts

    private bool isFacingRight = true;

    private PlayerMovement playerMovement;
    private CircleCollider2D hitboxCollider;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();  // Get PlayerMovement reference

        if (jumpAttackHitbox)
        {
            hitboxCollider = jumpAttackHitbox.GetComponent<CircleCollider2D>();
            jumpAttackHitbox.SetActive(false); // Ensure it's disabled at start
        }
    }

    public void Shoot()
    {
        if (!playerMovement) return;

        // Determine shooting direction
        Vector2 shootDirection = playerMovement.isLocked ? playerMovement.aimDirection :
                                  (playerMovement.isFacingRight ? Vector2.right : Vector2.left);

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

        // Enable the hitbox for a short duration
        jumpAttackHitbox.SetActive(true);
        hitboxCollider.enabled = true;

        // Disable after a short delay
        Invoke("DisableJumpAttackHitbox", jumpAttackDuration);
    }
    private void DisableJumpAttackHitbox()
    {
        if (jumpAttackHitbox)
        {
            jumpAttackHitbox.SetActive(false);
            hitboxCollider.enabled = false;
            Debug.Log("Jump Melee Attack Disabled!");
        }
    }

    public void Flip(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}
