using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool isNerd = false;
    [SerializeField] private Transform targetToFollow;

    private Vector3 currentDestination;
    private bool hasDestination = false;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (targetToFollow != null)
        {
            MoveToPosition(targetToFollow.position);
        }

        if (hasDestination)
        {
            MoveToPosition(currentDestination);

            if (Vector3.Distance(transform.position, currentDestination) < 0.1f)
            {
                hasDestination = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void MoveToPosition(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isNerd && collision.gameObject.CompareTag("Player"))
        {
            FindAnyObjectByType<GameSequenceManager>().OnNerdCollision();
        }
    }

    public void SetDestination(Vector3 target)
    {
        targetToFollow = null;
        currentDestination = target;
        hasDestination = true;
    }

    public void StopFollowing()
    {
        targetToFollow = null;
        hasDestination = false;
        rb.linearVelocity = Vector2.zero;
    }
}