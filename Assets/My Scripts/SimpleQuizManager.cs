using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public TMP_Text txtTimer; // MỚI: Ô chứa Đồng hồ

    [Header("Cài đặt thời gian")]
    public float timePerQuestion = 15f; // MỚI: Mặc định 15 giây cho mỗi câu

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

    void Update() // MỚI: Hàm này chạy liên tục để đếm lùi thời gian
    {
        if (isTimerRunning && !isWaiting)
        {
            currentTime -= Time.deltaTime; // Trừ dần thời gian thực

            // Hiển thị ra màn hình chữ Txt_Timer
            txtTimer.text = "Thời gian: " + Mathf.CeilToInt(currentTime).ToString() + "s";

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
        }
        else
        {
            txtQuestion.text = "Chúc mừng! Bạn đã hoàn thành bài kiểm tra!";
            txtBtn1.transform.parent.gameObject.SetActive(false);
            txtBtn2.transform.parent.gameObject.SetActive(false);
            txtBtn3.transform.parent.gameObject.SetActive(false);
            txtFeedback.text = "";
            txtTimer.text = ""; // Giấu đồng hồ đi khi xong bài
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
        }
        else
        {
            txtFeedback.text = "Sai rồi!";
            txtFeedback.color = Color.red;
        }

        StartCoroutine(WaitAndNext());
    }

    void TimeUp() // MỚI: Xử lý khi hết thời gian
    {
        isTimerRunning = false;
        txtTimer.text = "Hết giờ!";
        txtTimer.color = Color.red; // Đổi màu chữ đồng hồ thành đỏ
        
        txtFeedback.text = "Quá thời gian!";
        txtFeedback.color = Color.red;
        
        StartCoroutine(WaitAndNext());
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