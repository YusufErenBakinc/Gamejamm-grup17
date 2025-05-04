using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private float Move;

    public float speed;
    public float jump;
    private bool hasDoubleJumped = false;
    private TimeTravel timeTravel; // TimeTravel referansı


    bool grounded;

    private Animator anim;

    private bool isFacingRight;

    public Transform respawnPoint; // Respawn için hedef nokta
    private void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = new GameObject("RespawnPoint").transform;
        respawnPoint.position = new Vector3(-5.57f, 1.35f, 0f);

        timeTravel = GetComponent<TimeTravel>();

    }

    private void Update()
    {
        Move = Input.GetAxisRaw("Horizontal");
        
        rb.velocity = new Vector2(Move * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb.AddForce(new Vector2(rb.velocity.x, jump * 10));
                hasDoubleJumped = false; // Yere basınca sıfırla
            }
            else if (!hasDoubleJumped && timeTravel != null && timeTravel.IsCloneActive())
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); // Eski zıplamayı sıfırla
                rb.AddForce(new Vector2(rb.velocity.x, jump * 10));
                anim.SetTrigger("doubleJump");
                hasDoubleJumped = true; // Double jump kullanıldı
            }
        }


        if (Move !=0) 
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning",false);
        }

        anim.SetBool("isJumping", !grounded);

        if (!isFacingRight && Move > 0)
        {
            Flip();

        }
        else if (isFacingRight && Move < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Vector3 normal = other.GetContact(0).normal;

            // E�er normal yukar�ya yak�nsa (�rn. en az %90 yukar� bak�yorsa)
            if (Vector3.Angle(normal, Vector3.up) < 45f)
            {
                grounded = true;
            }
        }

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            transform.position = respawnPoint.position;
            rb.velocity = Vector2.zero; // Hız sıfırlama (opsiyonel)
        }
    }
}
