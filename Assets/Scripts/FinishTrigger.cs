using UnityEngine;
using System.Collections;

public class FinishTrigger : MonoBehaviour
{
    public GameObject targetObject; // Player objesi
    public GameObject finishSign;
    public GameObject lastDialog; // Son diyalog objesi
    public GameObject lastDialogSubtitle; // Son diyalog altyazısı objesi
    public float movementDisableTime = 20f; // Hareketin devre dışı kalacağı süre (saniye)
    
    private bool hasTriggered = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // Player ile çarpışma kontrolü
        {
            hasTriggered = true; // Birden fazla tetiklemeyi önle
            
            // Finish işaretini etkinleştir
            if (finishSign != null)
            {
                finishSign.SetActive(true);
                Debug.Log("Finish Triggered: Finish sign activated.");
            }
            
            // Son diyaloğu aktifleştir
            if (lastDialog != null)
            {
                lastDialog.SetActive(true);
                Debug.Log("Finish Triggered: Last dialog activated.");
                
                // Ses kaynağını çal (lastDialog içinde AudioSource varsa)
                AudioSource dialogAudio = lastDialog.GetComponent<AudioSource>();
                if (dialogAudio != null)
                {
                    dialogAudio.Play();
                    Debug.Log("Finish Triggered: Last dialog audio playing.");
                    
                    lastDialogSubtitle.SetActive(true); // Altyazıyı etkinleştir
                }
            }
            
            // Oyuncu hareketini devre dışı bırak
            if (targetObject != null)
            {
                playerMovement movement = targetObject.GetComponent<playerMovement>();
                if (movement != null)
                {
                    movement.enabled = false;
                    Debug.Log("Finish Triggered: Player movement disabled.");
                    
                    // Belirtilen süre sonra hareketi tekrar aktif et
                    StartCoroutine(ReenableMovementAfterDelay(movement, movementDisableTime));
                    GameControl gameControl = targetObject.GetComponent<GameControl>();
                    if (gameControl != null)
                    {
                        gameControl.enabled = true;
                    }
                }
            }
        }
    }
    
    private IEnumerator ReenableMovementAfterDelay(playerMovement movement, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (movement != null)
        {
            movement.enabled = true;
            Debug.Log("Player movement re-enabled after " + delay + " seconds.");
        }
    }
}
