using UnityEngine;

public class Answer : MonoBehaviour
{
    public bool isCorrect = false;
    private Question question;

    private void Start()
    {
        question = GetComponentInParent<Question>();
    }

    public void SelectQuestion()
    {
        question.selectedAnswer = gameObject;
    }
}
