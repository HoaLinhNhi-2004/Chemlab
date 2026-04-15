using UnityEngine;

public class PourObjects : MonoBehaviour
{
    [Header("Tên hạt muốn đổ ra (VD: Cube, Drop)")]
    public string dropName = "Cube"; // Mặc định là Cube để không làm hỏng bài Kẽm cũ
    
    [Header("Góc nghiêng để đổ (Độ)")]
    public float pourAngle = 90f; 
    
    [Header("Thời gian giữa 2 lần rớt hạt (Giây)")]
    public float pourDelay = 0.3f; 
    
    private float timer = 0f;

    void Update()
    {
        // Tính toán độ nghiêng
        float angle = Vector3.Angle(Vector3.up, transform.up);

        if (angle > pourAngle)
        {
            timer += Time.deltaTime;
            if (timer >= pourDelay)
            {
                PourOneItem();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f; 
        }
    }

    void PourOneItem()
    {
        Transform childToPour = null;
        foreach (Transform child in transform)
        {
            // Đã thay đổi: Nó sẽ tìm tên dựa theo ô dropName bạn nhập trên Unity
            if (child.name.Contains(dropName)) 
            {
                childToPour = child;
                break;
            }
        }

        if (childToPour != null)
        {
            childToPour.SetParent(null);
            
            Rigidbody rb = childToPour.gameObject.GetComponent<Rigidbody>();
            if (rb == null) rb = childToPour.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            Collider col = childToPour.gameObject.GetComponent<Collider>();
            if (col == null) childToPour.gameObject.AddComponent<BoxCollider>();
        }
    }
}