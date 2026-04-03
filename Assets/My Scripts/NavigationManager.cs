using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [Header("Bảng Menu & Nút Thoát")]
    public GameObject mainMenu;    // Bảng Menu chọn bài
    public GameObject exitButton;  // Nút Exit

    [Header("Các phần học (Bật/Tắt)")]
    public GameObject theorySection;   
    public GameObject practiceSection; 
    public GameObject quizSection;     

    void Start()
    {
        // Khi mới vào game: Bật Menu, Tắt mọi thứ khác
        ReturnToMainMenu(); 
    }

    // --- HÀM CHO NÚT EXIT (QUAY LẠI MENU) ---
    public void ReturnToMainMenu()
    {
        // Tắt hết các phần học
        if (theorySection != null) theorySection.SetActive(false);
        if (practiceSection != null) practiceSection.SetActive(false);
        if (quizSection != null) quizSection.SetActive(false);

        // Tắt luôn nút Exit và Bật lại bảng Menu
        if (exitButton != null) exitButton.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    // --- CÁC HÀM DÀNH CHO NÚT CHỌN BÀI TRÊN MENU ---
    public void ShowTheory()
    {
        OpenSection(theorySection);
    }

    public void ShowPractice()
    {
        OpenSection(practiceSection);
    }

    public void ShowQuiz()
    {
        OpenSection(quizSection);
    }

    // Hàm tiện ích: Ẩn Menu, Bật Nút Exit và Bật Phần học được chọn
    private void OpenSection(GameObject sectionToOpen)
    {
        // 1. Tắt bảng Menu
        if (mainMenu != null) mainMenu.SetActive(false);
        
        // 2. Bật nút Exit lên
        if (exitButton != null) exitButton.SetActive(true);

        // 3. Tắt hết các bài học cũ (đề phòng) và bật bài học mới
        if (theorySection != null) theorySection.SetActive(false);
        if (practiceSection != null) practiceSection.SetActive(false);
        if (quizSection != null) quizSection.SetActive(false);
        
        if (sectionToOpen != null) sectionToOpen.SetActive(true);
    }
}