using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [Header("Danh sách các Hộp Thí nghiệm")]
    public GameObject[] experimentSetups; 

    void Start()
    {
        // Khi vừa vào game (vừa bấm Play), tự động dọn sạch mặt bàn
        HideAllExperiments();
    }

    // Hàm cất tất cả đồ đạc
    public void HideAllExperiments()
    {
        foreach (GameObject setup in experimentSetups)
        {
            if (setup != null) setup.SetActive(false);
        }
    }

    // Hàm gọi đồ đạc của từng bài ra
    public void LoadExperiment(int id)
    {
        HideAllExperiments(); // Dọn bàn cũ trước

        // Bày đồ mới ra
        if (id >= 0 && id < experimentSetups.Length)
        {
            if (experimentSetups[id] != null)
            {
                experimentSetups[id].SetActive(true);
            }
        }
    }
}