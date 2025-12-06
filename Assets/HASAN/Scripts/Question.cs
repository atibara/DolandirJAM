using TMPro;
using UnityEngine;

public class Question : MonoBehaviour
{
    public GameObject selectedAnswer;
    public TMP_Text scoreText;

    public void SubmitAnswer()
    {
        if (selectedAnswer == null)
        {
            Debug.Log("no answer selected");
        }
        else
        {
            var answer = selectedAnswer.GetComponent<Answer>();

            if (answer.isCorrect)
            {
                Debug.Log("correct answer");
            }
            else
            {
                Debug.Log("wrong answer");
            }            
        }

        Destroy(gameObject);
    }
}
