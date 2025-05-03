using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard") || collision.CompareTag("FallZone"))
        {
            transform.position = respawnPoint.position;
        }
    }
}

