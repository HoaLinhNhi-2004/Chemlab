using UnityEngine;
using TMPro;

[ExecuteAlways]
public class BendText : MonoBehaviour
{
    public TMP_Text textComponent;
    [Tooltip("Độ cong (Càng nhỏ càng cong, hãy chỉnh cho khớp với chai)")]
    public float bendRadius = 1.0f; 

    void Update()
    {
        if (textComponent == null) textComponent = GetComponent<TMP_Text>();
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = vertices[vertexIndex + j];
                // Tính toán góc cong dựa trên bán kính
                float angle = v.x / bendRadius;
                // Kéo lùi trục Z và bẻ cong trục X
                v.z -= bendRadius * (1 - Mathf.Cos(angle));
                v.x = bendRadius * Mathf.Sin(angle);
                vertices[vertexIndex + j] = v;
            }
        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}