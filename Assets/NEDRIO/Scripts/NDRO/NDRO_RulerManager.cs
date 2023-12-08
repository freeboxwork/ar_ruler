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

        [Header("Ruler Points Management")]
        public NDRO_RulerPoints curRulerPoint;
        private List<NDRO_RulerPoints> rulerPointPoolList = new List<NDRO_RulerPoints>();

        // State Variables
        private RaycastHit hit;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private Vector3 rulerPosSave;
        private float lastDistance = -1;
        private Vector2 scrCenterVec;

        // Flags
        private bool isSurfaceDetected = false;
        private bool isFirstRulerPoint = false;




        void Start()
        {
            scrCenterVec = new Vector2(Screen.width / 2, Screen.height / 2);
            SetBtnEvent();
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
        }


        void RaycastFromCamera()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                isFirstRulerPoint = hit.transform.CompareTag("firstRulerPoint");
                // path close
                HandleFirstRulerPoint();
            }
            else
            {
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
            trPivotCenter.transform.position = Vector3.Lerp(trPivotCenter.transform.position, worldPoint, Time.deltaTime * 20f);
        }

        void ResetPivotCenterPosition()
        {
            // Smoothly interpolate back to the starting position
            trPivotCenter.transform.localPosition = Vector3.Lerp(trPivotCenter.transform.localPosition, Vector3.zero, Time.deltaTime * 15f);
            isFirstRulerPoint = false;
        }




        void HandleSurfaceDetection()
        {
            isSurfaceDetected = rayManager.Raycast(scrCenterVec, hits, TrackableType.PlaneWithinPolygon);
            UpdateAlpha(isSurfaceDetected);

            if (!isSurfaceDetected)
            {
                txtUserDisance.text = "Please point at a surface";
                return;
            }

            UpdateDistance();
            rulerPosSave = hits[0].pose.position;
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
                txtUserDisance.text = closestDistance.ToString("N2") + " cm";

                if (curRulerPoint != null)
                {
                    if (isFirstRulerPoint)
                    {
                        rulerPosSave = rulerPointPoolList[0].pointA.position;
                    }
                    curRulerPoint.SetObj(rulerPosSave);
                }


                // 크기 조정 로직 추가
                AdjustScaleBasedOnDistance(closestDistance);
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
                    polygonMeshCreator.InitMeshCreater(rulerPointPoolList);

                    // mesh 정보 출력
                    var vectors = NDRO_PolygonPlaneCalculator.GetVectorsByNDRO_RulerPoints(rulerPointPoolList);
                    var info = NDRO_PolygonPlaneCalculator.CalculateDimensions(vectors);
                    Debug.Log($"width: {info.width}, height: {info.height}, plane: {info.plane}");

                    rulerPointPoolList.Clear();

                    // 측정 완료
                    return;
                }

                // complete
                if (curRulerPoint != null)
                    curRulerPoint.Complet();

                var rulerPointUI = Instantiate(prefabRulerPointUI, trRulerPointUIPool);

                NDRO_RulerPoints tObj = Instantiate(prefabRulerPoint, trRulerPool);
                tObj.transform.position = Vector3.zero;
                tObj.transform.localScale = Vector3.one;
                tObj.rulerPointUI = rulerPointUI;

                if (curRulerPoint != null)
                {
                    rulerPosSave = curRulerPoint.pointB.position;
                }
                else
                {
                    tObj.pointA.tag = "firstRulerPoint";
                }

                tObj.SetInits(rulerPosSave);
                tObj.SetMainCam(cam);
                rulerPointPoolList.Add(tObj);




                curRulerPoint = tObj;

            }
            // else
            // {
            //     curRulerPoint = null;
            // }
        }


    }

}


