using UnityEngine;
using UnityEngine.UI;

public class RectTransformConnection : MonoBehaviour
{
    public RectTransform rectA;
    public RectTransform rectB;
    public Image lineImage;

    void Update()
    {
        if (rectA == null || rectB == null || lineImage == null)
            return;

        Vector2 positionA = rectA.anchoredPosition;
        Vector2 positionB = rectB.anchoredPosition;

        Vector2 direction = (positionB - positionA).normalized;
        float distance = Vector2.Distance(positionA, positionB);

        lineImage.rectTransform.anchoredPosition = positionA + (positionB - positionA) / 2;
        lineImage.rectTransform.sizeDelta = new Vector2(distance, lineImage.rectTransform.sizeDelta.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
