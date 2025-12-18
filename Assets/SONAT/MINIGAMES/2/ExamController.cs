using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ExamQuestion
{
    public Sprite questionSprite;
    public Sprite[] answerSprites; // Þýk resimleri
    public int correctIndex; // 0=A, 1=B, 2=C, 3=D
}

public class ExamController : MonoBehaviour
{
    [Header("Sýnav UI")]
    [SerializeField] private Image questionImageDisplay;
    [SerializeField] private Button[] mainAnswerButtons;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI gradeText;

    [Header("Kopya UI")]
    [SerializeField] private GameObject cheatPopupSports;
    [SerializeField] private Button[] sportsCheatButtons;
    [SerializeField] private GameObject cheatPopupPunk;
    [SerializeField] private Button[] punkCheatButtons;

    [Header("Ayarlar")]
    [SerializeField] private float timePerQuestion = 10f;
    [SerializeField] private List<ExamQuestion> questions;

    // Durum Deðiþkenleri
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isExamActive = false;
    private int correctAnswersCount = 0;
    private bool isQuestionProcessing = false;

    // Kopya Durumu
    private bool isCheatActive = false;
    private int currentCheatRequester = -1;

    // YENÝ: Sporcuya bu sýnavda yardým ettik mi?
    private bool hasHelpedSports = false;

    private void OnEnable()
    {
        if (questions == null || questions.Count == 0) return;
        StartExam();
    }

    private void StartExam()
    {
        currentQuestionIndex = 0;
        correctAnswersCount = 0;
        isExamActive = true;
        isQuestionProcessing = false;

        hasHelpedSports = false; // Her sýnavda sýfýrla

        cheatPopupSports.SetActive(false);
        cheatPopupPunk.SetActive(false);
        if (gradeText) gradeText.gameObject.SetActive(false);

        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndExam();
            return;
        }

        timer = timePerQuestion;
        isQuestionProcessing = false;

        ExamQuestion q = questions[currentQuestionIndex];
        questionImageDisplay.sprite = q.questionSprite;

        for (int i = 0; i < mainAnswerButtons.Length; i++)
        {
            int index = i;
            mainAnswerButtons[i].onClick.RemoveAllListeners();
            mainAnswerButtons[i].onClick.AddListener(() => OnMainAnswerSelected(index));

            if (q.answerSprites != null && q.answerSprites.Length > i)
            {
                mainAnswerButtons[i].GetComponent<Image>().sprite = q.answerSprites[i];
            }
        }

        if (Random.value > 0.6f)
        {
            Invoke("TriggerCheatEvent", 2f);
        }
    }

    private void Update()
    {
        if (!isExamActive || isQuestionProcessing) return;

        timer -= Time.deltaTime;
        if (timerSlider) timerSlider.value = timer / timePerQuestion;

        if (timer <= 0)
        {
            OnMainAnswerSelected(-1);
        }
    }

    public void OnMainAnswerSelected(int index)
    {
        if (!isExamActive || isQuestionProcessing) return;

        isQuestionProcessing = true;

        CloseActiveCheatPopup();
        CancelInvoke("TriggerCheatEvent");

        if (index != -1 && index == questions[currentQuestionIndex].correctIndex)
        {
            correctAnswersCount++;
        }

        currentQuestionIndex++;
        LoadQuestion();
    }

    private void TriggerCheatEvent()
    {
        if (!isExamActive || isQuestionProcessing) return;

        isCheatActive = true;
        currentCheatRequester = Random.Range(0, 2);

        if (currentCheatRequester == 0)
        {
            cheatPopupSports.SetActive(true);
            SetupCheatButtons(sportsCheatButtons);
        }
        else
        {
            cheatPopupPunk.SetActive(true);
            SetupCheatButtons(punkCheatButtons);
        }
    }

    private void SetupCheatButtons(Button[] buttonsToSetup)
    {
        for (int i = 0; i < buttonsToSetup.Length; i++)
        {
            int answerIdx = i;
            buttonsToSetup[i].onClick.RemoveAllListeners();
            buttonsToSetup[i].onClick.AddListener(() => GiveCheatAnswer(answerIdx));
        }
    }

    public void GiveCheatAnswer(int givenAnswerIndex)
    {
        int realCorrect = questions[currentQuestionIndex].correctIndex;
        bool isCorrectAdvice = (givenAnswerIndex == realCorrect);
        var stats = GameScenarioManager.Instance.StatsManager;

        if (currentCheatRequester == 0) // Sporcu
        {
            if (isCorrectAdvice)
            {
                stats.addSportsLove(10);
                // KRÝTÝK NOKTA: Sadece doðru kopya verirsen bu bayraðý kaldýrýyoruz
                hasHelpedSports = true;
            }
            else stats.addSportsLove(-5);
        }
        else // Serseri
        {
            if (isCorrectAdvice) stats.addPunkLove(10);
            else stats.addPunkLove(-2);
        }

        CloseActiveCheatPopup();
    }

    private void CloseActiveCheatPopup()
    {
        cheatPopupSports.SetActive(false);
        cheatPopupPunk.SetActive(false);
        isCheatActive = false;
    }

    private void EndExam()
    {
        isExamActive = false;

        string grade = CalculateGrade();
        if (gradeText != null)
        {
            gradeText.text = grade;
            gradeText.gameObject.SetActive(true);
        }

        int nerdScore = Random.Range(5, 7);
        // DÜZELTME: Nerd'ü geçersen puaný daha sert düþürüyoruz (-20) ki kesin sinirlensin.
        if (correctAnswersCount >= nerdScore) GameScenarioManager.Instance.StatsManager.addNerdLove(-20);
        else GameScenarioManager.Instance.StatsManager.addNerdLove(5);

        Invoke("FinishMinigame", 3f);
    }

    private string CalculateGrade()
    {
        switch (correctAnswersCount)
        {
            case 6: return "A+";
            case 5: return "A";
            case 4: return "B";
            case 3: return "C";
            case 2: return "D";
            case 1: return "F";
            default: return "F-";
        }
    }

    private void FinishMinigame()
    {
        gameObject.SetActive(false);
        // DÜZELTME: Manager'a "Sporcuya yardým ettik mi?" bilgisini de gönderiyoruz.
        GameScenarioManager.Instance.OnMinigame2Finished_Logic(hasHelpedSports);
    }
}