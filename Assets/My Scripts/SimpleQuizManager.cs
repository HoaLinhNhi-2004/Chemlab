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
    public TMP_Text txtTimer; // Ô chứa Đồng hồ
    public Image imgTimerFill360; // Image dạng Filled (Radial 360)

    [Header("NPC Animation")]
    public Animator npcAnimator;       // Kéo thả NPC vào đây
    public string clapTriggerName = "Clap"; // Tên Trigger vỗ tay trong Animator

    [Header("Cài đặt thời gian")]
    public float timePerQuestion = 15f; // Mặc định 15 giây cho mỗi câu

    [Header("Danh sách câu hỏi")]
    public List<QuizQuestion> questions;
    private int currentIndex = 0;
    private bool isWaiting = false;

    // Biến cho đồng hồ
    private float currentTime; 
    private bool isTimerRunning = false;

    void Start()
    {
        txtFeedback.text = "";
        DisplayQuestion();
    }

    void Update() // Hàm này chạy liên tục để đếm lùi thời gian
    {
        if (isTimerRunning && !isWaiting)
        {
            currentTime -= Time.deltaTime; // Trừ dần thời gian thực
            UpdateTimerUI();

            // Nếu hết giờ
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

            // Reset và chạy lại đồng hồ
            currentTime = timePerQuestion;
            isTimerRunning = true;
            UpdateTimerUI(); // Bắt đầu câu mới: fill = 1
        }
        else
        {
            txtQuestion.text = "Chúc mừng! Bạn đã hoàn thành bài kiểm tra!";
            txtBtn1.transform.parent.gameObject.SetActive(false);
            txtBtn2.transform.parent.gameObject.SetActive(false);
            txtBtn3.transform.parent.gameObject.SetActive(false);
            txtFeedback.text = "";
            txtTimer.text = ""; // Giấu đồng hồ đi khi xong bài
            if (imgTimerFill360 != null) imgTimerFill360.fillAmount = 0f;
            isTimerRunning = false;
        }
    }

    public void ClickButton1() { CheckAnswer(0); }
    public void ClickButton2() { CheckAnswer(1); }
    public void ClickButton3() { CheckAnswer(2); }

    void CheckAnswer(int buttonIndex)
    {
        if (isWaiting || currentIndex >= questions.Count) return;

        isTimerRunning = false; // Dừng đồng hồ ngay khi người chơi bấm nút

        QuizQuestion q = questions[currentIndex];
        if (buttonIndex == q.correctAnswerIndex)
        {
            txtFeedback.text = "Chính xác!";
            txtFeedback.color = Color.green;

            // Gọi NPC vỗ tay khi trả lời đúng
            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger(clapTriggerName);
            }
        }
        else
        {
            txtFeedback.text = "Sai rồi!";
            txtFeedback.color = Color.red;
        }

        StartCoroutine(WaitAndNext());
    }

    void TimeUp() // Xử lý khi hết thời gian
    {
        isTimerRunning = false;
        txtTimer.text = "Hết giờ!";
        txtTimer.color = Color.red; // Đổi màu chữ đồng hồ thành đỏ
        if (imgTimerFill360 != null) imgTimerFill360.fillAmount = 0f;
        
        txtFeedback.text = "Quá thời gian!";
        txtFeedback.color = Color.red;
        
        StartCoroutine(WaitAndNext());
    }

    void UpdateTimerUI()
    {
        float safeTimePerQuestion = Mathf.Max(0.0001f, timePerQuestion);
        float normalizedTime = Mathf.Clamp01(currentTime / safeTimePerQuestion);

        // Hiển thị chữ đồng hồ
        if (txtTimer != null)
        {
            txtTimer.text = Mathf.CeilToInt(Mathf.Max(0f, currentTime)).ToString();
        }

        // Fill chạy từ 1 -> 0 theo thời gian
        if (imgTimerFill360 != null)
        {
            imgTimerFill360.fillAmount = normalizedTime;
        }
    }

    IEnumerator WaitAndNext()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        txtTimer.color = Color.white; // Trả lại màu trắng cho đồng hồ ở câu mới
        currentIndex++;
        DisplayQuestion();
        isWaiting = false;
    }
}