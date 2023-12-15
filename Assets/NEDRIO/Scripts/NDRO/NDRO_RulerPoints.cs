using UnityEngine;
using System.Collections.Generic;
using TMPro;
namespace NDRO.Ruler
{
    public class NDRO_RulerPoints : MonoBehaviour
    {
        private readonly Dictionary<Vector3, Vector3> rotationMap = new Dictionary<Vector3, Vector3>
        {
            { new Vector3(0f,1f,0f), new Vector3(0, 0, -90) },
            { new Vector3(0f,-1f,0f), new Vector3(0, 0, 90) },
            { new Vector3(-1f,0f,0f), new Vector3(90, 0, 0) },
            { new Vector3(1f,0f,0f), new Vector3(-90, 0, 0) },
            { new Vector3(0f,0f,1f), new Vector3(0, -90, 0) },
            { new Vector3(0f,0f,-1f), new Vector3(0, 90, 0) }
        };

        public enum Direction
        {
            Up,      // 상 (0, 1, 0) : (0,0,-90)
            Down,    // 하 (0, -1, 0) : (0,0,90)
            Left,    // 좌 (-1, 0, 0) : (0,180,0) 
            Right,   // 우 (1, 0, 0)  : (0,0,0)
            Forward, // 전 (0, 0, 1) : (0,-90,0)
            Back     // 후 (0, 0, -1) : (0,90,0); 
        }
        public Transform pointA;
        public Transform pointB;
        public MeshRenderer mrPointA;
        public MeshRenderer mrPointB;
        public LineRenderer lineRenderer;
        public SpriteRenderer rdTxtBg;

        public Transform txtBox;
        public TextMeshPro textValue;
        public Camera mainCamera;
        public float distance;

        public NDRO_RulerPointUI rulerPointUI;

        public Transform txtSet;

        NDRO_RulerManager rulerManager;



        //public Pose hitPose;

        // SCALE RANGE
        DistanceScaleRange scaleRange_point = new DistanceScaleRange(10f, 300f, 0.01f, 0.02f);
        DistanceScaleRange scaleRange_line = new DistanceScaleRange(10f, 300f, 0.002f, 0.009f);
        DistanceScaleRange scaleRange_textBox = new DistanceScaleRange(10f, 300f, 0.2f, 2f);

        bool isComplete = false;



        void Start()
        {

        }

        float GetCamDisance()
        {
            var camPos = mainCamera.transform.position;
            var dis = Vector3.Distance(camPos, transform.position);
            return dis * 100f;
        }

        void Update()
        {
            // 사용자 거리에 따른 스케일 조정
            var dis = GetCamDisance();
            pointA.localScale = NdroUtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_point);
            pointB.localScale = NdroUtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_point);
            lineRenderer.startWidth = NdroUtilityMethod.AdjustValueBasedOnDistance(dis, scaleRange_line);
            txtBox.localScale = NdroUtilityMethod.AdjustScaleBasedOnDistance(dis, scaleRange_textBox);



            if (isComplete)
            {
                return;
            }



            Vector3 distanceVector = pointB.position - pointA.position;

            // 텍스트 위치 설정
            txtBox.position = pointA.position + distanceVector * 0.5f;

            // 거리를 센티미터로 계산 및 텍스트 업데이트
            distance = distanceVector.magnitude * 100f; // 미터를 센티미터로 변환
            var disText = distance.ToString("N0") + "cm";
            textValue.text = disText; // cm 단위로 표시

            var posA = pointA.localPosition;
            var posB = pointB.localPosition;
            var ax = posA.x;
            var az = posA.z;
            var bx = posB.x;
            var bz = posB.z;
            var angle = Mathf.Atan2(bz - az, bx - ax) * Mathf.Rad2Deg;

            //var ang = Vector3.SignedAngle(pointA.position, pointB.position, pointA.forward);
            //Debug.Log(angle);


            // Vector3 direction = pointB.position - pointA.position;
            // if (mainCamera != null)
            // {
            //     // 카메라 방향에 따라 각도의 부호 조정
            //     Vector3 cameraDirection = (txtSet.position - mainCamera.transform.position).normalized;
            //     float dotProduct = Vector3.Dot(direction.normalized, cameraDirection);

            //     if (dotProduct < 0)
            //     {
            //         angle += 180;
            //     }
            // }

            //Debug.Log("angle : " + angle);

            switch (rulerManager.planeDirType)
            {
                case NDRO_EnumDefinition.PlaneDirType.horizontal:
                    if (angle >= -90 && angle <= 90)
                    {
                        angle += 180;
                    }
                    break;
                case NDRO_EnumDefinition.PlaneDirType.vertical:
                    if (angle < 0)
                    {
                        angle += 180;
                    }
                    break;
            }

            // if (rulerManager.planeDirType == NDRO_EnumDefinition.PlaneDirType.horizontal)) // horizontal
            // {
            //     //angle += 180;
            //     //Debug.Log("floor!");
            //     if (angle >= -90 && angle <= 90)
            //     {
            //         angle += 180;
            //     }
            // }

            // else // vertical
            // {
            //     if (angle < 0)
            //     {
            //         angle += 180;
            //     }
            // }




            txtSet.localRotation = Quaternion.Euler(0, 0, angle);

            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Vector3 direction = (pointB.position - pointA.position).normalized;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // txtSet.localRotation = Quaternion.Euler(0, 0, angle);


            // //설정
            // txtBox.localEulerAngles = new Vector3(0, 0, angle);

            // var dist = Vector3.Distance(pointA.position, pointB.position);
            // var dir = (pointB.position - pointA.position);
            // var normal = hitPlaneUpSide;
            // var upd = Vector3.Cross(dir, normal);
            // var rot = Quaternion.LookRotation(-normal, upd);



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


        public void SetInits(Vector3 position, Pose hitPose, NDRO_RulerManager rulerManager, Camera cam)
        {
            transform.position = position;
            pointA.position = position;
            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, position);
            this.rulerManager = rulerManager;
            mainCamera = cam;

            if (hitPose != null)
            {
                gameObject.transform.rotation = hitPose.rotation;
            }
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
            isComplete = true;

            lineRenderer.material.SetColor("_LineColor", Color.white);
            lineRenderer.material.SetFloat("_DashLength", 0f);
            mrPointA.material.SetColor("_Color", Color.white);
            mrPointB.material.SetColor("_Color", Color.white);
            rdTxtBg.color = Color.white;

            //rulerPointUI.SetCompleteLine();
        }

        public void UnComplet()
        {
            isComplete = false;

            lineRenderer.material.SetColor("_LineColor", Color.yellow);
            lineRenderer.material.SetFloat("_DashLength", 0.05f);
            mrPointA.material.SetColor("_Color", Color.yellow);
            mrPointB.material.SetColor("_Color", Color.yellow);
            rdTxtBg.color = new Color32(255, 190, 0, 255);

            // rulerPointUI.SetProgressLine();
        }


        public Direction GetClosestDirection(Vector3 directionVector)
        {
            directionVector.Normalize(); // 방향 벡터 정규화

            float maxDot = -Mathf.Infinity;
            Direction closestDirection = Direction.Up;

            foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                Vector3 dirVector = GetDirectionVector(dir);
                float dot = Vector3.Dot(dirVector, directionVector);

                if (dot > maxDot)
                {
                    maxDot = dot;
                    closestDirection = dir;
                }
            }

            return closestDirection;
        }

        private Vector3 GetDirectionVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Vector3.up;
                case Direction.Down: return Vector3.down;
                case Direction.Left: return new Vector3(90, 0, 0); //Vector3.left;
                case Direction.Right: return Vector3.right;
                case Direction.Forward: return Vector3.forward;
                case Direction.Back: return Vector3.back;
                default: return Vector3.zero;
            }
        }



        public Vector3 GetDirVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Vector3.up;
                case Direction.Down: return Vector3.down;
                case Direction.Left: return Vector3.left;
                case Direction.Right: return Vector3.right;
                case Direction.Forward: return Vector3.forward;
                case Direction.Back: return Vector3.back;
                default: return Vector3.zero;
            }
        }


    }

}

