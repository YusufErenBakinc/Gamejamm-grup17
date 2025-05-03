using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignManager : MonoBehaviour
{
    [Header("Tabela Ayarları")]
    [SerializeField] private string signMessage = "Bu bir tabela mesajıdır!";
    [SerializeField] private float displayDuration = 5f;
    
    [Header("UI Referansları")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    private bool isDisplaying = false;
    private float displayTimer = 0f;

    private void Start()
    {
        // Başlangıçta panel kapalı olsun
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void Update()
    {
        // Eğer mesaj gösteriliyorsa süresini kontrol et
        if (isDisplaying)
        {
            displayTimer -= Time.deltaTime;
            
            if (displayTimer <= 0)
            {
                HideMessage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpışan nesne oyuncu ise ve henüz mesaj gösterilmiyorsa
        if (collision.CompareTag("Player") && !isDisplaying)
        {
            ShowMessage();
        }
    }
    
    private void ShowMessage()
    {
        if (messagePanel != null && messageText != null)
        {
            // Mesaj panelini etkinleştir
            messagePanel.SetActive(true);
            
            // Mesaj içeriğini ayarla
            messageText.text = signMessage;
            
            // Zamanlayıcıyı başlat
            displayTimer = displayDuration;
            isDisplaying = true;
        }
        else
        {
            Debug.LogError("SignManager: Message panel veya text referansları eksik!");
        }
    }
    
    private void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
            isDisplaying = false;
        }
    }
}
