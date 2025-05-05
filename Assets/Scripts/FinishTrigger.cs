using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public GameObject targetObject; // Player objesi
    public GameObject finishSign;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Player ile çarpışma kontrolü
        {
            if (targetObject != null)
            {
                // Finish işaretini etkinleştir
                finishSign.SetActive(true);
                Debug.Log("Finish Triggered: Finish sign activated.");
                // GameControl scriptini etkinleştir
                GameControl gameControl = targetObject.GetComponent<GameControl>();
                if (gameControl != null)
                {
                    gameControl.enabled = true;
                    Debug.Log("Finish Triggered: GameControl script activated.");
                }
                else
                {
                    Debug.LogWarning("GameControl script not found on targetObject.");
                }
            }
        }
    }
}
