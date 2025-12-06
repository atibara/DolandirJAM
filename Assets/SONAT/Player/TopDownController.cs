using UnityEngine;

public class TopDownController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // EKLEME: Eðer Manager oyunu durdurduysa (Panel açýksa), hareket girdisi alma
        if (GameScenarioManager.Instance != null && !GameScenarioManager.Instance.IsGamePlaying)
        {
            movementInput = Vector2.zero; // Hareketi sýfýrla
            return;
        }

        ProcessInputs();
    }

    private void FixedUpdate()
    {
        // EKLEME: Oyun durduysa fiziksel hareketi de kes (Kaymayý engeller)
        if (GameScenarioManager.Instance != null && !GameScenarioManager.Instance.IsGamePlaying)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Move();
        RotateTowardMovement();
    }

    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(moveX, moveY).normalized;
    }

    private void Move()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }

    private void RotateTowardMovement()
    {
        if (movementInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}