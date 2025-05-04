using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
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
        Debug.Log(transform.position);
        Debug.Log("Zaman klonu oluşturuldu!");
    }
    
    // Oyuncu respawn olduğunda klonu yok et (diğer scriptlerden çağrılacak)
    public void OnPlayerRespawn()
    {
        if (activeClone != null)
        {
            Destroy(activeClone);
            activeClone = null;

            respawnPoint.position = initialRespawnPosition;
        }
    }
}*/



public class TimeTravel : MonoBehaviour
{

    // TimeTravel sınıfı içinde yeni bir yapı tanımlayın
    [System.Serializable]
    public struct PlayerFrame
    {
        public Vector3 position;
        public bool facingRight; // Karakterin yönü

        public PlayerFrame(Vector3 pos, bool facing)
        {
            position = pos;
            facingRight = facing;
        }
    }

    public GameObject purpleFilterPanel; // Mor filtre paneli referansı



    [Header("Respawn Settings")]
    public Transform defaultRespawnPoint; // Inspector'dan ayarlanacak özel respawn noktası
    public bool useDefaultRespawnPoint = true; // Özel respawn noktasını kullanma seçeneği

    [Header("Time Clone Settings")]
    public GameObject clonePrefab; // Klon prefabı
    public Transform respawnPoint; // Respawn noktası referansı
    public float replaySpeed = 2.0f; // Klonun kayıtlı hareketleri ne kadar hızlı tekrarlayacağı

    [Header("Afterimage Settings")]
    public float afterimageInterval = 0.5f; // Görüntü izlerinin oluşma sıklığı (saniye)
    public float afterimageAlpha = 0.4f; // Görüntü izlerinin transparanlığı

    private GameObject activeClone; // Aktif olan klon
    private playerMovement playerMovement; // Oyuncu hareketi referansı
    private Vector3 initialRespawnPosition; // Başlangıç respawn pozisyonu

    // Önceki pozisyon listesi yerine şimdi frame listesi kullanacağız
    private List<PlayerFrame> recordedFrames = new List<PlayerFrame>();
    private List<GameObject> afterimages = new List<GameObject>(); // Oluşturulan görüntü izleri
    private bool isRecording = false;
    private float recordInterval = 0.05f; // Her 0.05 saniyede bir pozisyon kaydet
    private float lastRecordTime = 0f;
    private float lastAfterimageTime = 0f; // Son görüntü izinin oluşturulma zamanı
    private Vector3 finalPosition; // E'ye ikinci kez basıldığındaki konum

    private Vector3 recordedStartPosition;


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
        // E tuşuna basıldığında
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isRecording && activeClone == null)
            {
                // İlk kez E'ye basılıyor - Klonu oluştur ve kaydetmeye başla
                StartRecording();
            }
            else if (isRecording && activeClone != null)
            {
                // İkinci kez E'ye basılıyor - Kaydı durdur ve oynatmaya başla
                StopRecordingAndPlayback();
            }
        }
        // Q tuşuna basıldığında kaydı iptal et ve her şeyi temizle
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CancelRecording(); // Kaydı iptal et
        }
        // Kayıt modundayken belirli aralıklarla pozisyonu kaydet
        if (isRecording && Time.time - lastRecordTime > recordInterval)
        {
            // Oyuncunun yönünü belirle (SpriteRenderer.flipX kullanarak)
            SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
            bool isFacingRight = (playerRenderer != null) ? !playerRenderer.flipX : true;

            // Pozisyon ve yönü kaydet
            recordedFrames.Add(new PlayerFrame(transform.position, isFacingRight));
            lastRecordTime = Time.time;
        }

        // Kayıt modundayken belirli aralıklarla görüntü izi oluştur
        if (isRecording && Time.time - lastAfterimageTime > afterimageInterval)
        {
            CreateAfterimage();
            lastAfterimageTime = Time.time;
        }
    }

    // Kayda başla ve klonu oluştur
    private void StartRecording()
    {
        // Kaydı başlat
        recordedFrames.Clear();
        ClearAfterimages(); // Önceki görüntü izlerini temizle
        isRecording = true;
        lastRecordTime = Time.time;
        lastAfterimageTime = Time.time;
        recordedStartPosition = transform.position;


        if (purpleFilterPanel != null)
        {
            purpleFilterPanel.SetActive(true);
        }

        // Klonu oluştur
        activeClone = Instantiate(clonePrefab, transform.position, transform.rotation);

        // Klonun görünüşünü ayarla
        SpriteRenderer cloneRenderer = activeClone.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();

        if (cloneRenderer != null && playerRenderer != null)
        {
            cloneRenderer.sprite = playerRenderer.sprite;
            cloneRenderer.flipX = playerRenderer.flipX;

            // Yarı saydam yap
            Color cloneColor = cloneRenderer.color;
            cloneColor.a = 0.7f; // Alpha değerini ayarla
            cloneRenderer.color = cloneColor;
        }

        // Respawn noktasını güncelle
        if (respawnPoint != null)
        {
            respawnPoint.position = transform.position;
        }
        Time.timeScale = 0.5f;
        Debug.Log("Zaman klonu oluşturuldu ve kayıt başladı!");
    }
    private void CancelRecording()
    {
        if (isRecording)
        {
            // Kayıt sırasında her şeyi temizle
            isRecording = false;
            ClearAfterimages(); // Görüntü izlerini temizle

            // Zamanı normale döndür
            Time.timeScale = 1.0f;

            // Klonu yok et
            if (activeClone != null)
            {
                Destroy(activeClone);
                activeClone = null;
            }

            // Mor filtreyi kapat
            if (purpleFilterPanel != null)
            {
                purpleFilterPanel.SetActive(false);
            }

            // Oyuncunun hareketini tekrar etkinleştir
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

            Debug.Log("Kayıt iptal edildi, her şey sıfırlandı.");
        }
    }

    // Kayıt sırasında görüntü izi oluştur
    private void CreateAfterimage()
    {
        // Oyuncunun şu anki pozisyonunda bir görüntü izi oluştur
        GameObject afterimage = Instantiate(clonePrefab, transform.position, transform.rotation);
        afterimages.Add(afterimage);

        // Görüntü izinin görünüşünü ayarla
        SpriteRenderer afterimageRenderer = afterimage.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();

        if (afterimageRenderer != null && playerRenderer != null)
        {
            afterimageRenderer.sprite = playerRenderer.sprite;

            // Yön bilgisini kopyala
            afterimageRenderer.flipX = playerRenderer.flipX;

            // Daha saydam yap
            Color afterimageColor = afterimageRenderer.color;
            afterimageColor.a = afterimageAlpha;
            afterimageRenderer.color = afterimageColor;
        }

        // Collider'ları devre dışı bırak (varsa)
        Collider2D collider = afterimage.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    // Tüm görüntü izlerini temizle
    private void ClearAfterimages()
    {
        foreach (GameObject afterimage in afterimages)
        {
            if (afterimage != null)
            {
                Destroy(afterimage);
            }
        }
        afterimages.Clear();
    }

    // Kaydı durdur ve klona hareketleri oynat
    private void StopRecordingAndPlayback()
    {
        // Kaydı durdur
        isRecording = false;
        finalPosition = transform.position;
        transform.position = recordedStartPosition;


        // Zamanı tamamen durdur
        Time.timeScale = 0f;

        // Oyuncunun hareketini devre dışı bırak
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Klona kayıtlı hareketleri oynat
        StartCoroutine(PlaybackRecordedMovements());
    }

    // Kayıtlı hareketleri hızlı bir şekilde oynat ve görüntü izlerini sil
    private IEnumerator PlaybackRecordedMovements()
    {
        if (activeClone != null && recordedFrames.Count > 0)
        {
            // Hızlı oynatma için bekleme süresini hesapla
            float waitTime = recordInterval / replaySpeed;

            // Klonun SpriteRenderer bileşenini al
            SpriteRenderer cloneRenderer = activeClone.GetComponent<SpriteRenderer>();

            // Tüm kayıtlı pozisyonları ve yönleri sırayla uygula
            for (int i = 0; i < recordedFrames.Count; i++)
            {
                if (activeClone != null)
                {
                    // Pozisyonu güncelle
                    activeClone.transform.position = recordedFrames[i].position;

                    // Yönü güncelle (facingRight true ise flipX false olmalı)
                    if (cloneRenderer != null)
                    {
                        cloneRenderer.flipX = !recordedFrames[i].facingRight;
                    }

                    // Eğer klon bir görüntü iziyle yeterince yakınsa, o izi yok et
                    CheckAndDestroyAfterimages();

                    yield return new WaitForSecondsRealtime(waitTime);
                }
                else
                {
                    // Klon yok edilmiş
                    RestoreTimeAndPlayerControl();
                    ClearAfterimages();
                    yield break;
                }
            }

            // Son konuma git ve klonu yok et
            if (activeClone != null)
            {
                // Klonu başlangıç pozisyonuna ışınla (veya istersen hareket ettir)
                PlayerFrame firstFrame = recordedFrames[0];
                activeClone.transform.position = firstFrame.position;

                if (cloneRenderer != null)
                {
                    cloneRenderer.flipX = !firstFrame.facingRight;
                }


                // Son kalan görüntü izlerini temizle
                ClearAfterimages();

                yield return new WaitForSecondsRealtime(0.2f);

                Destroy(activeClone);
                activeClone = null;
                Debug.Log("Klon hareketleri tamamladı ve yok edildi!");

                // İşlem tamamlandı, zamanı ve oyuncu kontrolünü geri yükle
                RestoreTimeAndPlayerControl();
            }
        }
        else
        {
            RestoreTimeAndPlayerControl();
        }
    }

    // Zamanı ve oyuncu kontrolünü normale döndür
    private void RestoreTimeAndPlayerControl()
    {
        if (purpleFilterPanel != null)
        {
            purpleFilterPanel.SetActive(false);
        }
        // Zamanı normale döndür
        Time.timeScale = 1.0f;

        // Oyuncu hareketini tekrar etkinleştir
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    // Klon görüntü izlerine yakınsa onları yok et
    private void CheckAndDestroyAfterimages()
    {
        float destroyDistance = 0.5f; // Yok etme mesafesi
        List<GameObject> imagesToRemove = new List<GameObject>();

        foreach (GameObject afterimage in afterimages)
        {
            if (afterimage != null && activeClone != null)
            {
                // Mesafeyi kontrol et
                float distance = Vector3.Distance(afterimage.transform.position, activeClone.transform.position);
                if (distance < destroyDistance)
                {
                    // Görüntü izini yok et ve listeden çıkarmak için işaretle
                    Destroy(afterimage);
                    imagesToRemove.Add(afterimage);
                }
            }
        }

        // İşaretlenen görüntü izlerini listeden çıkar
        foreach (GameObject imageToRemove in imagesToRemove)
        {
            afterimages.Remove(imageToRemove);
        }
    }

    // Oyuncu respawn olduğunda klonu yok et
    public void OnPlayerRespawn()
    {
        try
        {
            // Kaydı durdur ve zamanı normale döndür
            if (isRecording)
            {
                isRecording = false;
                if (purpleFilterPanel != null)
                    purpleFilterPanel.SetActive(false);

                Time.timeScale = 1.0f;

                // Oyuncu hareketini tekrar etkinleştir (eğer devre dışı bırakılmışsa)
                if (playerMovement != null && !playerMovement.enabled)
                {
                    playerMovement.enabled = true;
                }

                Debug.Log("Kayıt sırasında ölüm: Kayıt durduruldu ve zaman normale döndü.");
            }

            // Klonu temizle
            if (activeClone != null)
            {
                Destroy(activeClone);
                activeClone = null;
                Debug.Log("Respawn: Klon temizlendi.");
            }

            // Kayıtları temizle
            recordedFrames.Clear();
            ClearAfterimages();

            // Respawn noktasını güncelle
            if (respawnPoint != null)
            {
                // Oyun mekanikleri için respawn noktasını güncelle
                if (useDefaultRespawnPoint && defaultRespawnPoint != null)
                {
                    respawnPoint.position = defaultRespawnPoint.position;
                    Debug.Log("Respawn: Özel respawn noktası kullanıldı.");
                }
                else
                {
                    respawnPoint.position = initialRespawnPosition;
                    Debug.Log("Respawn: Başlangıç respawn noktası kullanıldı.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("OnPlayerRespawn hata: " + e.Message);
            // Temel güvenliği sağla
            Time.timeScale = 1.0f;
        }
    }
    // Klon aktif mi kontrolü
    public bool IsCloneActive()
    {
        return activeClone != null;
    }

}