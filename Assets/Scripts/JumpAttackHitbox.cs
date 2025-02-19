using UnityEngine;

public class JumpAttackHitbox : MonoBehaviour
{
    public int attackDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Ensure enemies have the "Enemy" tag
        {
            Enemy enemy = collision.GetComponent<Enemy>(); // Get Enemy script
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("Enemy Hit by Jump Attack!");
            }
        }
    }
}
