using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Altyazı metni için TextMeshPro bileşeni
    public float displayDurationPerLine = 3f; // Her bir diyalog satırının ekranda kalma süresi

    private Coroutine currentDialogueCoroutine;

    // Diyaloglar ve konuşmacılar
    private string[] dialogues = {
        "hello, where am i?, what is this place?",
        "relax",
        "who is that. god is that you",
        "relax man. im the developer of this game",
        "so you are not god.",
        "tovbe hasa man no. i said am the developer of this game",
        "what game?",
        "this game.",
        "so im in a game?",
        "yes",
        "alrigth so why the hell i'm in a game?, why am am i a frog? and why do i have a stupid red bandana on my head?",
        "you are asking too many questions.",
        "aight get me out of this place.",
        "i cant.",
        "what?why?",
        "cause there is a problem in unity program and i cant remove you.",
        "you are just lazy and you dont want to spend your time dont you?",
        "yea.",
        "so what happens now?",
        "can you see any apple around there?",
        "no.",
        "there must be one.find it. find and eat it. after you eat it you'll probably be gone.",
        "how can an apple remove me from this game",
        "because i coded it that way.",
        "oh great.",
        "there is a big parkour in your way. ill help you with superpowers.",
        "what superpowers?",
        "there. now you can slow down time and double jump.",
        "niice",
        "okey im leaving now",
        "where are you going",
        "to get a coffee. gamejam made me very tired you know.",
        "oh",
        "anyways. have fun...",
        "thanks god.",
        "im not god bro. just play the damn game!",
        "fine.",
        "good luck",
        "thanks."
    };

    private string[] speakers = {
        "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER",
        "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER",
        "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER",
        "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER",
        "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER", "FROG", "DEVELOPER"
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
