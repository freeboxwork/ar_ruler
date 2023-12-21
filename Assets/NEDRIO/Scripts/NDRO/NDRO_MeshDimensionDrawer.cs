using UnityEngine;
namespace NDRO.Ruler
{
    public class NDRO_MeshDimensionDrawer : MonoBehaviour
    {
        public Color lineColor = Color.red;
        public float lineWidth = 0.005f;
        public Material lineMaterial;
        public Material curveLineMaterial;
        public Material infoMeshMaterial;
        public string planeType;


        //포물선
        public int segmentCount = 20;// 포물선을 구성하는 포인트의 수
        float height = 0.15f;  // 포물선 높이 설정


        ResultText resultTextPrefab;
        ResultInfo resultInfo;
        Transform parent;

        public LineRenderer lrWidth;
        public LineRenderer lrHeight;
        public LineRenderer lrWidthCurve;
        public LineRenderer lrHeightCurve;

        public MeshFilter infoMeshFilter;
        public ResultText infoTextWidth;
        public ResultText infoTextHeight;
        public ResultText infoTextPlane;

        public Transform point_0;
        public Transform point_1;
        public Transform point_2;
        public Transform point_3;
        public Transform point_4;

        public NDRO_RulerManager rulerManager;


        // 메시의 높이와 너비를 그리기
        public void DrawDimensions(GameObject meshObject, ResultText resultTextPrefab, ResultInfo resultInfo)
        {
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;

            parent = Instantiate(new GameObject("DimensionLineParent"), transform.transform).transform;

            this.resultTextPrefab = resultTextPrefab;
            this.resultInfo = resultInfo;

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
                    DrawLine(point_0, point_1, lrWidth, lrWidthCurve, leftBottom, leftBottom + new Vector3(size.x, 0, 0), "x"); // X축 라인
                    DrawLine(point_2, point_3, lrHeight, lrHeightCurve, leftBottom, leftBottom + new Vector3(0, size.y, 0), "y"); // Y축 라인
                    break;
                case "XZ":
                    DrawLine(point_0, point_1, lrWidth, lrWidthCurve, leftBottom, leftBottom + new Vector3(size.x, 0, 0), "x"); // X축 라인
                    DrawLine(point_2, point_3, lrHeight, lrHeightCurve, leftBottom, leftBottom - new Vector3(0, 0, size.z), "z"); // Z축 라인
                    break;
                case "YZ":
                    DrawLine(point_0, point_1, lrWidth, lrWidthCurve, leftBottom, leftBottom + new Vector3(0, size.y, 0), "y"); // Y축 라인
                    DrawLine(point_2, point_3, lrHeight, lrHeightCurve, leftBottom, leftBottom - new Vector3(0, 0, size.z), "z"); // Z축 라인
                    break;
            }

            //draw info mesh
            DrawInfoMesh();

        }

        void DrawInfoMesh()
        {
            var pointA = point_0.position;
            var pointB = point_1.position;
            var pointC = point_3.position;
            var pointD = pointA - pointB + pointC;
            point_4.position = pointD;
        }

        private void DrawLine(Transform pointA, Transform pointB, LineRenderer lineRenderer, LineRenderer lineCurve, Vector3 start, Vector3 end, string direction = "")
        {
            // GameObject lineObj = new GameObject("DimensionLine");
            // lineObj.transform.SetParent(parent, false);
            // LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

            pointA.position = start;
            pointB.position = end;

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
                    if (direction == "x")
                    {
                        center = new Vector3(center.x, center.y, center.z + height);
                        InitInfoText(infoTextWidth, center, resultInfo.width.ToString("0.00"), new Vector3(0, -180, 0));
                    }
                    else
                    {
                        center = new Vector3(center.x - height, center.y, center.z);
                        InitInfoText(infoTextHeight, center, resultInfo.height.ToString("0.00"), new Vector3(180, 0, 90));
                    }

                    break;
                case "XZ":
                    if (direction == "x")
                    {
                        center = new Vector3(center.x, center.y, center.z + height);
                        InitInfoText(infoTextWidth, center, resultInfo.width.ToString("0.00"), new Vector3(90, 0, 0));
                    }

                    else
                    {
                        center = new Vector3(center.x - height, center.y, center.z);
                        InitInfoText(infoTextHeight, center, resultInfo.height.ToString("0.00"), new Vector3(90, 0, -90));
                    }

                    break;
                case "YZ":
                    if (direction == "y")
                    {
                        center = new Vector3(center.x, center.y, center.z + height);
                        InitInfoText(infoTextWidth, center, resultInfo.width.ToString("0.00"), new Vector3(0, 90, 90));
                    }
                    else
                    {
                        center = new Vector3(center.x, center.y - height, center.z);
                        InitInfoText(infoTextHeight, center, resultInfo.height.ToString("0.00"), new Vector3(0, 90, 0));
                    }
                    break;
            }
            DrawCurve(lineCurve, start, center, end);
        }

        void InitInfoText(ResultText resultText, Vector3 pos, string value, Vector3 rotValue)
        {
            //var resultText = Instantiate(resultTextPrefab, parent);
            resultText.transform.position = pos;
            resultText.transform.eulerAngles = rotValue;
            resultText.SetTxtValue(value);
        }

        // 베지어 곡선을 그리는 메서드
        public void DrawCurve(LineRenderer lineRenderer, Vector3 startPosition, Vector3 centerPosition, Vector3 endPosition)
        {
            // GameObject lineObj = new GameObject("curve line");
            // lineObj.transform.SetParent(parent, false);
            // LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
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

        // text mesh 생성


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