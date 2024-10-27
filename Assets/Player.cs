using System;
using UnityEngine;

public class Player : MonoBehaviour 
{

    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

    [Header("Attack Info")]
    [SerializeField] private float comboTimer = 0.3f;
    private float comboTimeWindow;
    private bool isAttacking;
    private int comboCounter;

    
    private float xInput;
    private int facingDir = 1;
    private bool facingRight = true;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movement();
        CheckInput();
        CollisionChecks();

        dashTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;

        FlipController();
        AnimatorControllers();
    }

    // Call function on attack animation event end
    public void FinishAttack() 
    {
        // Reset attack state
        isAttacking = false;

        // Increment combo counter (attack animation chain)
        comboCounter += 1;

        // Reset combo counter if greater than 2
        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

    // Check if the player is on ground
    private void CollisionChecks()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
    }

    // Manage inputs
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttackEvent();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }
    }

    // Start attack event
    private void StartAttackEvent()
    {
        if (!isGrounded)
        {
            return;
        }

        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }

        isAttacking = true;
        comboTimeWindow = comboTimer;
    }

    // Dash ability
    private void DashAbility()
    {
        if (dashCooldownTimer < 0 && !isAttacking) 
        {
            dashCooldownTimer = dashCooldown;
            dashTimer = dashDuration;
        }
    }

    // Move player
    private void Movement() 
    {
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (dashTimer > 0)
        {
            // Let us dash
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else
        {
            // Let us move normally
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    // Jump player
    private void Jump() 
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // Animator controllers
    private void AnimatorControllers() 
    {
        bool isMoving = rb.velocity.x != 0;

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTimer > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
    }

    // Flip player
    private void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    // Flip player based on direction of movement
    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        } 
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }
}
