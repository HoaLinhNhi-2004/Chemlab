using UnityEngine;

public class SmartTestTube : MonoBehaviour
{
    [Header("CÔNG THỨC PHẢN ỨNG")]
    [Tooltip("Tên của viên rắn hoặc giọt lỏng thứ 1")]
    public string ingredient1 = "Drop_BaCl2"; 
    [Tooltip("Tên của viên rắn hoặc giọt lỏng thứ 2")]
    public string ingredient2 = "Drop_Na2SO4";

    [Header("HIỆU ỨNG KẾT QUẢ")]
    [Tooltip("Vật thể nước trong ống nghiệm")]
    public Renderer liquidInside; 
    [Tooltip("Màu nước sẽ biến thành khi phản ứng (Ví dụ: Kết tủa trắng/xanh)")]
    public Material resultMaterial; 
    [Tooltip("Hiệu ứng phụ (Bọt khí, Khói...) - Để trống nếu không cần")]
    public GameObject extraEffect;

    // Bộ nhớ của ống nghiệm
    private bool hasIng1 = false;
    private bool hasIng2 = false;
    private bool isReacting = false;

    // Hàm này kích hoạt khi có một vật rơi xuyên qua miệng ống nghiệm
    void OnTriggerEnter(Collider other)
    {
        if (isReacting) return; // Đang phản ứng rồi thì không nhận thêm

        string itemRớtVào = other.gameObject.name;

        // Nếu vật rớt vào là Chất 1
        if (itemRớtVào.Contains(ingredient1) && !hasIng1)
        {
            hasIng1 = true;
            Destroy(other.gameObject); // Tiêu hủy giọt nước/viên rắn
            HienNuocTrongOngNghiem(); // Bật nước lên (nếu ống đang cạn)
        }
        // Nếu vật rớt vào là Chất 2
        else if (itemRớtVào.Contains(ingredient2) && !hasIng2)
        {
            hasIng2 = true;
            Destroy(other.gameObject);
            HienNuocTrongOngNghiem();
        }

        // KHI GOM ĐỦ 2 CHẤT -> PHẢN ỨNG BÙM!
        if (hasIng1 && hasIng2)
        {
            TriggerReaction();
        }
    }

    void HienNuocTrongOngNghiem()
    {
        if (liquidInside != null && !liquidInside.gameObject.activeSelf)
        {
            liquidInside.gameObject.SetActive(true); // Hiển thị mức nước
        }
    }

    void TriggerReaction()
    {
        isReacting = true;

        // 1. Đổi màu nước (Tạo kết tủa)
        if (liquidInside != null && resultMaterial != null)
        {
            liquidInside.material = resultMaterial;
        }

        // 2. Bật sủi bọt/khói (nếu có)
        if (extraEffect != null)
        {
            extraEffect.SetActive(true);
        }
        
        Debug.Log("Phản ứng thành công!");
    }
}