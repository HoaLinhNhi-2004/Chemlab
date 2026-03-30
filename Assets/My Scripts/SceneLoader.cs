using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện để điều khiển các màn chơi

public class SceneLoader : MonoBehaviour
{
    // Hàm này sẽ được gọi khi bạn bấm nút trên bảng điều khiển
    public void LoadExperiment(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Hàm để thoát ứng dụng
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Đã thoát game!");
    }
}