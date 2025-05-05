using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Vector3 startPosition; // Oyuncunun başlangıç pozisyonu

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Exiting the game...");
            Application.Quit(); // Oyundan çıkış
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restarting position...");
            if (playerTransform != null)
                playerTransform.position = startPosition; // Oyuncuyu başlangıç pozisyonuna taşı
        }
    }
}
