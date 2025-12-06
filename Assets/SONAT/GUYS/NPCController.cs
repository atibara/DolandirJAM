using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool enableWandering = false;
    [SerializeField] private float wanderRadius = 5f;

    private NavMeshAgent agent;
    private bool isWandering;
    private float wanderTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (isWandering && enableWandering)
        {
            HandleWandering();
        }
    }

    private void HandleWandering()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
            wanderTimer = Random.Range(3f, 6f);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // Eðer NavMesh üzerinde deðilse (henüz bake olmadýysa veya havadaysa) iþlem yapma
        if (!agent.isOnNavMesh)
        {
            // Agent'ý en yakýn NavMesh noktasýna ýþýnlamayý dene
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // Ajaný zemine yapýþtýr
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} bir NavMesh üzerinde deðil! Hareket iptal edildi.");
                return;
            }
        }

        isWandering = false;
        agent.isStopped = false; // "Resume" hatasý veren yer burasýydý, artýk güvenli.
        agent.SetDestination(targetPosition);
    }

    public void StartWandering()
    {
        isWandering = true;
        agent.isStopped = false;
    }

    public void StopMoving()
    {
        isWandering = false;
        agent.isStopped = true;
        agent.ResetPath();
    }
}