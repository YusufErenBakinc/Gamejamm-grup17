using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignManager : MonoBehaviour
{
    [Header("Tabela Ayarları")]
    [SerializeField] private string signMessage = "Bu bir tabela mesajıdır!";
    
    [Header("UI Referansları")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    private bool isDisplaying = false;

    private void Start()
    {
        // Başlangıçta panel kapalı olsun
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpışan nesne oyuncu ise
        if (collision.CompareTag("Player"))
        {
            ShowMessage();
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Eğer çıkış yapan nesne oyuncu ise
        if (collision.CompareTag("Player"))
        {
            HideMessage();
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
