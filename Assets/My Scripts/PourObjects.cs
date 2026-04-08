using UnityEngine;

public class PourObjects : MonoBehaviour
{
    [Header("Góc nghiêng để đổ (Độ)")]
    public float pourAngle = 90f; 
    
    [Header("Thời gian giữa 2 lần rớt hạt (Giây)")]
    public float pourDelay = 0.3f; 
    
    private float timer = 0f;

    void Update()
    {
        // Tính toán độ nghiêng của cái lọ so với mặt đất
        float angle = Vector3.Angle(Vector3.up, transform.up);

        // Nếu người chơi dốc ngược lọ
        if (angle > pourAngle)
        {
            timer += Time.deltaTime;
            if (timer >= pourDelay) // Rớt từng hạt một
            {
                PourOneItem();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f; // Dựng thẳng bình lên thì ngưng đổ
        }
    }

    void PourOneItem()
    {
        // Quét tìm xem trong lọ còn hạt kẽm (Cube) nào không
        Transform childToPour = null;
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Cube")) // Tên hạt kẽm của bạn
            {
                childToPour = child;
                break; // Chỉ lấy 1 viên
            }
        }

        // Nếu tìm thấy hạt kẽm
        if (childToPour != null)
        {
            // 1. Tách hạt kẽm ra khỏi lọ (hết bị dính keo)
            childToPour.SetParent(null);
            
            // 2. Bơm vật lý (Trọng lực) vào để nó rơi tự do
            Rigidbody rb = childToPour.gameObject.GetComponent<Rigidbody>();
            if (rb == null) rb = childToPour.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            // 3. Đảm bảo nó có khung va chạm để chạm vào miệng bình Axit
            Collider col = childToPour.gameObject.GetComponent<Collider>();
            if (col == null) childToPour.gameObject.AddComponent<BoxCollider>();
        }
    }
}