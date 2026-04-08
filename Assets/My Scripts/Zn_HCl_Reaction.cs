using UnityEngine;

public class Zn_HCl_Reaction : MonoBehaviour
{
    [Header("Hiệu ứng Bọt khí (Kéo Particle System vào đây)")]
    public GameObject bubblesEffect; // Tương đương với cái 'fume' ở code cũ

    [Header("Thời gian sủi bọt (giây)")]
    public float reactionTime = 5f;
    
    private float timer = 0f;
    private bool isReacting = false;

    void Start()
    {
        // Vừa vào game, đảm bảo bọt khí đang tắt
        if (bubblesEffect != null)
        {
            bubblesEffect.SetActive(false);
        }
    }

    // Hàm này tự động kích hoạt khi có một vật thể rơi chạm vào bình Axit
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem vật rơi vào có phải là viên Kẽm không
        // (Trong thư mục của bạn, các viên kẽm đang tên là "Cube")
        if (collision.gameObject.name.Contains("Cube") && !isReacting)
        {
            StartReaction();
            
            // Hiệu ứng "tan biến": Tiêu hủy viên kẽm sau 0.5 giây
            Destroy(collision.gameObject, 0.5f);
        }
    }

    void StartReaction()
    {
        isReacting = true;
        timer = 0f;
        
        // Bật sủi bọt khí H2
        if (bubblesEffect != null)
        {
            bubblesEffect.SetActive(true);
        }
        
        Debug.Log("Phản ứng Kẽm và HCl đang diễn ra!");
    }

    void Update()
    {
        // Bộ đếm thời gian để tự động tắt bọt khí
        if (isReacting)
        {
            timer += Time.deltaTime;
            if (timer >= reactionTime)
            {
                isReacting = false;
                if (bubblesEffect != null)
                {
                    bubblesEffect.SetActive(false); // Dừng sủi bọt
                }
                Debug.Log("Phản ứng kết thúc!");
            }
        }
    }
}