using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Timer : MonoBehaviour
{
    private TMP_Text timerText;
    public float countdown = 11f;
    private bool timerFinished = false;
    private Question question;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
        question = GetComponentInParent<Question>();
    }

    private void Update()
    {
        if (!timerFinished)
        {
            countdown -= Time.deltaTime;

            float n;
            if (countdown - 1 <= 0)
            {
                n = 0f;
            }
            else
            {
                n = countdown - 1;
            }

            timerText.text = Mathf.Ceil(n).ToString();

            if (countdown < 0)
            {
                question.SubmitAnswer();
                timerFinished = true;
            }
        }
    }
}
