using System.Collections;
using UnityEngine;
using TMPro;

public class EndingDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Altyazı metni için TextMeshPro bileşeni
    public float displayDurationPerLine = 3f; // Her bir diyalog satırının ekranda kalma süresi

    private Coroutine currentDialogueCoroutine;

    // Diyaloglar ve konuşmacılar
    private string[] dialogues = {
        "good job man. i liked the enthusiasm. is your father a frog or something.",
        "hahaha really funny now get me out of this place. i dont want to see other spike or fire. can i ask something",
        "sure",
        "why apple",
        "this was the only asset that i've found in assetpack.",
        "lazy.",
        "yea. anyways gamejam is over. farewell frog.",
        "likewise developer."
    };

    private string[] speakers = {
        "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG"
    };

    private Color frogColor = Color.white; // Kurbağa için kırmızı
    private Color developerColor = Color.red; // Dış ses için beyaz

    void Start()
    {
        // Diyalogları sırayla başlat
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }

        currentDialogueCoroutine = StartCoroutine(DisplayDialogue());
    }

    private IEnumerator DisplayDialogue()
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            // Konuşmacıya göre renk ayarla
            if (speakers[i] == "FROG")
            {
                dialogueText.color = frogColor;
            }
            else if (speakers[i] == "DEVELOPER")
            {
                dialogueText.color = developerColor;
            }

            // Altyazıyı ayarla ve bekle
            dialogueText.text = $"{speakers[i]}: {dialogues[i]}";
            yield return new WaitForSeconds(displayDurationPerLine);
        }

        // Tüm diyaloglar bittiğinde altyazıyı temizle
        dialogueText.text = "";
    }
}