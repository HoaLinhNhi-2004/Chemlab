using UnityEngine;

public class Mg_Combustion : MonoBehaviour
{
    [Header("Tên viên Magie rớt vào")]
    public string targetName = "Drop_Mg";

    [Header("GameObject sẽ được bật khi va chạm")]
    public GameObject objectToActivate;

    [Header("Hiệu ứng ánh sáng (Kéo Point Light vào đây)")]
    public Light flashLight; 

    [Header("Độ sáng chói lóa (Chỉnh tùy thích)")]
    public float maxIntensity = 500f; // Đã tăng mức mặc định lên cực chói

    [Header("Bán kính chiếu sáng (Range)")]
    public float lightRange = 10f; // Đảm bảo ánh sáng tỏa ra đủ xa

    private float burnTimer = 0f;
    private bool isBurning = false;

    void Start()
    {
        if (flashLight != null) 
        {
            flashLight.intensity = 0f;
            flashLight.range = lightRange; // Cập nhật bán kính
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(targetName) && !isBurning)
        {
            isBurning = true;
            Destroy(other.gameObject); 

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            if (flashLight != null) flashLight.intensity = maxIntensity;
            
            Debug.Log("Magie đang cháy khét lẹt!");
        }
    }

    void Update()
    {
        if (isBurning)
        {
            burnTimer += Time.deltaTime;
            
            if (flashLight != null)
            {
                flashLight.intensity = Mathf.Lerp(maxIntensity, 0f, burnTimer / 2f);
            }

            if (burnTimer >= 2.5f)
            {
                isBurning = false;
                burnTimer = 0f;
            }
        }
    }
}