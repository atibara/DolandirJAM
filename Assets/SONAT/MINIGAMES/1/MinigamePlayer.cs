using UnityEngine;
using System.Collections.Generic;

public class MinigamePlayer : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] private float collectRadius = 100f;
    [SerializeField] private Transform holdPosition;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Þu an elimizde tuttuðumuz eþyalar
    private List<MinigameItem> carriedItems = new List<MinigameItem>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (holdPosition == null) holdPosition = transform;
    }

    private void Update()
    {
        // 1. Hareket
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;

        // 2. EÞYA TOPLAMA MANTIÐI

        // Sadece tuþa ÝLK bastýðýn an etrafýndakileri yakala
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryCollectItems();
        }

        // Tuþu býraktýðýn an hepsini yere býrak
        if (Input.GetKeyUp(KeyCode.K))
        {
            DropAllItems();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void TryCollectItems()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collectRadius);

        foreach (var hit in hits)
        {
            MinigameItem item = hit.GetComponent<MinigameItem>();

            // Eðer item ise ve zaten bizde deðilse
            if (item != null && !item.isCollected)
            {
                carriedItems.Add(item);
                // Parent iþlemi
                item.AttachToPlayer(holdPosition);
            }
        }
    }

    private void DropAllItems()
    {
        // Listeyi boþalt ve itemlarý serbest býrak
        foreach (var item in carriedItems)
        {
            if (item != null) item.DetachFromPlayer();
        }
        carriedItems.Clear();
    }

    // KUTUYA GELDÝK
    public void DepositItemsInBox(ItemType boxType)
    {
        // Tersten dönüyoruz çünkü listeden eleman sileceðiz
        for (int i = carriedItems.Count - 1; i >= 0; i--)
        {
            MinigameItem item = carriedItems[i];

            if (item.type == boxType)
            {
                // DOÐRU KUTU
                Minigame1Controller.Instance.AddScore(item.type);
                item.OnDeposited(); // Yok et
                carriedItems.RemoveAt(i); // Listeden sil
            }
            else
            {
                // YANLIÞ KUTU
                Minigame1Controller.Instance.RespawnItemRandomly(item);
                carriedItems.RemoveAt(i); // Listeden sil (artýk bizde deðil, ýþýnlandý)
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}