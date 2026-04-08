using UnityEngine;
using UnityEngine.UI;

public class ToggleTargetObjectButton : MonoBehaviour
{
    [Header("Kéo 5 Button UI vào đây (index 0-4)")]
    [SerializeField] private Button[] uiButtons = new Button[5];

    [Header("Kéo 5 Object tương ứng vào đây (index 0-4)")]
    [SerializeField] private GameObject[] targetObjects = new GameObject[5];

    private void Awake()
    {
        BindButtons();
    }

    private void BindButtons()
    {
        int count = Mathf.Min(uiButtons.Length, targetObjects.Length);
        for (int i = 0; i < count; i++)
        {
            int index = i;
            if (uiButtons[index] == null) continue;

            uiButtons[index].onClick.RemoveAllListeners();
            uiButtons[index].onClick.AddListener(() => ToggleByIndex(index));
        }
    }

    public void ToggleByIndex(int index)
    {
        if (targetObjects == null || targetObjects.Length == 0)
        {
            Debug.LogWarning("ToggleTargetObjectButton: targetObjects chưa được gán.", this);
            return;
        }

        if (index < 0 || index >= targetObjects.Length)
        {
            Debug.LogWarning($"ToggleTargetObjectButton: index {index} không hợp lệ.", this);
            return;
        }

        GameObject target = targetObjects[index];
        if (target == null)
        {
            Debug.LogWarning($"ToggleTargetObjectButton: targetObjects[{index}] đang null.", this);
            return;
        }

        target.SetActive(!target.activeSelf);
    }
}
