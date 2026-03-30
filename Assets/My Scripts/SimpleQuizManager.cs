using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Thư viện dùng cho TextMeshPro

// Khai báo cấu trúc của 1 câu hỏi
[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public string[] answers = new string[3]; // Luôn có 3 đáp án
    public int correctAnswerIndex; // Số 0 (Đáp án 1), Số 1 (Đáp án 2), Số 2 (Đáp án 3)
}

public class SimpleQuizManager : MonoBehaviour
{
    [Header("Kéo thả UI vào đây")]
    public TMP_Text txtQuestion;
    public TMP_Text txtBtn1;
    public TMP_Text txtBtn2;
    public TMP_Text txtBtn3;
    public TMP_Text txtFeedback;

    [Header("Danh sách câu hỏi (Thêm ở đây)")]
    public List<QuizQuestion> questions;
    private int currentIndex = 0;
    private bool isWaiting = false; // Chặn bấm liên tục khi đang chuyển câu

    void Start()
    {
        txtFeedback.text = "";
        DisplayQuestion();
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
        }
        else
        {
            txtQuestion.text = "Chúc mừng! Bạn đã hoàn thành bài kiểm tra Chương 1!";
            txtBtn1.transform.parent.gameObject.SetActive(false); // Ẩn nút 1
            txtBtn2.transform.parent.gameObject.SetActive(false); // Ẩn nút 2
            txtBtn3.transform.parent.gameObject.SetActive(false); // Ẩn nút 3
            txtFeedback.text = "";
        }
    }

    // Các hàm dành cho 3 nút bấm
    public void ClickButton1() { CheckAnswer(0); }
    public void ClickButton2() { CheckAnswer(1); }
    public void ClickButton3() { CheckAnswer(2); }

    void CheckAnswer(int buttonIndex)
    {
        if (isWaiting || currentIndex >= questions.Count) return;

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

    // Đợi 2 giây rồi tự chuyển câu hỏi tiếp theo
    IEnumerator WaitAndNext()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        currentIndex++;
        DisplayQuestion();
        isWaiting = false;
    }
}