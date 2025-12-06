using UnityEngine;

public class GirlNPC : BaseNPC
{
    [Header("Girl Settings")]
    [SerializeField] private Transform targetToFollow; // Sporcuyu buraya sürükle
    [SerializeField] private Transform mySeat; // Kendi sýrasý
    [SerializeField] private Transform restroomPoint; // Tuvalet noktasý

    [SerializeField] private float followDistance = 1.5f; // Takip mesafesi

    protected override void ExecutePhaseBehavior(int phase)
    {
        switch (phase)
        {
            case 0: // KORÝDOR: Sporcuyu takip et
                HandleFollowLogic();
                break;

            case 1: // SINAV: Sýraya git
                MoveTo(mySeat.position);
                break;

            case 2: // TUVALET: Tuvalete git
                MoveTo(restroomPoint.position);
                break;
        }
    }

    private void HandleFollowLogic()
    {
        if (targetToFollow == null) return;

        float dist = Vector3.Distance(transform.position, targetToFollow.position);
        if (dist > followDistance)
        {
            MoveTo(targetToFollow.position);
        }
        else
        {
            Stop(); // Çok yaklaþtý, bekle
        }
    }
}