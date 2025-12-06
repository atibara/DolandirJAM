using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool isNerd = false;

    [Header("Wander Settings (Serbest Mod)")]
    [SerializeField] private bool enableWandering = false; // Diðerleri için iþaretle
    [SerializeField] private Transform wanderCenter; // Kimin etrafýnda gezecek? (Nerd)
    [SerializeField] private float wanderRadius = 3f; // Ne kadar yakýnda gezsin?
    [SerializeField] private float wanderInterval = 2f; // Kaç saniyede bir yeni yere gitsin?

    [Header("Start Target (Sadece Nerd Ýçin)")]
    [SerializeField] private Transform initialTarget; // Oyun baþlar baþlamaz gideceði yer

    private Vector3 currentDestination;
    private bool hasDestination = false; // Þu an bir emri var mý?
    private bool isScriptedSequence = false; // Hikaye modu baþladý mý?

    private float wanderTimer;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Oyun baþladýðýnda Nerd ise hemen baþlangýç noktasýna (çarpýþma yerine) gitsin
        if (isNerd && initialTarget != null)
        {
            SetDestination(initialTarget.position);
        }
    }

    private void Update()
    {
        // Eðer hikaye modunda deðilsek ve Wandering açýksa
        if (!isScriptedSequence && !isNerd && enableWandering)
        {
            HandleWandering();
        }

        // Hedefe gitme iþlemi (Hem hikaye hem wander için ortak)
        if (hasDestination)
        {
            MoveToPosition(currentDestination);

            if (Vector3.Distance(transform.position, currentDestination) < 0.1f)
            {
                hasDestination = false;
                rb.linearVelocity = Vector2.zero; // Dur
            }
        }
    }

    private void HandleWandering()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0 && wanderCenter != null)
        {
            // Nerd'ün etrafýnda rastgele bir nokta bul
            Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
            Vector3 targetPos = wanderCenter.position + new Vector3(randomPoint.x, randomPoint.y, 0);

            SetDestination(targetPos);

            // Sýradaki hareket için rastgele bekleme süresi
            wanderTimer = wanderInterval;
        }
    }

    private void MoveToPosition(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    // GameManager burayý çaðýrdýðýnda NPC artýk kendi kafasýna göre gezmeyi býrakýr
    public void SetDestination(Vector3 target, bool isSequenceOrder = false)
    {
        if (isSequenceOrder)
        {
            isScriptedSequence = true; // Artýk hikaye modundayýz, wander iptal.
        }

        currentDestination = target;
        hasDestination = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isNerd && collision.gameObject.CompareTag("Player"))
        {
            // Çarpýþma olduðunda GameManager'a haber ver
            FindAnyObjectByType<GameSequenceManager>().OnNerdCollision();
        }
    }

    // Editörde yarýçapý görebilmek için gizmo (Sarý çember)
    private void OnDrawGizmosSelected()
    {
        if (wanderCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wanderCenter.position, wanderRadius);
        }
    }
}