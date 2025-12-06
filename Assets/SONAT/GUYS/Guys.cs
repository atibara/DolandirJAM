using UnityEngine;

public class Guys : MonoBehaviour
{

    private float nerdLOVE = 0;
    private float punkLOVE = 0;
    private float sportsLOVE = 0;

    public float NERDLOVE => nerdLOVE;
    public float PUNKLOVE => punkLOVE;
    public float SPORTSLOVE => sportsLOVE;


    public void addNerdLove(float love)
    {
        nerdLOVE += love;
    }

    public void addPunkLove(float love)
    {
        punkLOVE += love;
    }

    public void addSportsLove(float love)
    {
        sportsLOVE += love;
    }
}
