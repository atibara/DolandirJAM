using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameScenarioManager : MonoBehaviour
{
    public static GameScenarioManager Instance;

    [Header("Game State")]
    public int CurrentPhase = 0;

    public bool IsPanelActive = false;
    public bool IsMinigameActive = false;
    public bool IsGamePlaying = false;

    [Header("Global References")]
    public Transform PlayerTransform;

    [Header("YOUR PANELS (Senin Panellerin)")]
    public List<GameObject> PhasePanels;

    [Header("Trigger Locations (EFEKTLERÝN OLDUÐU OBJELER)")]
    [SerializeField] private Transform playerSeat;
    [SerializeField] private Transform restroomPointPlayer;

    [Header("Components")]
    [SerializeField] private Guys statsManager;

    [Header("UI System")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image characterPortraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject minigamePlaceholderPanel;
    [SerializeField] private TextMeshProUGUI minigameInfoText;
    [SerializeField] private GameObject selectionPanel;

    [Header("Dialogues")]
    [SerializeField] private DialogueSequence introDialogue;
    [SerializeField] private DialogueSequence examDialogue;
    [SerializeField] private DialogueSequence restroomDialogue;

    private Queue<DialogueLine> currentDialogueQueue;
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        currentDialogueQueue = new Queue<DialogueLine>();
    }

    private void Start()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (minigamePlaceholderPanel) minigamePlaceholderPanel.SetActive(false);
        if (selectionPanel) selectionPanel.SetActive(false);
        foreach (var p in PhasePanels) if (p) p.SetActive(false);

        // 1. EFEKT OBJELERÝNÝ BAÞLANGIÇTA GÝZLE
        if (playerSeat) playerSeat.gameObject.SetActive(false);
        if (restroomPointPlayer) restroomPointPlayer.gameObject.SetActive(false);

        IsGamePlaying = true;
        IsPanelActive = false;
        IsMinigameActive = false;
    }

    private void Update()
    {
        if (IsPanelActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) ClosePanelAndStartEvent();
            return;
        }

        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) DisplayNextSentence();
            return;
        }

        if (IsGamePlaying)
        {
            CheckArrivalTriggers();
        }
    }

    public void OpenPhasePanel()
    {
        IsGamePlaying = false;
        IsPanelActive = true;

        if (CurrentPhase < PhasePanels.Count && PhasePanels[CurrentPhase] != null)
        {
            PhasePanels[CurrentPhase].SetActive(true);
        }
    }

    private void ClosePanelAndStartEvent()
    {
        if (CurrentPhase < PhasePanels.Count && PhasePanels[CurrentPhase] != null)
        {
            PhasePanels[CurrentPhase].SetActive(false);
        }

        IsPanelActive = false;

        // 2. PANEL KAPANDI, GÖREV BAÞLIYOR -> EFEKTÝ AÇ!
        // Eðer Faz 1 (Sýnav) ise Sýrayý göster
        if (CurrentPhase == 1 && playerSeat)
            playerSeat.gameObject.SetActive(true);

        // Eðer Faz 2 (Tuvalet) ise Tuvalet noktasýný göster
        if (CurrentPhase == 2 && restroomPointPlayer)
            restroomPointPlayer.gameObject.SetActive(true);


        // Faz 0 ise direkt minigame, diðerlerinde yürüme baþlýyor
        if (CurrentPhase == 0) StartMinigame("Nerd ile çarpýþtýn! Eþyalarý topla.", OnMinigame1Complete);
        else
        {
            IsGamePlaying = true; // Yürümeye baþla
        }
    }

    public void TriggerNerdCollision()
    {
        if (!IsGamePlaying || CurrentPhase != 0) return;
        OpenPhasePanel();
    }

    private void CheckArrivalTriggers()
    {
        // FAZ 1: Sýnav Sýrasýna Varýþ
        if (CurrentPhase == 1)
        {
            float dist = Vector3.Distance(PlayerTransform.position, playerSeat.position);
            if (dist < 0.5f)
            {
                // 3. HEDEFE VARDIN -> EFEKTÝ KAPAT
                playerSeat.gameObject.SetActive(false);
                OpenPhasePanel();
            }
        }

        // FAZ 2: Tuvalete Varýþ
        if (CurrentPhase == 2)
        {
            float dist = Vector3.Distance(PlayerTransform.position, restroomPointPlayer.position);
            if (dist < 0.8f)
            {
                // 3. HEDEFE VARDIN -> EFEKTÝ KAPAT
                restroomPointPlayer.gameObject.SetActive(false);
                OpenPhasePanel();
            }
        }
    }

    // --- SONRASI AYNI (Minigame Logic) ---
    public void AdvancePhase()
    {
        CurrentPhase++;
        IsGamePlaying = true;
    }

    private void OnMinigame1Complete()
    {
        if (statsManager) { statsManager.addNerdLove(5); statsManager.addSportsLove(5); }
        StartDialogue(introDialogue, AdvancePhase);
    }

    private void OnMinigame2Complete()
    {
        if (statsManager) { statsManager.addNerdLove(-5); statsManager.addPunkLove(5); statsManager.addSportsLove(5); }
        StartDialogue(examDialogue, AdvancePhase);
    }

    private void OnMinigame3Complete()
    {
        if (statsManager) { statsManager.addSportsLove(5); statsManager.addPunkLove(5); }
        StartDialogue(restroomDialogue, () => selectionPanel.SetActive(true));
    }

    private void StartMinigame(string info, System.Action onComplete)
    {
        IsMinigameActive = true;
        minigamePlaceholderPanel.SetActive(true);
        minigameInfoText.text = info;
        minigamePlaceholderPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        minigamePlaceholderPanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
            minigamePlaceholderPanel.SetActive(false);
            IsMinigameActive = false;
            onComplete?.Invoke();
        });
    }

    private System.Action onDialogueEndCallback;
    private void StartDialogue(DialogueSequence sequence, System.Action onEnd)
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        onDialogueEndCallback = onEnd;
        currentDialogueQueue.Clear();
        if (sequence != null && sequence.lines != null)
            foreach (var line in sequence.lines) currentDialogueQueue.Enqueue(line);
        DisplayNextSentence();
    }
    private void DisplayNextSentence()
    {
        if (currentDialogueQueue.Count == 0) { EndDialogue(); return; }
        DialogueLine line = currentDialogueQueue.Dequeue();
        dialogueText.text = line.sentence;
        characterPortraitImage.sprite = line.characterImage;
        if (line.characterImage == null) characterPortraitImage.color = Color.white;
    }
    private void EndDialogue() { dialoguePanel.SetActive(false); isDialogueActive = false; onDialogueEndCallback?.Invoke(); }
    public void SelectSportsGuy() { if (statsManager.SPORTSLOVE > 10) Debug.Log("MUTLU SON"); else Debug.Log("RED"); }
}