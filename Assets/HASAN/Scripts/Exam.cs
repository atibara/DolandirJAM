using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Exam : MonoBehaviour
{
    public List<GameObject> questions;
    private Queue<GameObject> questionsQueue;
    private GameObject currentQuestion;

    void Start()
    {
        questionsQueue = new(questions);
    }

    void Update()
    {
        if (currentQuestion == null && questionsQueue.Count != 0)
        {
            currentQuestion = InstantiateQuestion();
        }
    }

    private GameObject InstantiateQuestion()
    {
        return Instantiate(questionsQueue.Dequeue(), transform);
    }
}
