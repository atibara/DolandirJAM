using UnityEngine;

public class TopDownController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f; // Dönüþ yumuþaklýðý

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Inputlarý her karede alýyoruz
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        // Fizik iþlemlerini burada yapýyoruz
        Move();
        RotateTowardMovement();
    }

    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A-D veya Sol-Sað Ok
        float moveY = Input.GetAxisRaw("Vertical");   // W-S veya Yukarý-Aþaðý Ok

        movementInput = new Vector2(moveX, moveY).normalized; // Çaprazda hýzlanmayý engeller
    }

    private void Move()
    {
        // Karakteri hareket ettir
        rb.linearVelocity = movementInput * moveSpeed;
        // Not: Unity 6'da 'velocity' yerine 'linearVelocity' tavsiye edilir, 
        // eðer eski sürümden uyarý alýrsan burayý 'rb.velocity' yapabilirsin.
    }

    private void RotateTowardMovement()
    {
        // Eðer karakter hareket ediyorsa
        if (movementInput != Vector2.zero)
        {
            // Hareket yönüne göre açýyý hesapla
            float targetAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;

            // Sprite'ýnýn varsayýlan yönü SAÐ (Right) ise -90 çýkarmana gerek yok.
            // Eðer Sprite'ýn YUKARI bakýyorsa, (targetAngle - 90) yapmalýsýn.
            // Genelde Unity 2D'de "Sað" 0 derecedir.

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // Yumuþak dönüþ için Slerp kullanýyoruz
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}