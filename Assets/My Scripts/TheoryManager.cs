using UnityEngine;
using TMPro;

public class TheoryManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI txtContent;
    public TextMeshProUGUI txtTitle; // nếu có

    [Header("Nội dung lý thuyết")]
    [TextArea(3, 6)]
    public string[] contents; // điền nội dung từng trang trong Inspector

    private int currentIndex = 0;

    void Start()
    {
        ShowContent(0);
    }

    public void NextPage()
    {
        if (currentIndex < contents.Length - 1)
        {
            currentIndex++;
            ShowContent(currentIndex);
        }
        else
        {
            // Hết nội dung → làm gì đó (thoát, mở quiz,...)
            Debug.Log("Đã hết nội dung");
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowContent(currentIndex);
        }
    }

    void ShowContent(int index)
    {
        txtContent.text = contents[index];
    }
}