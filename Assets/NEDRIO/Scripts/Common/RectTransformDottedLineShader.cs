using UnityEngine;
using UnityEngine.UI;

public class RectTransformDottedLineShader : MonoBehaviour
{
    public RectTransform startPoint;
    public RectTransform endPoint;
    public Image dottedLineImage;
    public Material dottedLineMaterial;

    void Update()
    {
        if (startPoint == null || endPoint == null || dottedLineImage == null)
            return;

        Vector2 start = startPoint.anchoredPosition;
        Vector2 end = endPoint.anchoredPosition;

        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        dottedLineImage.rectTransform.anchoredPosition = start + (end - start) / 2;
        dottedLineImage.rectTransform.sizeDelta = new Vector2(distance, dottedLineImage.rectTransform.sizeDelta.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dottedLineImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        // 쉐이더 속성 설정
        if (dottedLineMaterial != null)
        {
            dottedLineImage.material = dottedLineMaterial;
        }
    }
}
