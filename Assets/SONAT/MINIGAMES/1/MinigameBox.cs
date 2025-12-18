using UnityEngine;

public class MinigameBox : MonoBehaviour
{
    public ItemType acceptedType; // NerdItem mý, PlayerItem mý?

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Karakter bize çarptý mý?
        MinigamePlayer player = other.GetComponent<MinigamePlayer>();

        if (player != null)
        {
            // Karakterin elindekileri kontrol etmesi için çaðýrýyoruz
            player.DepositItemsInBox(acceptedType);
        }
    }
}