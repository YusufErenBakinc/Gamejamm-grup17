using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kamera hedef nesneyi takip edecek þekilde hareket edecek
public class CameraFollow : MonoBehaviour
{
    // Takip edilecek hedef nesne
    public GameObject targetObject;

    // Kameranýn hedef nesneye göre sabit kalmasý gereken mesafe (ofset)
    public Vector3 cameraOffset;

    // Kameranýn hedeflenen pozisyonu (hesaplanan pozisyon)
    public Vector3 targetedPosition;

    // SmoothDamp fonksiyonu için gerekli hýz vektörü (Unity'nin yumuþatma fonksiyonu tarafýndan kullanýlýr)
    private Vector3 velocity = Vector3.zero;

    // Kameranýn hedef pozisyona geçiþinin ne kadar sürede gerçekleþeceðini kontrol eder
    public float smoothTime = 0.3F;

    // Update metodu her karede çaðrýlýr. Ancak LateUpdate, diðer Update metodlarýndan sonra çalýþýr.
    void LateUpdate()
    {
        // Hedef pozisyon hesaplanýr: hedef nesnenin pozisyonuna ofset eklenir
        targetedPosition = targetObject.transform.position + cameraOffset;

        // Kameranýn mevcut pozisyonu, hedef pozisyona doðru yumuþatýlarak hareket ettirilir
        // SmoothDamp, kameranýn belirli bir süre içinde hedef pozisyona geçiþini saðlar
        transform.position = Vector3.SmoothDamp(transform.position, targetedPosition, ref velocity, smoothTime);
    }
}