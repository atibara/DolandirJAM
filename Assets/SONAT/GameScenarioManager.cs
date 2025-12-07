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

    [Header("INTRO PANELS (Bölüm Baþlangýç Panelleri)")]
    public List<GameObject> PhasePanels;

    [Header("MINIGAME OBJECTS")]
    public List<GameObject> MinigameObjects;

    [Header("Trigger Locations")]
    [SerializeField] private Transform playerSeat;
    [SerializeField] private Transform restroomPointPlayer;

    [Header("Components")]
    [SerializeField] private Guys statsManager;

    [Header("UI System")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image characterPortraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
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
        if (selectionPanel) selectionPanel.SetActive(false);
        foreach (var p in PhasePanels) if (p) p.SetActive(false);
        foreach (var g in MinigameObjects) if (g) g.SetActive(false);
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

        if (CurrentPhase == 1 && playerSeat) playerSeat.gameObject.SetActive(true);
        if (CurrentPhase == 2 && restroomPointPlayer) restroomPointPlayer.gameObject.SetActive(true);

        if (CurrentPhase == 0) StartMinigame(0, OnMinigame1Complete);
        else if (CurrentPhase == 1) IsGamePlaying = true;
        else if (CurrentPhase == 2) IsGamePlaying = true;
    }

    public void TriggerNerdCollision()
    {
        if (!IsGamePlaying || CurrentPhase != 0) return;
        OpenPhasePanel();
    }

    private void CheckArrivalTriggers()
    {
        if (CurrentPhase == 1)
        {
            float dist = Vector3.Distance(PlayerTransform.position, playerSeat.position);
            if (dist < 0.5f)
            {
                playerSeat.gameObject.SetActive(false);
                StartMinigame(1, OnMinigame2Complete);
            }
        }

        if (CurrentPhase == 2)
        {
            float dist = Vector3.Distance(PlayerTransform.position, restroomPointPlayer.position);
            if (dist < 0.8f)
            {
                restroomPointPlayer.gameObject.SetActive(false);
                StartMinigame(2, OnMinigame3Complete);
            }
        }
    }

    private void StartMinigame(int gameIndex, System.Action onComplete)
    {
        IsGamePlaying = false;
        IsMinigameActive = true;

        if (gameIndex < MinigameObjects.Count && MinigameObjects[gameIndex] != null)
        {
            GameObject gameObj = MinigameObjects[gameIndex];
            gameObj.SetActive(true);

            Button btn = gameObj.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    gameObj.SetActive(false);
                    IsMinigameActive = false;
                    onComplete?.Invoke();
                });
            }
        }
        else
        {
            IsMinigameActive = false;
            onComplete?.Invoke();
        }
    }

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

    // --- BURASI GÜNCELLENDÝ: ARTIK RENK DE DEÐÝÞÝYOR ---
    private void DisplayNextSentence()
    {
        if (currentDialogueQueue.Count == 0) { EndDialogue(); return; }

        DialogueLine line = currentDialogueQueue.Dequeue();

        // 1. Yazýyý güncelle
        dialogueText.text = line.sentence;

        // 2. Resmi güncelle
        if (line.characterImage != null)
            characterPortraitImage.sprite = line.characterImage;

        // 3. RENGI GÜNCELLE (YENÝ ÖZELLÝK)
        // Eðer diyalog verisinde renk þeffaf (Alpha=0) gelirse varsayýlan Beyaz yap ki görünmez olmasýn
        // (Ama sen bilerek þeffaf yapmak istersen kodunu ona göre ayarlayabilirsin, þu an direkt atýyor)
        if (line.portraitColor.a == 0 && line.portraitColor.r == 0 && line.portraitColor.g == 0 && line.portraitColor.b == 0)
        {
            // Eðer kullanýcý renk seçmeyi unuttuysa resim kaybolmasýn diye otomatik Beyaz yap
            characterPortraitImage.color = Color.white;
        }
        else
        {
            characterPortraitImage.color = line.portraitColor;
        }
    }

    private void EndDialogue() { dialoguePanel.SetActive(false); isDialogueActive = false; onDialogueEndCallback?.Invoke(); }
    public void SelectSportsGuy() { if (statsManager.SPORTSLOVE > 10) Debug.Log("MUTLU SON"); else Debug.Log("RED"); }
}