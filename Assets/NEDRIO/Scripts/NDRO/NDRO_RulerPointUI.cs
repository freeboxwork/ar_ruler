using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace NDRO.Ruler
{
    public class NDRO_RulerPointUI : MonoBehaviour
    {

        public RectTransform pointA;
        public RectTransform pointB;

        public Image imgPointA;
        public Image imgPointB;
        public Image imgLine;
        public Image imgBg;

        public RectTransform textBg;
        public TextMeshProUGUI textValue;


        public Sprite spriteCompleteLine;
        public Sprite spriteProgressLine;

        public Color colorCompleteLine;
        public Color colorProgressLine;

        void Start()
        {

        }


        void Update()
        {

        }

        public void SetPosition(Vector3 scrPointA, Vector3 scrPointB, Camera mainCamera = null)
        {
            pointA.anchoredPosition = scrPointA;
            pointB.anchoredPosition = scrPointB;

            Vector3 middlePoint = (scrPointA + scrPointB) / 2f;
            textBg.anchoredPosition = middlePoint;

            // pointA와 pointB 사이의 방향 벡터 계산
            Vector2 direction = scrPointB - scrPointA;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (mainCamera != null)
            {
                // 카메라 방향에 따라 각도의 부호 조정
                Vector2 cameraDirection = (textBg.position - Camera.main.transform.position).normalized;
                float dotProduct = Vector2.Dot(direction.normalized, cameraDirection);

                if (dotProduct < 0)
                {
                    angle += 180;
                }
            }
            // textBg의 z 회전 각도 설정
            textBg.localEulerAngles = new Vector3(0, 0, angle);

        }

        public void SetTextValue(string text)
        {
            textValue.text = text + "cm";
        }


        public void SetCompleteLine()
        {
            imgLine.sprite = spriteCompleteLine;
            imgLine.color = colorCompleteLine;
            imgPointA.color = colorCompleteLine;
            imgPointB.color = colorCompleteLine;
            imgBg.color = Color.white;
            textValue.color = Color.black;
            imgLine.rectTransform.sizeDelta = new Vector2(imgLine.rectTransform.sizeDelta.x, 10f);

        }

        public void SetProgressLine()
        {
            imgLine.sprite = spriteProgressLine;
            imgLine.color = colorProgressLine;
            imgPointA.color = colorProgressLine;
            imgPointB.color = colorProgressLine;
            imgBg.color = colorProgressLine;
            textValue.color = Color.white;
            imgLine.rectTransform.sizeDelta = new Vector2(imgLine.rectTransform.sizeDelta.x, 20f);
        }

    }
}
