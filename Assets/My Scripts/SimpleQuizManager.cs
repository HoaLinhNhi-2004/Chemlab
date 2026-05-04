using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public string[] answers = new string[3];
    public int correctAnswerIndex;
}

public class SimpleQuizManager : MonoBehaviour
{
    [Header("Kéo thả UI vào đây")]
    public TMP_Text txtQuestion;
    public TMP_Text txtBtn1;
    public TMP_Text txtBtn2;
    public TMP_Text txtBtn3;
    public TMP_Text txtFeedback;
    public TMP_Text txtTimer;
    public Image imgTimerFill360;

    [Header("UI Kết quả")]
    public GameObject resultPanel;
    public TMP_Text txtFinalScore;
    public TMP_Text txtRating;

    [Header("NPC Animation")]
    public Animator npcAnimator;
    public string clapTriggerName = "Clap";

    [Header("Cài đặt thời gian")]
    public float timePerQuestion = 15f;

    [Header("Danh sách câu hỏi")]
    public List<QuizQuestion> questions;

    private int currentIndex = 0;
    private bool isWaiting = false;
    private float currentTime;
    private bool isTimerRunning = false;

    // Thêm mới: biến tính điểm
    private int score = 0;

    void Start()
    {
        score = 0;
        txtFeedback.text = "";
        if (resultPanel != null) resultPanel.SetActive(false);
        DisplayQuestion();
    }

    void Update()
    {
        if (isTimerRunning && !isWaiting)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                TimeUp();
            }
        }
    }

    void DisplayQuestion()
    {
        if (currentIndex < questions.Count)
        {
            QuizQuestion q = questions[currentIndex];
            txtQuestion.text = q.questionText;
            txtBtn1.text = q.answers[0];
            txtBtn2.text = q.answers[1];
            txtBtn3.text = q.answers[2];
            txtFeedback.text = "";

            currentTime = timePerQuestion;
            isTimerRunning = true;
            UpdateTimerUI();
        }
        else
        {
            // Hết câu hỏi → giống code gốc, thêm ShowResult
            txtBtn1.transform.parent.gameObject.SetActive(false);
            txtBtn2.transform.parent.gameObject.SetActive(false);
            txtBtn3.transform.parent.gameObject.SetActive(false);
            txtFeedback.text = "";
            txtTimer.text = "";
            if (imgTimerFill360 != null) imgTimerFill360.fillAmount = 0f;
            isTimerRunning = false;

            ShowResult();
        }
    }

    public void ClickButton1() { CheckAnswer(0); }
    public void ClickButton2() { CheckAnswer(1); }
    public void ClickButton3() { CheckAnswer(2); }

    void CheckAnswer(int buttonIndex)
    {
        if (isWaiting || currentIndex >= questions.Count) return;

        isTimerRunning = false;

        QuizQuestion q = questions[currentIndex];
        if (buttonIndex == q.correctAnswerIndex)
        {
            score++; // Thêm mới: cộng điểm
            txtFeedback.text = "Chính xác!";
            txtFeedback.color = Color.green;

            if (npcAnimator != null)
                npcAnimator.SetTrigger(clapTriggerName);
        }
        else
        {
            txtFeedback.text = "Sai rồi!";
            txtFeedback.color = Color.red;
        }

        StartCoroutine(WaitAndNext());
    }

    void TimeUp()
    {
        isTimerRunning = false;
        txtTimer.text = "Hết giờ!";
        txtTimer.color = Color.red;
        if (imgTimerFill360 != null) imgTimerFill360.fillAmount = 0f;

        txtFeedback.text = "Quá thời gian!";
        txtFeedback.color = Color.red;

        StartCoroutine(WaitAndNext());
    }

    void UpdateTimerUI()
    {
        float safeTimePerQuestion = Mathf.Max(0.0001f, timePerQuestion);
        float normalizedTime = Mathf.Clamp01(currentTime / safeTimePerQuestion);

        if (txtTimer != null)
            txtTimer.text = Mathf.CeilToInt(Mathf.Max(0f, currentTime)).ToString();

        if (imgTimerFill360 != null)
            imgTimerFill360.fillAmount = normalizedTime;
    }

    // Thêm mới: hiện kết quả
    void ShowResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            if (txtFinalScore != null)
                txtFinalScore.text = score + "/" + questions.Count + " câu đúng";

            if (txtRating != null)
            {
                float ratio = (float)score / questions.Count;
                if (ratio == 1f)
                    txtRating.text = "Xuất sắc! Bạn trả lời đúng tất cả!";
                else if (ratio >= 0.6f)
                    txtRating.text = "Tốt! Hãy ôn lại các câu sai nhé.";
                else
                    txtRating.text = "Cần cố gắng thêm! Ôn lại lý thuyết nhé.";
            }
        }
    }

    // Thêm mới: làm lại bài
    public void ReplayQuiz()
    {
        score = 0;
        currentIndex = 0;

        txtBtn1.transform.parent.gameObject.SetActive(true);
        txtBtn2.transform.parent.gameObject.SetActive(true);
        txtBtn3.transform.parent.gameObject.SetActive(true);
        txtTimer.color = Color.white;
        txtFeedback.color = Color.white;

        if (resultPanel != null) resultPanel.SetActive(false);

        DisplayQuestion();
    }

    IEnumerator WaitAndNext()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        txtTimer.color = Color.white;
        currentIndex++;
        DisplayQuestion();
        isWaiting = false;
    }
}