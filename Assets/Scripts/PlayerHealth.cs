using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private CameraShake cameraShake;


    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        cameraShake = FindFirstObjectByType<CameraShake>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy)
        {
            animator.SetTrigger("damage");
            TakeDamage(enemy.damage);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //GAME OVER
        }

        // Trigger Camera Shake
        if (cameraShake != null)
        {
            cameraShake.ShakeCamera();
        }
        else
        {
            Debug.LogWarning("No CameraShake script found in the scene!");
        }
    }

}
