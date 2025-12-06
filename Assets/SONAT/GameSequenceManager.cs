using UnityEngine;

public class GameSequenceManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private NPCController nerdNPC;
    [SerializeField] private NPCController punkNPC;
    [SerializeField] private NPCController jockNPC;
    [SerializeField] private NPCController girl1NPC;
    [SerializeField] private NPCController girl2NPC;

    [Header("Seat Positions")]
    [SerializeField] private Transform playerSeat;
    [SerializeField] private Transform nerdSeat;
    [SerializeField] private Transform punkSeat;
    [SerializeField] private Transform jockSeat;
    [SerializeField] private Transform girl1Seat;
    [SerializeField] private Transform girl2Seat;

    [Header("Other Targets")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform girlsRestroomPoint;
    [SerializeField] private Transform playerRestroomPoint;

    private bool isExamSequenceActive = false;
    private bool isRestroomSequenceActive = false;

    private void Update()
    {
        if (isExamSequenceActive)
        {
            CheckPlayerAtSeat();
        }

        if (isRestroomSequenceActive)
        {
            CheckPlayerAtRestroom();
        }
    }

    public void OnNerdCollision()
    {
        Debug.Log("Nerd Collision Triggered!");
        StartExamSequence();
    }

    private void StartExamSequence()
    {
        isExamSequenceActive = true;

        // true parametresi gönderiyoruz ki Wander modundan çýksýnlar
        nerdNPC.SetDestination(nerdSeat.position, true);
        punkNPC.SetDestination(punkSeat.position, true);
        jockNPC.SetDestination(jockSeat.position, true);
        girl1NPC.SetDestination(girl1Seat.position, true);
        girl2NPC.SetDestination(girl2Seat.position, true);
    }

    private void CheckPlayerAtSeat()
    {
        if (Vector3.Distance(playerTransform.position, playerSeat.position) < 0.5f)
        {
            isExamSequenceActive = false;
            OnExamStart();
        }
    }

    private void OnExamStart()
    {
        Debug.Log("Exam Started!");
        Invoke(nameof(EndExamAndStartRestroomSequence), 3f);
    }

    private void EndExamAndStartRestroomSequence()
    {
        nerdNPC.SetDestination(exitPoint.position, true);
        punkNPC.SetDestination(exitPoint.position, true);
        jockNPC.SetDestination(exitPoint.position, true);

        girl1NPC.SetDestination(girlsRestroomPoint.position, true);
        girl2NPC.SetDestination(girlsRestroomPoint.position, true);

        isRestroomSequenceActive = true;
    }

    private void CheckPlayerAtRestroom()
    {
        if (Vector3.Distance(playerTransform.position, playerRestroomPoint.position) < 0.8f)
        {
            isRestroomSequenceActive = false;
            OnGameStart();
        }
    }

    private void OnGameStart()
    {
        Debug.Log("Main Game Started!");
    }




}