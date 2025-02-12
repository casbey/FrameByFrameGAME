using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float firstJumpAmount = 4f;
    float horizontalInput;
    bool isOnGround = false;
    private bool isFacingRight = true;

    Rigidbody2D rb;
    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, firstJumpAmount);
            isOnGround = false;
            anim.SetBool("isJumping", !isOnGround);
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        anim.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity",rb.linearVelocity.y);
    }

    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOnGround = true;
        anim.SetBool("isJumping", !isOnGround);
    }
}
