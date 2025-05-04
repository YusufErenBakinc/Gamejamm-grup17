using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    [Header("Testere Hareket Ayarları")]
    [SerializeField] private float minX; // Minimum X koordinatı (sol sınır)
    [SerializeField] private float maxX; // Maksimum X koordinatı (sağ sınır)
    [SerializeField] private float moveSpeed = 2f; // Hareket hızı
    
    private bool moveRight = true; // Hareket yönü kontrolü
    
    void Update()
    {
        
        // Yöne göre hareket ettir
        if (moveRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        
        // Sınırlara gelince yön değiştir
        if (transform.position.x >= maxX)
        {
            moveRight = false;
        }
        else if (transform.position.x <= minX)
        {
            moveRight = true;
        }
    }
}