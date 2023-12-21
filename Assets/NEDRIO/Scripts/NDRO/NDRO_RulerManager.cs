using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

namespace NDRO.Ruler
{
    public class NDRO_RulerManager : MonoBehaviour
    {

        public ARRaycastManager rayManager;
        public Camera cam;

        [Header("UI Components")]
        public TextMeshProUGUI txtUserDisance;
        public Image closePointUI;
        public Button btnAddRulerPoint;

        [Header("Mesh Renderers")]
        public MeshRenderer mrPivitCenter;
        public MeshRenderer mrPivitEdge;

        [Header("Transforms")]
        public Transform trPivitObj;
        public Transform trPivotCenter;
        public Transform trRulerPool;
        public Transform trRulerPointUIPool;


        [Header("Prefab References")]
        public NDRO_RulerPoints prefabRulerPoint;
        public NDRO_RulerPointUI prefabRulerPointUI;

        [Header("Data Managers")]
        public NDRO_ARDataManager arDataManager;
        public NDRO_PolygonMeshCreator polygonMeshCreator;

        //[Header("Ruler Points Management")]
        private NDRO_RulerPoints curRulerPoint;
        private List<NDRO_RulerPoints> rulerPointPoolList = new List<NDRO_RulerPoints>();

        // State Variables
        private RaycastHit hit;
        public Transform curHitTr;

        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Vector3 rulerPosSave;
        private float lastDistance = -1;
        private Vector2 scrCenterVec;

        // Flags
        private bool isSurfaceDetected = false;
        private bool isFirstRulerPoint = false;
        private bool isFirstDetectionPlane = false;

        Pose hitPose;
        Vector3 hitUpSide;
        public NDRO_EnumDefinition.PlaneDirType planeDirType;

        [Header("Ruler UI Controller")]
        public NDRO_UIController uiController;

        public Material lineMaterial;
        public Material curveLineMaterial;

        public ResultText resultTextPrefab;

        public NDRO_MeshDimensionDrawer meshDimensionDrawerPrefab;

        DistanceScaleRange pivotScaleRange = new DistanceScaleRange(10f, 300f, 1.0f, 0.3f);

        void Start()
        {
            Init();
        }

        void Init()
        {
            scrCenterVec = new Vector2(Screen.width / 2, Screen.height / 2);
            SetBtnEvent();
            uiController.EnablePlaneDetectionAnimUI(true);
        }

        void SetBtnEvent()
        {
            btnAddRulerPoint.onClick.AddListener(MakeRulerPoint);
        }

        // Update is called once per frame
        void Update()
        {
            RaycastFromCamera();
            HandleSurfaceDetection();
            EnablePlaneDetactionAnim();
            EnableDisatanceText();
        }




        void RaycastFromCamera()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                isFirstRulerPoint = hit.transform.CompareTag("firstRulerPoint");
                curHitTr = hit.transform;
                // path close
                HandleFirstRulerPoint();
            }
            else
            {
                curHitTr = null;
                ResetPivotCenterPosition();
            }
        }

        void HandleFirstRulerPoint()
        {
            if (!isFirstRulerPoint)
            {
                ResetPivotCenterPosition();
                return;
            }

            // World position to 2D position
            Vector3 screenPos = cam.WorldToScreenPoint(rulerPointPoolList[0].pointA.position);
            closePointUI.rectTransform.anchoredPosition = screenPos;

            // UI to 3D position
            Vector3 screenPoint = closePointUI.rectTransform.position;
            screenPoint.z = cam.nearClipPlane + 0.13f; // Adjust depth as needed
            Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);

            // Smoothly interpolate the trPivotCenter position to the worldPoint
            trPivotCenter.transform.position = Vector3.Lerp(trPivotCenter.transform.position, worldPoint, Time.deltaTime * 10f);
        }

        void ResetPivotCenterPosition()
        {
            // Smoothly interpolate back to the starting position
            trPivotCenter.transform.localPosition = Vector3.Lerp(trPivotCenter.transform.localPosition, Vector3.zero, Time.deltaTime * 15f);
            isFirstRulerPoint = false;
        }

        bool dtext = false;
        void EnableDisatanceText()
        {
            uiController.EnableUISetDistanceText(isSurfaceDetected);
        }



        void HandleSurfaceDetection()
        {
            isSurfaceDetected = rayManager.Raycast(scrCenterVec, hits, TrackableType.PlaneWithinPolygon);
            UpdateAlpha(isSurfaceDetected);

            if (!isSurfaceDetected)
            {
                //txtUserDisance.text = "Please point at a surface";
                return;
            }

            UpdateDistance();
            rulerPosSave = hits[0].pose.position;
            hitPose = hits[0].pose;
        }

        void EnablePlaneDetactionAnim()
        {
            if (isFirstDetectionPlane) return;

            if (IsFirstDeteactionPlane())
            {
                isFirstDetectionPlane = true;
                uiController.EnablePlaneDetectionAnimUI(false);
            }
        }

        bool IsFirstDeteactionPlane()
        {
            return hits.Count > 0;
        }



        // 유저가 표면을 찾았을 때 거리 측정 및 업데이트
        void UpdateDistance()
        {
            float closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                float currentDistance = Vector3.Distance(hit.pose.position, trPivitObj.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trPivitObj.rotation = Quaternion.Lerp(trPivitObj.rotation, hit.pose.rotation, 0.05f);
                }
            }

            if (closestDistance != lastDistance)
            {
                lastDistance = closestDistance;
                closestDistance = ConvertToCentimeters(closestDistance);
                uiController.SetDistanceText(closestDistance);

                if (curRulerPoint != null)
                {
                    if (isFirstRulerPoint)
                    {
                        rulerPosSave = rulerPointPoolList[0].pointA.position;
                    }
                    curRulerPoint.SetObj(rulerPosSave);
                }

                //거리에 따른 크기 조정 로직 추가
                //AdjustScaleBasedOnDistance(closestDistance, trPivitObj, pivotScaleRange);
                trPivitObj.localScale = NdroUtilityMethod.AdjustScaleBasedOnDistance(closestDistance, pivotScaleRange);
            }
        }



        float ConvertToCentimeters(float meters)
        {
            return meters * 100f;
        }



        void AdjustScaleBasedOnDistance(float distance)
        {
            // 거리(cm)를 10cm ~ 300cm 사이의 값으로 제한
            distance = Mathf.Clamp(distance, 10f, 300f);

            // 거리에 따른 스케일 계산 (10cm에서는 1.0, 300cm에서는 0.3)
            float scale = Mathf.Lerp(1.0f, 0.3f, (distance - 10f) / (300f - 10f));

            // 스케일 적용
            trPivitObj.localScale = new Vector3(scale, scale, scale);
        }

        //trPivitObj
        void AdjustScaleBasedOnDistance(float distance, Transform transform, DistanceScaleRange range)
        {
            // 거리를 minDistance ~ maxDistance 사이의 값으로 제한
            distance = Mathf.Clamp(distance, range.minDistance, range.maxDistance);

            // 거리에 따른 스케일 계산 (minDistance에서는 minScale, maxDistance에서는 maxScale)
            float scale = Mathf.Lerp(range.minScale, range.maxScale, (distance - range.minDistance) / (range.maxDistance - range.minDistance));

            // 스케일 적용
            transform.localScale = new Vector3(scale, scale, scale);
        }

        void UpdateAlpha(bool isSurfaceDetected)
        {
            var mat = mrPivitCenter.material;
            float targetAlpha = isSurfaceDetected ? 1.0f : 0.0f; // 찾았을 때 불투명, 못 찾았을 때 투명
            float currentAlpha = mat.GetFloat("_Alpha");
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 10f); // 부드러운 전환
            mrPivitCenter.material.SetFloat("_Alpha", newAlpha);
            mrPivitEdge.material.SetFloat("_Alpha", newAlpha);
        }



        void MakeRulerPoint()
        {
            if (isSurfaceDetected)
            {

                var curHitObjTr = curHitTr;

                if (isFirstRulerPoint)
                {

                    if (rulerPointPoolList.Count < 3)
                    {
                        Debug.Log("최소 2개 이상의 선분이 필요합니다.");
                        return;
                    }

                    curRulerPoint.Complet();

                    curRulerPoint = null;

                    // json model 저장 
                    rulerPointPoolList[0].pointA.tag = "Untagged";
                    arDataManager.SaveTapeRulerData(rulerPointPoolList, "test_customer", "test_customer_code");

                    // mesh 생성
                    polygonMeshCreator.InitMeshCreater(rulerPointPoolList, out GameObject meshObject);

                    // mesh 정보 출력
                    var vectors = NDRO_PolygonPlaneCalculator.GetVectorsByNDRO_RulerPoints(rulerPointPoolList);
                    var info = NDRO_PolygonPlaneCalculator.CalculateDimensions(vectors);
                    ResultInfo resultInfo = new ResultInfo(info.width, info.height, info.plane);

                    Debug.Log($"width: {info.width}, height: {info.height}, plane: {info.plane}");

                    // info 
                    NDRO_MeshDimensionDrawer drawer = Instantiate(meshDimensionDrawerPrefab, meshObject.transform); // meshObject.AddComponent<NDRO_MeshDimensionDrawer>();
                    // drawer.lineMaterial = lineMaterial;
                    // drawer.curveLineMaterial = curveLineMaterial;
                    drawer.planeType = info.plane;
                    drawer.DrawDimensions(meshObject, resultTextPrefab, resultInfo);

                    rulerPointPoolList.Clear();
                    planeDirType = NDRO_EnumDefinition.PlaneDirType.none;
                    // 측정 완료

                    return;
                }

                var dri = curHitObjTr.GetComponent<ARPlane>().classification;
                Debug.Log("dri: " + dri);

                if (curRulerPoint != null && planeDirType != GetDetectPlaneType(curHitObjTr))
                {
                    Debug.Log("평면이 다릅니다.");
                    return;
                }

                // 2개의 라인이 연결됨. complete
                if (curRulerPoint != null)
                    curRulerPoint.Complet();

                //var rulerPointUI = Instantiate(prefabRulerPointUI, trRulerPointUIPool);

                NDRO_RulerPoints tObj = Instantiate(prefabRulerPoint, trRulerPool, curHitTr);
                tObj.transform.position = Vector3.zero;
                tObj.transform.localScale = Vector3.one;

                //tObj.rulerPointUI = rulerPointUI;

                if (curRulerPoint != null)
                {
                    rulerPosSave = curRulerPoint.pointB.position;
                }
                else
                {
                    // set tag first point
                    tObj.pointA.tag = "firstRulerPoint";

                    // set plane first dir type
                    planeDirType = GetDetectPlaneType(curHitObjTr);

                    Debug.Log("planeDirType: " + planeDirType);

                }

                tObj.SetInits(rulerPosSave, hitPose, this, cam);
                tObj.SetMainCam(cam);
                rulerPointPoolList.Add(tObj);
                curRulerPoint = tObj;

            }
        }

        NDRO_EnumDefinition.PlaneDirType GetDetectPlaneType(Transform transform)
        {
            return IsRotationHorizontal(transform) ? NDRO_EnumDefinition.PlaneDirType.horizontal : NDRO_EnumDefinition.PlaneDirType.vertical;
        }

        bool IsRotationHorizontal(Transform transform)
        {
            Vector3 forward = transform.forward; // 전방향 벡터
            Vector3 up = transform.up; // 위쪽 방향 벡터

            // forward 벡터가 수평면 내에 있는지 확인
            bool isForwardHorizontal = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) < 0.1f;

            // up 벡터가 수직으로 향하고 있는지 확인
            bool isUpVertical = Vector3.Dot(up, Vector3.up) > 0.9f;

            return isForwardHorizontal && isUpVertical;
        }


    }

    /// <summary>
    /// 거리와 스케일의 최소값과 최대값을 정의하는 구조체입니다.
    /// </summary>
    public struct DistanceScaleRange
    {
        /// <summary>
        /// 거리의 최소값 (cm 단위).
        /// </summary>
        public float minDistance;

        /// <summary>
        /// 거리의 최대값 (cm 단위).
        /// </summary>
        public float maxDistance;

        /// <summary>
        /// 최소 거리에서의 스케일 값.
        /// </summary>
        public float minScale;

        /// <summary>
        /// 최대 거리에서의 스케일 값.
        /// </summary>
        public float maxScale;

        public DistanceScaleRange(float minDistance, float maxDistance, float minScale, float maxScale)
        {
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.minScale = minScale;
            this.maxScale = maxScale;
        }
    }

    public struct ResultInfo
    {
        public float width;
        public float height;
        public string plane;

        public ResultInfo(float width, float height, string plane)
        {
            this.width = width;
            this.height = height;
            this.plane = plane;
        }
    }

}


