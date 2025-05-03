using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kamera hedef nesneyi takip edecek �ekilde hareket edecek
public class CameraFollow : MonoBehaviour
{
    // Takip edilecek hedef nesne
    public GameObject targetObject;

    // Kameran�n hedef nesneye g�re sabit kalmas� gereken mesafe (ofset)
    public Vector3 cameraOffset;

    // Kameran�n hedeflenen pozisyonu (hesaplanan pozisyon)
    public Vector3 targetedPosition;

    // SmoothDamp fonksiyonu i�in gerekli h�z vekt�r� (Unity'nin yumu�atma fonksiyonu taraf�ndan kullan�l�r)
    private Vector3 velocity = Vector3.zero;

    // Kameran�n hedef pozisyona ge�i�inin ne kadar s�rede ger�ekle�ece�ini kontrol eder
    public float smoothTime = 0.3F;

    // Update metodu her karede �a�r�l�r. Ancak LateUpdate, di�er Update metodlar�ndan sonra �al���r.
    void LateUpdate()
    {
        // Hedef pozisyon hesaplan�r: hedef nesnenin pozisyonuna ofset eklenir
        targetedPosition = targetObject.transform.position + cameraOffset;

        // Kameran�n mevcut pozisyonu, hedef pozisyona do�ru yumu�at�larak hareket ettirilir
        // SmoothDamp, kameran�n belirli bir s�re i�inde hedef pozisyona ge�i�ini sa�lar
        transform.position = Vector3.SmoothDamp(transform.position, targetedPosition, ref velocity, smoothTime);
    }
}