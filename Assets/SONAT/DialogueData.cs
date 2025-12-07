using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public Sprite characterImage;

    [Tooltip("Karakter resminin rengi (Varsayýlan Beyaz). Þeffaflýk için Alpha'yý kýsabilirsin.")]
    public Color portraitColor;

    [TextArea(3, 10)] public string sentence;
}

[System.Serializable]
public class DialogueSequence
{
    public DialogueLine[] lines;
}