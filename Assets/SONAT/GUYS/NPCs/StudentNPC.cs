using UnityEngine;

public class StudentNPC : BaseNPC
{
    [Header("Student Settings")]
    [SerializeField] private Transform mySeat; // Kendi sýrasý

    protected override void ExecutePhaseBehavior(int phase)
    {
        switch (phase)
        {
            case 0: // KORÝDOR: Rastgele gez
                Wander();
                break;

            case 1: // SINAV: Sýraya git
                MoveTo(mySeat.position);
                break;

            case 2: // TUVALET FAZI: Sýnav bitti, yine gez
                Wander();
                break;
        }
    }
}