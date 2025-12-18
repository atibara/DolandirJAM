using UnityEngine;
using System.Collections.Generic; // List'leri kullanmak için bu þart

[System.Serializable]
public class DialogueLine
{
    [Header("Kim Konuþuyor?")]
    public string characterName; // Ýsim alaný
    public Sprite characterImage; // Resim
    public Color portraitColor = Color.white; // Resim yoksa renk

    [Header("Ne Diyor?")]
    [TextArea(3, 10)]
    public string sentence; // Cümle
}

[System.Serializable]
public class DialogueSequence
{
    public string sequenceID; // Hatýrlatýcý not
    public List<DialogueLine> lines; // Array yerine List kullanýyoruz, düzenlemesi daha kolay
}