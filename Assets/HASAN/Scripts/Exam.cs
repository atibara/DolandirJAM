using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Exam : MonoBehaviour
{
    public List<GameObject> questions;
    private Queue<GameObject> questionsQueue;
    private GameObject currentQuestionInstance;

    public int scorePoint = 20;
    private int score = 0;
    public TMP_Text scoreText;

    void Start()
    {
        questionsQueue = new(questions);
    }

    void Update()
    {
        if (currentQuestionInstance == null && questionsQueue.Count != 0)
        {
            currentQuestionInstance = InstantiateQuestion();

            var question = currentQuestionInstance.GetComponent<Question>();
            question.onQuestionAnswered += UpdateScore;
        }
    }

    private void UpdateScore(bool correct)
    {
        if (correct)
        {
            score += 20;
        }

        scoreText.text = score.ToString() + "/100";
    }

    private GameObject InstantiateQuestion()
    {
        var questionInstance = questionsQueue.Dequeue();

        return Instantiate(questionInstance, transform);
    }
}
