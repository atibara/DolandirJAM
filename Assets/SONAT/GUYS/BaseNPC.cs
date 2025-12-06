using UnityEngine;
using UnityEngine.AI;

public abstract class BaseNPC : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected float originalSpeed;
    [Header("Base Settings")]
    [SerializeField] protected float wanderRadius = 5f;
    private float wanderTimer;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        originalSpeed = agent.speed;
    }

    protected virtual void Update()
    {
        // SADECE OYUN OYNANIYORSA HAREKET ET
        // Minigame varsa veya Panel açýksa dur.
        if (!GameScenarioManager.Instance.IsGamePlaying)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        // Hareket serbest
        if (agent.isOnNavMesh) agent.isStopped = false;

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        ExecutePhaseBehavior(GameScenarioManager.Instance.CurrentPhase);
    }

    protected abstract void ExecutePhaseBehavior(int phase);

    protected void MoveTo(Vector3 target)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    // Diðer metodlar ayný...
    protected void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            Vector3 randomPos = (Vector3)Random.insideUnitCircle * wanderRadius + transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, wanderRadius, NavMesh.AllAreas))
                MoveTo(hit.position);
            wanderTimer = Random.Range(3f, 6f);
        }
    }
    protected void Stop() { if (agent.isOnNavMesh) agent.isStopped = true; }
}