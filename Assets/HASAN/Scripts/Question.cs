using UnityEngine;

public class Question : MonoBehaviour
{
    public void SubmitAnswer(bool correct)
    {
        if (correct)
        {
            Debug.Log("Correct answer.");
        }
        else
        {
            Debug.Log("Wrong answer.");
        }

        Destroy(gameObject);
    }
}
