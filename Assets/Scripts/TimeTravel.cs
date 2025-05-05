using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    [Header("Audio Settings")]
    [SerializeField] private AudioClip recordStartSound;
    [SerializeField] private AudioClip recordStopSound;
    private AudioSource audioSource;

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

        // AudioSource component'ini al veya oluştur
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        if (isRecording && activeClone != null)
        {
            // Yeni özellik: Klonu oyuncuya getir
            StartCloneReturnSequence();
        }
        else
        {
            // Normal kayıt iptali
            CancelRecording();
        }
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

        // Kayıt başlatma sesini çal
        if (audioSource != null && recordStartSound != null)
        {
            audioSource.PlayOneShot(recordStartSound);
        }

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

        // Kayıt durdurma sesini çal - timeScale 0 olacağından ignoreListenerPause kullan
        if (audioSource != null && recordStopSound != null)
        {
            audioSource.ignoreListenerPause = true;
            audioSource.PlayOneShot(recordStopSound);
        }
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
        // Audio ayarını geri al
        if (audioSource != null)
        {
            audioSource.ignoreListenerPause = false;
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
        // Önce kayıt durumuna göre respawn pozisyonunu belirle
        if (isRecording && recordedFrames.Count > 0)
        {
            // Kayıttaysak ve kaydedilen frame'ler varsa, ilk klonun pozisyonuna git (kaydın başladığı konum)
            transform.position = recordedFrames[0].position;
            Debug.Log("Respawn: Zaman kaydındaki başlangıç pozisyonuna dönüldü.");
        }
        else
        {
            // Kayıtta değilsek, respawnPoint'e git
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
                Debug.Log("Respawn: Respawn noktasına dönüldü.");
            }
            else if (defaultRespawnPoint != null && useDefaultRespawnPoint)
            {
                transform.position = defaultRespawnPoint.position;
                Debug.Log("Respawn: Varsayılan respawn noktasına dönüldü.");
            }
            else
            {
                Debug.LogError("Respawn noktası ayarlanmamış!");
            }
        }
        
        // Rigidbody varsa hızı sıfırla
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        // Eğer kayıt modu aktifse, kaydı iptal et ve tüm hologramları temizle
        if (isRecording)
        {
            CancelRecording(); // Bu metod zaten hologramları temizliyor ve kaydı iptal ediyor
            Debug.Log("Ölüm nedeniyle kayıt iptal edildi ve hologramlar temizlendi.");
        }

        // Diğer respawn işlemleri devam edebilir
        if (activeClone != null && !isRecording)
        {
            // Klon aktiflse ama kayıt yoksa (ölmeden önce oynatma modundayken)
            Destroy(activeClone);
            activeClone = null;
            RestoreTimeAndPlayerControl();
            Debug.Log("Ölüm nedeniyle klon temizlendi.");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("OnPlayerRespawn hata: " + e.Message);
        Time.timeScale = 1.0f;
    }
}
    // Klon aktif mi kontrolü
    public bool IsCloneActive()
    {
        return activeClone != null;
    }

// Klonu oyuncuya getirme sekansını başlat
private void StartCloneReturnSequence()
{
        // Kayıt durdurma sesini çal
    if (audioSource != null && recordStopSound != null)
    {
        audioSource.PlayOneShot(recordStopSound);
    }
    // Zamanı durdur
    Time.timeScale = 0f;
    
    // Oyuncu kontrolünü devre dışı bırak
    if (playerMovement != null)
    {
        playerMovement.enabled = false;
    }
    
    // Klonu oyuncuya getiren coroutine'i başlat
    StartCoroutine(ReturnCloneToPlayer());
}

// Klonu oyuncuya getir ve hologramları yok et
private IEnumerator ReturnCloneToPlayer()
{
    if (activeClone != null)
    {
        // Tüm hologramları oluşturma sıralarına göre bir yola dönüştür
        List<Vector3> path = new List<Vector3>();
        
        // Hologramları listeye ekle
        foreach (GameObject afterimage in afterimages)
        {
            if (afterimage != null)
            {
                path.Add(afterimage.transform.position);
            }
        }
        
        // Klonun başlangıç noktası ve oyuncunun final pozisyonu
        Vector3 startPosition = activeClone.transform.position;
        Vector3 playerPosition = transform.position;
        
        // Eğer hiç hologram yoksa, direkt oyuncuya git
        if (path.Count == 0)
        {
            // Anında git - hareket olmadan
            activeClone.transform.position = playerPosition;
        }
        else
        {
            // OPTIMIZASYON: Çok fazla hologram varsa aralarından seç
            List<Vector3> optimizedPath = new List<Vector3>();
            int step = path.Count > 20 ? 4 : (path.Count > 10 ? 2 : 1); // Hologram sayısına göre adım belirle
            
            for (int i = 0; i < path.Count; i += step)
            {
                optimizedPath.Add(path[i]);
            }
            
            // Son hologramı ekle (eğer zaten eklenmemişse)
            if (path.Count > 0 && optimizedPath[optimizedPath.Count - 1] != path[path.Count - 1])
            {
                optimizedPath.Add(path[path.Count - 1]);
            }
            
            // İşlenen hologramları takip etmek için liste
            List<GameObject> processedImages = new List<GameObject>();
            
            // Optimize edilmiş yol üzerinde ilerle
            for (int i = 0; i < optimizedPath.Count; i++)
            {
                Vector3 nextTarget = optimizedPath[i];
                
                // HIZLANDIRMA: Karmaşık hesaplamalar yerine doğrudan pozisyona git
                activeClone.transform.position = nextTarget;
                
                // Yakındaki tüm hologramları bul ve yok et
                for (int j = 0; j < afterimages.Count; j++)
                {
                    if (afterimages[j] != null && Vector3.Distance(afterimages[j].transform.position, nextTarget) < 1.0f)
                    {
                        Destroy(afterimages[j]);
                        processedImages.Add(afterimages[j]);
                    }
                }
                
                yield return new WaitForSecondsRealtime(0.01f);
                // Bekleme süresini tamamen kaldır

                
                
                // Çok kısa bir bekleme - görüntü güncellemesi için tek bir frame
                yield return null;
            }
            
            // İşlenen hologramları listeden çıkar
            foreach (GameObject img in processedImages)
            {
                if (afterimages.Contains(img))
                    afterimages.Remove(img);
            }
            
            // Son olarak oyuncunun konumuna anında git
            activeClone.transform.position = playerPosition;
        }
        
        // Son kalan hologramları temizle
        ClearAfterimages();
        
        // Klonu yok et - bekleme yapmadan hemen
        Destroy(activeClone);
        activeClone = null;
        
        // Kayıt modunu kapat
        isRecording = false;
        
        // Zamanı ve oyuncu kontrolünü geri yükle
        RestoreTimeAndPlayerControl();
        
        Debug.Log("Klon ışık hızında oyuncuya döndü!");
    }
    else
    {
        RestoreTimeAndPlayerControl();
    }
}

}

