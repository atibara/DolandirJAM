using Unity.VisualScripting;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public bool isCorrect = false;
    private Question question;

    private void Start()
    {
        question = GetComponentInParent<Question>();
    }

    public void AnswerQuestion()
    {
        question.SubmitAnswer(isCorrect);
    }
}
