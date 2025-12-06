using UnityEngine;

public class NerdNPC : BaseNPC
{
    [Header("Nerd Settings")]
    [SerializeField] private Transform nerdSeat;
    [SerializeField] private float chaseDistance = 4f;
    [SerializeField] private float contactDistance = 1.2f;

    private bool isChasing = false;
    private bool hasTriggeredCollision = false;

    protected override void ExecutePhaseBehavior(int phase)
    {
        switch (phase)
        {
            case 0: // KORÝDOR
                if (!hasTriggeredCollision)
                    HandleCorridorBehavior();
                break;

            case 1: // SINAV
                hasTriggeredCollision = false;
                isChasing = false;
                MoveTo(nerdSeat.position);
                break;

            case 2: // TUVALET
                Wander();
                break;
        }
    }

    private void HandleCorridorBehavior()
    {
        if (GameScenarioManager.Instance.PlayerTransform == null) return;

        Transform player = GameScenarioManager.Instance.PlayerTransform;
        float dist = Vector3.Distance(transform.position, player.position);

        // Debug.Log($"Nerd Mesafesi: {dist} | Chasing: {isChasing}"); // Sorun devam ederse bu satýrý aç

        // 1. ÇARPIÞMA (Yakalandýn)
        if (dist < contactDistance)
        {
            Stop();
            isChasing = false;
            hasTriggeredCollision = true;
            GameScenarioManager.Instance.TriggerNerdCollision();
            return;
        }

        // 2. KOVALAMA MODU
        if (isChasing)
        {
            // A. ZORLA DÖNDÜRME (Hýzdan baðýmsýz yüzünü sana dönsün)
            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // B. ZORLA YÜRÜTME
            agent.isStopped = false;
            agent.speed = originalSpeed * 1.5f;

            // C. HEDEF ATAMA
            bool pathFound = agent.SetDestination(player.position);

            // Eðer yol bulamýyorsa konsola yazsýn
            if (!pathFound) Debug.LogWarning("Nerd sana giden yolu bulamýyor! NavMesh'i kontrol et.");
        }

        // 3. FARK ETME MODU
        else if (dist < chaseDistance)
        {
            isChasing = true;
            Debug.Log("Nerd seni gördü, geliyor!");
        }
        else
        {
            Stop();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, contactDistance);
    }
}