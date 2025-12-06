using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public Sprite characterImage;
    [TextArea(3, 10)] public string sentence;
}

[System.Serializable]
public class DialogueSequence
{
    public DialogueLine[] lines;
}