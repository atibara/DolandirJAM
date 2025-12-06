using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Timer : MonoBehaviour
{
    private TMP_Text timerText;
    [SerializeField]
    private float countdown;
    private bool timerFinished = false;
    public event Action<float> OnTimerStart;
    public event Action OnTimerFinished;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
        OnTimerStart?.Invoke(countdown);
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
                OnTimerFinished?.Invoke();
                timerFinished = true;
            }
        }
    }
}
