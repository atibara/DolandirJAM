using System;
using TMPro;
using UnityEngine;

public class Question : MonoBehaviour
{
    [HideInInspector]
    public GameObject selectedAnswer;
    public Action<bool> onQuestionAnswered;

    public void SubmitAnswer()
    {
        if (selectedAnswer == null)
        {
            onQuestionAnswered?.Invoke(false);
        }
        else
        {
            var answer = selectedAnswer.GetComponent<Answer>();

            onQuestionAnswered?.Invoke(answer.isCorrect);
        }

        Destroy(gameObject);
    }
}
