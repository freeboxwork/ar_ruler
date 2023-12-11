using UnityEngine;
using UnityEngine.UI;

public class DotUIScaleAnimation : MonoBehaviour
{
    public Image dot;
    public Image pivot;

    float maxScale = 1.0f;
    float minScale = 0f;
    float maxDistance = 200f; // 이 값은 조정할 수 있습니다. 거리에 따른 스케일 변화를 조절하기 위한 값입니다.

    void Update()
    {
        // pivot과 dot 사이의 거리 계산
        float distance = Vector2.Distance(dot.rectTransform.anchoredPosition, pivot.rectTransform.anchoredPosition);

        // 거리에 따른 스케일 계산
        float scale = Mathf.Clamp(1 - (distance / maxDistance), minScale, maxScale);

        // dot의 스케일 업데이트
        dot.rectTransform.localScale = new Vector3(scale, scale, scale);
    }
}
