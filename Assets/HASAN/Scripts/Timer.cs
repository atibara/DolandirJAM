using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Timer : MonoBehaviour
{
    private TMP_Text timerText;
    public float countdown = 10f;
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
            timerText.text = Mathf.Ceil(countdown).ToString();

            if (countdown < 0)
            {
                question.SubmitAnswer(false);
                timerFinished = true;
            }
        }
    }
}
