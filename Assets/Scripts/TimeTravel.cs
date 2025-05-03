using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravel : MonoBehaviour
{
    [Header("Time Clone Settings")]
    public GameObject clonePrefab; // Klon prefabı
    public Transform respawnPoint; // Respawn noktası referansı
    
    private GameObject activeClone; // Aktif olan klon
    private playerMovement playerMovement; // Oyuncu hareketi referansı
    private Vector3 initialRespawnPosition; // Başlangıç respawn pozisyonu
    private void Start()
    {
        // Komponentleri al
        playerMovement = GetComponent<playerMovement>();
        
        // respawnPoint referansını playerMovement'tan al
        if (playerMovement != null)
        {
            respawnPoint = playerMovement.respawnPoint;
            initialRespawnPosition = respawnPoint.position;
        }
    }
    
    private void Update()
    {
        // E tuşuna basıldığında zaman klonu oluştur
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateTimeClone();
        }
    }
    
    // Zaman klonu oluştur ve respawn noktasını güncelle
    public void CreateTimeClone()
    {
        // Eğer aktif klon varsa yok et
        if (activeClone != null)
        {
            Destroy(activeClone);
        }
        
        // Oyuncunun bulunduğu pozisyonda klon oluştur
        activeClone = Instantiate(clonePrefab, transform.position, transform.rotation);
        
        // Klonun görünüşünü oyuncuya benzet
        SpriteRenderer cloneRenderer = activeClone.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        
        if (cloneRenderer != null && playerRenderer != null)
        {
            cloneRenderer.sprite = playerRenderer.sprite;
            cloneRenderer.flipX = playerRenderer.flipX;
            
            // Yarı saydam yap
            Color cloneColor = cloneRenderer.color;
            cloneColor.a = 0.7f; // Alpha (transparanlık) değerini ayarla
            cloneRenderer.color = cloneColor;
        }
        
        // Respawn noktasını güncelle
        if (respawnPoint != null)
        {
            respawnPoint.position = transform.position;
        }
        
        Debug.Log("Zaman klonu oluşturuldu!");
    }
    
    // Oyuncu respawn olduğunda klonu yok et (diğer scriptlerden çağrılacak)
    public void OnPlayerRespawn()
    {
        if (activeClone != null)
        {
            Destroy(activeClone);
            activeClone = null;
            // Klon yok edildiğinde başlangıç pozisyonuna dön
            if (respawnPoint != null)
            {
                respawnPoint.position = initialRespawnPosition;
            }
        }
    }
}
