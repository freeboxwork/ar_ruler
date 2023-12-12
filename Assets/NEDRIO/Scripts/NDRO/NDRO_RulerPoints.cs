using UnityEngine;
using TMPro;
namespace NDRO.Ruler
{
    public class NDRO_RulerPoints : MonoBehaviour
    {

        public Transform pointA;
        public Transform pointB;
        public LineRenderer lineRenderer;

        public Transform txtBox;
        public TextMeshPro textValue;
        public Camera mainCamera;
        public float distance;

        public NDRO_RulerPointUI rulerPointUI;


        void Start()
        {

        }
        void Update()
        {
            Vector3 distanceVector = pointB.position - pointA.position;

            // 텍스트 위치 설정
            txtBox.position = pointA.position + distanceVector * 0.5f;

            // 거리를 센티미터로 계산 및 텍스트 업데이트
            distance = distanceVector.magnitude * 100f; // 미터를 센티미터로 변환
            var disText = distance.ToString("N0") + "cm";
            textValue.text = disText; // cm 단위로 표시

            Vector3 direction = pointB.position - pointA.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            if (mainCamera != null)
            {
                // 카메라 방향에 따라 각도의 부호 조정
                Vector2 cameraDirection = (txtBox.position - mainCamera.transform.position).normalized;
                float dotProduct = Vector3.Dot(direction.normalized, cameraDirection);

                if (dotProduct < 0)
                {
                    angle += 180;
                }
            }
            // txtBoxdml z 회전 각도 설정
            txtBox.localEulerAngles = new Vector3(0, 0, angle);

            // 카메라를 바라보도록 설정
            // if (mainCamera != null)
            //     textPosition.LookAt(mainCamera.transform);

            // set ui position
            if (rulerPointUI != null)
            {

                Vector3 viewPointA = mainCamera.WorldToViewportPoint(pointA.position);
                Vector3 viewPointB = mainCamera.WorldToViewportPoint(pointB.position);

                // 카메라가 오브젝트를 정면에서 바라보고 있을 때만 UI 위치 업데이트
                if (viewPointA.z > 0 && viewPointB.z > 0)
                {
                    var scrPointA = GetScreenPosition(pointA);
                    var scrPointB = GetScreenPosition(pointB);
                    // rulerPointUI.SetPosition(scrPointA, scrPointB, mainCamera);
                    // rulerPointUI.SetTextValue(disText);

                }
                else
                {
                    // UI 요소를 화면 밖으로 이동하여 숨깁니다.
                    //  rulerPointUI.SetPosition(new Vector3(-1000, -1000, 0), new Vector3(-1000, -1000, 0));
                }
            }
        }

        // get world position to screen position
        public Vector3 GetScreenPosition(Transform target)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);
            // 화면 밖으로 표시되지 않도록 Z 좌표 조정
            if (screenPosition.z < 0)
            {
                screenPosition = new Vector3(-1000, -1000, 0);
            }
            return screenPosition;
        }


        public void SetInits(Vector3 position)
        {
            transform.position = position;
            pointA.position = position;
            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, position);
        }

        public void SetObj(Vector3 position)
        {
            pointB.position = position;
            lineRenderer.SetPosition(1, position);
        }

        public void SetMainCam(Camera cam)
        {
            mainCamera = cam;
        }

        public void Complet()
        {
            //rulerPointUI.SetCompleteLine();
        }

        public void UnComplet()
        {
            // rulerPointUI.SetProgressLine();
        }
    }

}

