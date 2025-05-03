using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private float Move;

    public float speed;
    public float jump;

    bool grounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move = Input.GetAxisRaw("Horizontal");
        
        rb.velocity = new Vector2(Move * speed, rb.velocity.y);
        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jump*10));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Vector3 normal = other.GetContact(0).normal;

            // Eðer normal yukarýya yakýnsa (örn. en az %90 yukarý bakýyorsa)
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
}
