using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerRespawnHandler : MonoBehaviour
{
    public Transform respawnPoint;
    public Image screenFader; // UI'deki siyah Image
    public float fadeDuration = 0.5f;

    private playerMovement playerMovement; // Oyuncunun hareket script'i
    private TimeTravel timeTravel;
    private void Start()
    {
        playerMovement = GetComponent<playerMovement>();
        timeTravel = GetComponent<TimeTravel>(); // TimeTravel referansını al
            // respawnPoint referansını playerMovement'tan al
        if (playerMovement != null && respawnPoint == null)
        {
            respawnPoint = playerMovement.respawnPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard") || collision.CompareTag("FallZone"))
        {
            StartCoroutine(RespawnWithFade());
        }
    }

    private IEnumerator RespawnWithFade()
    {
        // Hareketi kapat
        if (playerMovement != null)
            playerMovement.enabled = false;

        
        // Ekran tamamen karanlık olduğundan emin olmak için
        Color c = screenFader.color;
        c.a = 1f;
        screenFader.color = c;
        
        // Karanlıkta bir süre bekle
        yield return new WaitForSeconds(0.1f);

        // Karanlıktayken oyuncuyu ışınla
        transform.position = respawnPoint.position;

        // Zaman klonunu yok et (eğer TimeTravel scripti varsa)
        if (timeTravel != null)
        {
            timeTravel.OnPlayerRespawn();
        }

        // Oyuncu ışınlandıktan sonra kısa bir bekleme
        yield return new WaitForSeconds(0.2f);

        // Ekranı aydınlat
        yield return StartCoroutine(FadeScreen(1f, 0f));

        // Ekran tamamen aydınlandığından emin ol
        c = screenFader.color;
        c.a = 0f;
        screenFader.color = c;

        // Hareketi açmadan önce kısa bir bekleme (isteğe bağlı)
        yield return new WaitForSeconds(0.1f);

        // Hareketi aç
        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    private IEnumerator FadeScreen(float from, float to)
    {
        float elapsed = 0f;
        Color c = screenFader.color;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            c.a = Mathf.Lerp(from, to, t);
            screenFader.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = to;
        screenFader.color = c;
    }
}
