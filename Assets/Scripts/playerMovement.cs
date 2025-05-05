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

    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip footstepSound; // Yeni yürüme/koşma sesi değişkeni ekleyin
    private AudioSource audioSource;
    private void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = new GameObject("RespawnPoint").transform;
        respawnPoint.position = new Vector3(-5.57f, 1.35f, 0f);
        timeTravel = GetComponent<TimeTravel>();
        
        // Ses kaynağını al
        audioSource = GetComponent<AudioSource>();

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
                if (jumpSound != null && audioSource != null)
                    audioSource.PlayOneShot(jumpSound);
            }
            else if (!hasDoubleJumped && timeTravel != null && timeTravel.IsCloneActive())
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); // Eski zıplamayı sıfırla
                rb.AddForce(new Vector2(rb.velocity.x, jump * 10));
                anim.SetTrigger("doubleJump");
                hasDoubleJumped = true; // Double jump kullanıldı
                if (jumpSound != null && audioSource != null)
                    audioSource.PlayOneShot(jumpSound);
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

            // Yalnızca yukarı doğru olan yüzeyler için grounded = true
            if (normal.y > 0.7f && Mathf.Abs(normal.x) < 0.5f) // Yatay yüzeyleri hariç tut
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
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);

        if (timeTravel != null)
        {
            // Hazard'a çarptığımızda, respawn işlemi için TimeTravel scriptini kullan
            timeTravel.OnPlayerRespawn();
        }
        else
        {
            // TimeTravel yoksa normal respawn noktasını kullan
            transform.position = respawnPoint.position;
        }
        
        rb.velocity = Vector2.zero; // Hız sıfırlama
    }
}

public void PlayFootstepSound()
{
    // Oyuncu gerçekten hareket ediyorsa ve yerdeyse ses çal
    if (grounded && footstepSound != null && audioSource != null && Mathf.Abs(Move) > 0.1f)
    {
        audioSource.PlayOneShot(footstepSound, 6f);
    }
}

}
