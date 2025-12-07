using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Question : MonoBehaviour
{
    private GameObject selectedAnswer;
    public event Action<bool> OnQuestionAnswered;

    [SerializeField]
    private GameObject sportsGuy;
    [SerializeField]
    private GameObject punkGuy;
    [SerializeField]
    private Timer timer;

    private void Start()
    {
        timer.OnTimerFinished += SubmitAnswer;
        timer.OnTimerStart += CheatAtRandom;
    }

    private void CheatAtRandom(float time)
    {
        var sportsCheatTime = Random.Range(-time, time);
        var punkCheatTime = Random.Range(-time, time);

        if (sportsCheatTime > 0)
        {
            StartCoroutine(CheatGuyInSeconds(sportsCheatTime, sportsGuy));
        }

        if (punkCheatTime > 0)
        {
            StartCoroutine(CheatGuyInSeconds(punkCheatTime, punkGuy));
        }
    }

    public void SelectAnswer(GameObject answer) => selectedAnswer = answer;

    private IEnumerator CheatGuyInSeconds(float seconds, GameObject guy)
    {
        yield return new WaitForSeconds(seconds);
        Instantiate(guy, transform);
    }

    private void SubmitAnswer()
    {
        if (selectedAnswer == null)
        {
            OnQuestionAnswered?.Invoke(false);
        }
        else
        {
            var answer = selectedAnswer.GetComponent<Answer>();

            OnQuestionAnswered?.Invoke(answer.isCorrect);
        }

        Destroy(gameObject);
    }
}
