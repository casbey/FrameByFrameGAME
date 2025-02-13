using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 10f;

    private bool isFacingRight = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Change to Input System if needed
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Create bullet at the firePoint position
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Determine direction based on player's facing direction
        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * bulletSpeed, 0);

        // Flip bullet without affecting size
        Vector3 bulletScale = bullet.transform.localScale;
        bulletScale.x = Mathf.Abs(bulletScale.x) * direction; // Keep original scale but flip direction
        bullet.transform.localScale = bulletScale;
    }

    public void Flip(bool facingRight)
    {
        isFacingRight = facingRight;
    }
}
