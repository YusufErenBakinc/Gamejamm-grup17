using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerRespawnHandler : MonoBehaviour
{
    public Transform respawnPoint;
    public Image screenFader; // UI'deki siyah Image
    public float fadeDuration = 0.5f;

    private playerMovement playerMovement; // Oyuncunun hareket script'i

    private void Start()
    {
        playerMovement = GetComponent<playerMovement>();
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

        // Karart
        yield return StartCoroutine(FadeScreen(0f, 0f));
        yield return new WaitForSeconds(0.2f);

        // Respawn
        transform.position = respawnPoint.position;

        // Aydınlat
        yield return StartCoroutine(FadeScreen(1f, 0f));

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
