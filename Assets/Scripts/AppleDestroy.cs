using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleDestroy : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isDestroyed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDestroyed && collision.CompareTag("Player"))
        {
            isDestroyed = true;
            animator.SetTrigger("Destroy");

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    public void DestroyApple()
    {
        Destroy(gameObject);
    }
}
