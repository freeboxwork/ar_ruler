using UnityEngine;
using TMPro;
namespace NDRO.Ruler
{

    public class NDRO_RulerPoints : MonoBehaviour
    {

        public Transform pointA;
        public Transform pointB;
        public LineRenderer lineRenderer;

        public Transform textPosition;
        public TextMeshPro textValue;
        public Transform mainCamera;
        public float distance;


        void Start()
        {

        }
        void Update()
        {
            Vector3 distanceVector = pointB.position - pointA.position;

            // 텍스트 위치 설정
            textPosition.position = pointA.position + distanceVector * 0.5f;

            // 거리를 센티미터로 계산 및 텍스트 업데이트
            distance = distanceVector.magnitude * 100f; // 미터를 센티미터로 변환
            textValue.text = $"{distance:N2}cm"; // cm 단위로 표시

            // 카메라를 바라보도록 설정
            if (mainCamera != null)
                textPosition.LookAt(mainCamera);

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

        public void SetMainCam(GameObject cam)
        {
            mainCamera = cam.transform;
        }
    }

}

