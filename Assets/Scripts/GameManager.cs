using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player References")]
    public playerMovement playerMovementScript;

    [Header("Audio")]
    public AudioSource introAudioSource;
    public AudioSource outroAudioSource;

    [Header("Timing Settings")]
    public float introTime = 90f; // 1.5 dakika (90 saniye)
    public float outroTime = 20f; // Bitiş süresi
    public float fadeInDuration = 5f; // Karanlıktan aydınlığa geçiş süresi

    [Header("UI")]
    public Image fadePanel; // Siyah bir panel (UI Image)

    private bool gameStarted = false;
    private bool gameFinished = false;

    void Start()
    {
        // Oyun başlangıcında player hareketini devre dışı bırak
        if (playerMovementScript == null)
        {
            playerMovementScript = FindObjectOfType<playerMovement>();
        }
        
        DisablePlayerMovement();
        
        // Fade panelini hazırla
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 1); // Siyah ve tamamen opak
            StartCoroutine(FadeIn());
        }
        
        // Intro sesini çal ve bekle
        if (introAudioSource != null)
        {
            introAudioSource.Play();
        }
        
        // 1.5 dakika sonra oyunu başlat
        Invoke("StartGame", introTime);
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeInDuration);
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
            yield return null;
        }
        
        // Fade tamamlandığında paneli tamamen şeffaf yap
        fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0);
    }

    void StartGame()
    {
        if (!gameFinished)
        {
            gameStarted = true;
            EnablePlayerMovement();
            Debug.Log("Oyun başladı! Oyuncu hareket edebilir.");
        }
    }

    void EnablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    void DisablePlayerMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    public void FinishGame()
    {
        if (!gameFinished)
        {
            gameFinished = true;
            DisablePlayerMovement();
            
            // Outro sesini çal
            if (outroAudioSource != null)
            {
                outroAudioSource.Play();
            }
            
            Debug.Log("Oyun bitti! Oyuncu artık hareket edemiyor.");
            EnablePlayerMovement();
            // İsterseniz 20 saniye sonra yeni bir sahneye geçiş yapabilirsiniz
            // Invoke("LoadNextScene", outroTime);
        }
    }
}