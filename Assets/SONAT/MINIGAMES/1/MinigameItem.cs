using UnityEngine;
using UnityEngine.UI;

public class MinigameItem : MonoBehaviour
{
    public ItemType type;
    [HideInInspector] public bool isCollected = false;

    private Image myImage;
    private BoxCollider2D myCollider;
    private Transform originalParent;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        myCollider = GetComponent<BoxCollider2D>();
        originalParent = transform.parent;
    }

    public void AttachToPlayer(Transform holder)
    {
        isCollected = true;
        // Çarpýþmayý kapatalým ki karakter kutuya girerken itemlar kutuya çarpýp bizi itmesin
        myCollider.enabled = false;

        // CRITICAL: 'true' parametresi itemin o anki konumunu korumasýný saðlar, merkeze çekmez.
        transform.SetParent(holder, true);
    }

    public void DetachFromPlayer()
    {
        isCollected = false;
        myCollider.enabled = true; // Yere düþünce tekrar çarpýþabilsin

        // Eski container'a geri dön ama pozisyonunu koru
        transform.SetParent(originalParent, true);
    }

    public void ResetState()
    {
        isCollected = false;
        myCollider.enabled = true;
        transform.SetParent(originalParent, false); // Bu false kalsýn çünkü Iþýnlanma kodu pozisyonu elle veriyor
    }

    public void OnDeposited()
    {
        Destroy(gameObject);
    }
}
public enum ItemType { NerdItem, PlayerItem }