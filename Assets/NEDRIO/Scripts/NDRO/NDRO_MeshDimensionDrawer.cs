using UnityEngine;
namespace NDRO.Ruler
{
    public class NDRO_MeshDimensionDrawer : MonoBehaviour
    {
        public Color lineColor = Color.red;
        public float lineWidth = 0.005f;
        public Material lineMaterial;
        public Material curveLineMaterial;
        public string planeType;


        //포물선
        public int resolution = 20; // 포물선을 구성하는 포인트의 수
        public int segmentCount = 20;
        float height = 0.15f;  // 포물선 높이 설정

        // 메시의 높이와 너비를 그리기
        public void DrawDimensions(GameObject meshObject)
        {
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;

            Bounds bounds = meshRenderer.bounds;
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;

            // 중심점 부터 그리기
            // switch (planeType)
            // {
            //     case "XY":
            //         DrawLine(meshObject.transform, center - new Vector3(size.x / 2, 0, 0), center + new Vector3(size.x / 2, 0, 0)); // X축 라인
            //         DrawLine(meshObject.transform, center - new Vector3(0, size.y / 2, 0), center + new Vector3(0, size.y / 2, 0)); // Y축 라인
            //         break;
            //     case "XZ":
            //         DrawLine(meshObject.transform, center - new Vector3(size.x / 2, 0, 0), center + new Vector3(size.x / 2, 0, 0)); // X축 라인
            //         DrawLine(meshObject.transform, center - new Vector3(0, 0, size.z / 2), center + new Vector3(0, 0, size.z / 2)); // Z축 라인
            //         break;
            //     case "YZ":
            //         DrawLine(meshObject.transform, center - new Vector3(0, size.y / 2, 0), center + new Vector3(0, size.y / 2, 0)); // Y축 라인
            //         DrawLine(meshObject.transform, center - new Vector3(0, 0, size.z / 2), center + new Vector3(0, 0, size.z / 2)); // Z축 라인
            //         break;
            // }



            // 왼쪽 하단부터 그리기
            Vector3 leftBottom = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            switch (planeType)
            {
                case "XY":
                    DrawLine(meshObject.transform, leftBottom, leftBottom + new Vector3(size.x, 0, 0)); // X축 라인
                    DrawLine(meshObject.transform, leftBottom, leftBottom - new Vector3(0, size.y, 0)); // Y축 라인
                    break;
                case "XZ":
                    DrawLine(meshObject.transform, leftBottom, leftBottom + new Vector3(size.x, 0, 0), "x"); // X축 라인
                    DrawLine(meshObject.transform, leftBottom, leftBottom - new Vector3(0, 0, size.z), "z"); // Z축 라인
                    break;
                case "YZ":
                    DrawLine(meshObject.transform, leftBottom, leftBottom + new Vector3(0, size.y, 0)); // Y축 라인
                    DrawLine(meshObject.transform, leftBottom, leftBottom - new Vector3(0, 0, size.z)); // Z축 라인
                    break;
            }

        }

        private void DrawLine(Transform parent, Vector3 start, Vector3 end, string direction = "")
        {
            GameObject lineObj = new GameObject("DimensionLine");
            lineObj.transform.SetParent(parent, false);
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // 포물선 그리기
            var center = CalculateCenterPosition(start, end);

            switch (planeType)
            {
                case "XY":
                    center = new Vector3(center.x, center.y, center.z + height);
                    break;
                case "XZ":
                    if (direction == "x")
                        center = new Vector3(center.x, center.y, center.z + height);
                    else
                        center = new Vector3(center.x - height, center.y, center.z);
                    break;
                case "YZ":
                    center = new Vector3(center.x + height, center.y, center.z);
                    break;
            }
            DrawCurve(parent, start, center, end);
        }


        // 포물선을 그리는 메서드



        // 베지어 곡선을 그리는 메서드
        public void DrawCurve(Transform parent, Vector3 startPosition, Vector3 centerPosition, Vector3 endPosition)
        {
            GameObject lineObj = new GameObject("curve line");
            lineObj.transform.SetParent(parent, false);
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = curveLineMaterial;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            Vector3[] curvePoints = new Vector3[segmentCount];

            for (int i = 0; i < segmentCount; i++)
            {
                float t = i / (float)(segmentCount - 1);
                curvePoints[i] = CalculateBezierPoint(t, startPosition, centerPosition, endPosition);
            }

            lineRenderer.positionCount = segmentCount;
            lineRenderer.SetPositions(curvePoints);
        }

        // 베지어 곡선의 특정 지점을 계산하는 메서드
        private Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
        {
            // 베지어 곡선 공식 적용
            return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * center + t * t * end;
        }

        public Vector3 CalculateCenterPosition(Vector3 startPosition, Vector3 endPosition)
        {
            // 두 포인트 사이의 중간점 계산
            Vector3 centerPosition = Vector3.Lerp(startPosition, endPosition, 0.5f);
            // xz 평면에서 y
            //return new Vector3(centerPosition.x - height, centerPosition.y, centerPosition.z);
            // xz 평면에서 x
            //return new Vector3(centerPosition.x, centerPosition.y, centerPosition.z + height);
            // 중심점
            return new Vector3(centerPosition.x, centerPosition.y, centerPosition.z);
        }
    }
}