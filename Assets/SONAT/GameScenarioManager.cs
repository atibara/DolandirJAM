using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameScenarioManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Guys statsManager;
    [SerializeField] private Transform playerTransform;

    [Header("NPCs")]
    [SerializeField] private NPCController nerdNPC;
    [SerializeField] private NPCController punkNPC;
    [SerializeField] private NPCController sportsNPC;
    [SerializeField] private NPCController girl1;
    [SerializeField] private NPCController girl2;

    [Header("Locations")]
    [SerializeField] private Transform nerdCollisionPoint;
    [SerializeField] private Transform playerSeat;
    [SerializeField] private Transform nerdSeat;
    [SerializeField] private Transform punkSeat;
    [SerializeField] private Transform sportsSeat;
    [SerializeField] private Transform girl1Seat;
    [SerializeField] private Transform girl2Seat;
    [SerializeField] private Transform restroomPointGirl;
    [SerializeField] private Transform restroomPointPlayer;

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
    private int currentPhase = 0;
    private bool waitingForInteraction = false;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        minigamePlaceholderPanel.SetActive(false);
        selectionPanel.SetActive(false);
        currentDialogueQueue = new Queue<DialogueLine>();

        StartPhase1_IntroMovement();
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
            return;
        }

        CheckPhaseProgression();
    }

    private void CheckPhaseProgression()
    {
        if (currentPhase == 0)
        {
            float dist = Vector3.Distance(playerTransform.position, nerdCollisionPoint.position);
            if (dist < 1.0f && !waitingForInteraction)
            {
                waitingForInteraction = true;
                nerdNPC.StopMoving();
                StartMinigame("Nerd'ün Eþyalarýný Topla!\n(Týkla: Tamamla)", OnMinigame1Complete);
            }
        }
        else if (currentPhase == 1)
        {
            float dist = Vector3.Distance(playerTransform.position, playerSeat.position);
            if (dist < 0.5f && !waitingForInteraction)
            {
                waitingForInteraction = true;
                StartMinigame("Sýnav Baþladý! Kopyalarý Daðýt.\n(Týkla: Tamamla)", OnMinigame2Complete);
            }
        }
        else if (currentPhase == 2)
        {
            float dist = Vector3.Distance(playerTransform.position, restroomPointPlayer.position);
            if (dist < 0.5f && !waitingForInteraction)
            {
                waitingForInteraction = true;
                StartMinigame("Sistemi Hackle!\n(Týkla: Tamamla)", OnMinigame3Complete);
            }
        }
    }

    private void StartPhase1_IntroMovement()
    {
        currentPhase = 0;
        nerdNPC.MoveTo(nerdCollisionPoint.position);

        punkNPC.StartWandering();
        sportsNPC.StartWandering();
        girl1.StartWandering();
        girl2.StartWandering();
    }

    private void OnMinigame1Complete()
    {
        statsManager.addNerdLove(5);
        statsManager.addSportsLove(5);

        StartDialogue(introDialogue, StartPhase2_ClassMovement);
    }

    private void StartPhase2_ClassMovement()
    {
        currentPhase = 1;
        waitingForInteraction = false;

        nerdNPC.MoveTo(nerdSeat.position);
        punkNPC.MoveTo(punkSeat.position);
        sportsNPC.MoveTo(sportsSeat.position);
        girl1.MoveTo(girl1Seat.position);
        girl2.MoveTo(girl2Seat.position);
    }

    private void OnMinigame2Complete()
    {
        statsManager.addNerdLove(-5);
        statsManager.addPunkLove(5);
        statsManager.addSportsLove(5);

        StartDialogue(examDialogue, StartPhase3_RestroomMovement);
    }

    private void StartPhase3_RestroomMovement()
    {
        currentPhase = 2;
        waitingForInteraction = false;

        girl1.MoveTo(restroomPointGirl.position);

        nerdNPC.StartWandering();
        punkNPC.StartWandering();
        sportsNPC.StartWandering();
        girl2.StartWandering();
    }

    private void OnMinigame3Complete()
    {
        statsManager.addSportsLove(5);
        statsManager.addPunkLove(5);

        StartDialogue(restroomDialogue, StartSelectionPhase);
    }

    private void StartSelectionPhase()
    {
        selectionPanel.SetActive(true);
    }

    public void SelectSportsGuy()
    {
        if (statsManager.SPORTSLOVE > 10)
        {
            Debug.Log("MUTLU SON! Sporcu ile sevgili oldun.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("REDDEDÝLDÝN! Puan yetersiz.");
        }
    }

    private void StartMinigame(string info, System.Action onCompleteCallback)
    {
        minigamePlaceholderPanel.SetActive(true);
        minigameInfoText.text = info;

        Button btn = minigamePlaceholderPanel.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            minigamePlaceholderPanel.SetActive(false);
            onCompleteCallback?.Invoke();
        });
    }

    private System.Action onDialogueEndCallback;

    private void StartDialogue(DialogueSequence sequence, System.Action onEnd)
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        onDialogueEndCallback = onEnd;
        currentDialogueQueue.Clear();

        foreach (DialogueLine line in sequence.lines)
        {
            currentDialogueQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (currentDialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogueQueue.Dequeue();
        dialogueText.text = line.sentence;
        if (line.characterImage != null)
            characterPortraitImage.sprite = line.characterImage;
        else
            characterPortraitImage.color = Color.clear;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        onDialogueEndCallback?.Invoke();
    }
}