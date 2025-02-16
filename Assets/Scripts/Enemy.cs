using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    public int damage = 1;
    private int maxHealth = 3;
    private int health;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
