using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameScenarioManager : MonoBehaviour
{
    public static GameScenarioManager Instance;
    public Guys StatsManager => statsManager;

    [Header("Game State")]
    public int CurrentPhase = 0;
    public bool IsPanelActive = false;
    public bool IsMinigameActive = false;
    public bool IsGamePlaying = false;

    [Header("Global References")]
    public Transform PlayerTransform;
    [SerializeField] private Guys statsManager;

    [Header("INTRO PANELS & MINIGAMES")]
    public List<GameObject> PhasePanels;
    public List<GameObject> MinigameObjects;

    [Header("Trigger Locations")]
    [SerializeField] private Transform playerSeat;
    [SerializeField] private Transform restroomPointPlayer;

    [Header("UI System")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image characterPortraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject selectionPanel;

    [Header("--- PHASE 1: KORÝDOR SONU ---")]
    public DialogueSequence dia_P1_NerdHappy;
    public DialogueSequence dia_P1_SerseriHappy;
    public DialogueSequence dia_P1_SporcuHappy;

    [Header("--- PHASE 2: SINAV SONU ---")]
    public DialogueSequence dia_P2_NerdAngry;
    public DialogueSequence dia_P2_SporcuHappy;
    public DialogueSequence dia_P2_Standard;

    [Header("--- PHASE 3: TUVALET SONU ---")]
    public DialogueSequence dia_P3_Success;
    public DialogueSequence dia_P3_Fail;

    [Header("--- FÝNAL SENARYOLARI ---")]
    public DialogueSequence dia_End_Nerd_Good;
    public DialogueSequence dia_End_Nerd_Bad;
    public DialogueSequence dia_End_Sporcu_Good;
    public DialogueSequence dia_End_Sporcu_Bad;
    public DialogueSequence dia_End_Serseri_Good;
    public DialogueSequence dia_End_Serseri_Bad;

    private Queue<DialogueLine> currentDialogueQueue = new Queue<DialogueLine>();
    private Queue<DialogueSequence> dialogueSequenceQueue = new Queue<DialogueSequence>();
    private bool isDialogueActive = false;
    private System.Action onDialogueEndCallback;
    private DialogueSequence currentDialogueSequence;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentDialogueQueue = new Queue<DialogueLine>();
        dialogueSequenceQueue = new Queue<DialogueSequence>();
    }

    private void Start()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (selectionPanel) selectionPanel.SetActive(false);
        foreach (var p in PhasePanels) if (p) p.SetActive(false);
        foreach (var g in MinigameObjects) if (g) g.SetActive(false);
        if (playerSeat) playerSeat.gameObject.SetActive(false);
        if (restroomPointPlayer) restroomPointPlayer.gameObject.SetActive(false);

        CurrentPhase = 0;
        IsGamePlaying = true;
    }

    private void Update()
    {
        if (IsPanelActive && Input.GetKeyDown(KeyCode.Space))
        {
            ClosePanelAndStartMinigame();
            return;
        }

        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentence();
            return;
        }

        if (IsGamePlaying) CheckArrivalTriggers();
    }

    public void TriggerNerdCollision() { if (CurrentPhase == 0 && IsGamePlaying) OpenPhasePanel(); }

    private void CheckArrivalTriggers()
    {
        if (CurrentPhase == 1 && playerSeat.gameObject.activeSelf)
        {
            if (Vector3.Distance(PlayerTransform.position, playerSeat.position) < 0.8f)
            {
                playerSeat.gameObject.SetActive(false);
                OpenPhasePanel();
            }
        }
        if (CurrentPhase == 2 && restroomPointPlayer.gameObject.activeSelf)
        {
            if (Vector3.Distance(PlayerTransform.position, restroomPointPlayer.position) < 0.8f)
            {
                restroomPointPlayer.gameObject.SetActive(false);
                OpenPhasePanel();
            }
        }
    }

    public void OpenPhasePanel()
    {
        IsGamePlaying = false;
        IsPanelActive = true;
        if (CurrentPhase < PhasePanels.Count && PhasePanels[CurrentPhase])
            PhasePanels[CurrentPhase].SetActive(true);
    }

    private void ClosePanelAndStartMinigame()
    {
        if (CurrentPhase < PhasePanels.Count && PhasePanels[CurrentPhase])
            PhasePanels[CurrentPhase].SetActive(false);
        IsPanelActive = false;

        IsMinigameActive = true;
        if (CurrentPhase < MinigameObjects.Count && MinigameObjects[CurrentPhase])
            MinigameObjects[CurrentPhase].SetActive(true);
    }

    // --- PHASE 1 ---
    public void OnMinigame1Finished_Logic()
    {
        IsMinigameActive = false;
        if (MinigameObjects.Count > 0 && MinigameObjects[0]) MinigameObjects[0].SetActive(false);

        // Nerd'e yardým ettin mi?
        if (statsManager.NERDLOVE > 0) StartDialogue(dia_P1_NerdHappy);
        // Serseriye uydun mu?
        else if (statsManager.PUNKLOVE > 0) StartDialogue(dia_P1_SerseriHappy);

        // Sporcu mantýðý baðýmsýz (Eþyalarý o topladýysa)
        if (statsManager.SPORTSLOVE > 0) StartDialogue(dia_P1_SporcuHappy);

        SetCallbackForEndOfQueue(AdvancePhase);
    }

    // --- PHASE 2 (SINAV) --- 
    // DÜZELTME: helpedSports parametresi eklendi
    public void OnMinigame2Finished_Logic(bool helpedSports)
    {
        IsMinigameActive = false;
        if (MinigameObjects.Count > 1 && MinigameObjects[1]) MinigameObjects[1].SetActive(false);

        bool anyDialogue = false;

        // 1. Nerd Kýzgýn mý? 
        // Puaný -20 düþtüðü için artýk aran iyi olsa bile kesin sinirlenecek.
        if (statsManager.NERDLOVE < -5)
        {
            StartDialogue(dia_P2_NerdAngry);
            anyDialogue = true;
        }

        // 2. Sporcu Mutlu mu? 
        // ARTIK SADECE KOPYA VERÝRSEN (helpedSports == true) ÇALIÞIR. 
        // Eskisi gibi puana bakmýyoruz.
        if (helpedSports)
        {
            StartDialogue(dia_P2_SporcuHappy);
            anyDialogue = true;
        }

        // Eðer hiçbir olay olmadýysa standart
        if (!anyDialogue)
        {
            StartDialogue(dia_P2_Standard);
        }

        SetCallbackForEndOfQueue(AdvancePhase);
    }

    // --- PHASE 3 (HACK) ---
    public void OnMinigame3Finished_Logic(bool isSuccess)
    {
        IsMinigameActive = false;
        if (MinigameObjects.Count > 2 && MinigameObjects[2]) MinigameObjects[2].SetActive(false);

        DialogueSequence dialogueToPlay = isSuccess ? dia_P3_Success : dia_P3_Fail;

        StartDialogue(dialogueToPlay);
        SetCallbackForEndOfQueue(() => selectionPanel.SetActive(true));
    }

    public void SelectFinalCharacter(int charID)
    {
        selectionPanel.SetActive(false);
        DialogueSequence finalDia = null;
        float threshold = 0;

        switch (charID)
        {
            case 0: // Nerd
                if (statsManager.NERDLOVE > threshold) finalDia = dia_End_Nerd_Good;
                else finalDia = dia_End_Nerd_Bad;
                break;
            case 1: // Sporcu
                if (statsManager.SPORTSLOVE > threshold) finalDia = dia_End_Sporcu_Good;
                else finalDia = dia_End_Sporcu_Bad;
                break;
            case 2: // Serseri
                if (statsManager.PUNKLOVE > threshold) finalDia = dia_End_Serseri_Good;
                else finalDia = dia_End_Serseri_Bad;
                break;
        }

        StartDialogue(finalDia);
        SetCallbackForEndOfQueue(() => Debug.Log("OYUN BÝTTÝ."));
    }

    public void AdvancePhase()
    {
        CurrentPhase++;
        IsGamePlaying = true;
        if (CurrentPhase == 1 && playerSeat) playerSeat.gameObject.SetActive(true);
        if (CurrentPhase == 2 && restroomPointPlayer) restroomPointPlayer.gameObject.SetActive(true);
    }

    private void SetCallbackForEndOfQueue(System.Action onEnd) { onDialogueEndCallback = onEnd; }

    public void StartDialogue(DialogueSequence sequence, System.Action onEnd = null)
    {
        if (dialogueSequenceQueue == null) dialogueSequenceQueue = new Queue<DialogueSequence>();
        if (sequence == null || sequence.lines.Count == 0) return;

        dialogueSequenceQueue.Enqueue(sequence);
        if (onEnd != null) onDialogueEndCallback = onEnd;

        if (!isDialogueActive) PlayNextDialogueSequence();
    }

    private void PlayNextDialogueSequence()
    {
        if (dialogueSequenceQueue == null || dialogueSequenceQueue.Count == 0)
        {
            EndDialogueSystem();
            return;
        }

        currentDialogueSequence = dialogueSequenceQueue.Dequeue();

        if (currentDialogueQueue == null) currentDialogueQueue = new Queue<DialogueLine>();
        currentDialogueQueue.Clear();

        foreach (var line in currentDialogueSequence.lines) currentDialogueQueue.Enqueue(line);

        if (dialoguePanel) dialoguePanel.SetActive(true);
        isDialogueActive = true;

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (currentDialogueQueue.Count == 0) { PlayNextDialogueSequence(); return; }

        DialogueLine line = currentDialogueQueue.Dequeue();
        if (dialogueText) dialogueText.text = line.sentence;

        if (characterPortraitImage)
        {
            if (line.characterImage != null)
            {
                characterPortraitImage.sprite = line.characterImage;
                characterPortraitImage.color = Color.white;
            }
            else
            {
                characterPortraitImage.sprite = null;
                characterPortraitImage.color = line.portraitColor;
            }
        }
    }

    private void EndDialogueSystem()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (onDialogueEndCallback != null)
        {
            System.Action callback = onDialogueEndCallback;
            onDialogueEndCallback = null;
            callback.Invoke();
        }
    }
}