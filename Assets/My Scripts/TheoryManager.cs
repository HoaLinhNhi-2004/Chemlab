using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TheoryPage
{
    public string title;
    [TextArea(5, 10)] 
    public string content;
}

public class TheoryManager : MonoBehaviour
{
    public TMP_Text txtTitle;
    public TMP_Text txtContent;
    public List<TheoryPage> pages;

    private int currentIndex = 0;

    void Start()
    {
        UpdateUI();
    }

    public void NextPage()
    {
        if (currentIndex < pages.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (pages.Count > 0)
        {
            txtTitle.text = pages[currentIndex].title;
            txtContent.text = pages[currentIndex].content;
        }
    }
}