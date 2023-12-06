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

        RaycastHit hit;
        public ARRaycastManager rayManager;
        public List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 scrCenterVec;
        public Transform trPivitObj;
        public Transform trPivotCenter;

        public TextMeshProUGUI txtUserDisance;
        public Transform trRulerPool;
        private float lastDistance = -1;


        public MeshRenderer mrPivitCenter;
        public MeshRenderer mrPivitEdge;
        public Button btnAddRulerPoint;

        Vector3 rulerPosSave;
        public NDRO_RulerPoints prefabRulerPoint;
        public NDRO_RulerPoints curRulerPoint;
        List<NDRO_RulerPoints> rulerPointPoolList = new List<NDRO_RulerPoints>();

        public GameObject mainCam;

        bool isSurfaceDetected = false;

        bool isFirstRulerPoint = false;


        public Image closePointUI;
        public Camera cam;



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

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
            {
                isFirstRulerPoint = hit.transform.tag == "firstRulerPoint";
                if (isFirstRulerPoint)
                {
                    //rulerPosSave = hit.point;

                    // world position to 2d position
                    Vector3 screenPos = cam.WorldToScreenPoint(rulerPointPoolList[0].pointA.position);
                    closePointUI.rectTransform.anchoredPosition = screenPos;

                    // UI TO 3D POSITION
                    //Vector3 worldPos = cam.ScreenToWorldPoint(closePointUI.rectTransform.anchoredPosition);
                    Vector3 screenPoint = closePointUI.rectTransform.position;
                    screenPoint.z = cam.nearClipPlane;

                    Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);
                    trPivotCenter.transform.position = worldPoint;

                }
                else
                {
                    trPivotCenter.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                trPivotCenter.transform.localPosition = Vector3.zero;
                isFirstRulerPoint = false;
            }


            isSurfaceDetected = rayManager.Raycast(scrCenterVec, hits, TrackableType.PlaneWithinPolygon);
            UpdateAlpha(isSurfaceDetected);
            if (isSurfaceDetected)
            {

                UpdateDistance();
                rulerPosSave = hits[0].pose.position;
            }
            else
            {
                txtUserDisance.text = "Please point at a surface";
            }
        }


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
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * 1.5f); // 부드러운 전환
            mrPivitCenter.material.SetFloat("_Alpha", newAlpha);
            mrPivitEdge.material.SetFloat("_Alpha", newAlpha);
        }



        void MakeRulerPoint()
        {
            if (isSurfaceDetected)
            {

                NDRO_RulerPoints tObj = Instantiate(prefabRulerPoint, trRulerPool);
                tObj.transform.position = Vector3.zero;
                tObj.transform.localScale = Vector3.one;

                if (curRulerPoint != null)
                {
                    rulerPosSave = curRulerPoint.pointB.position;
                }
                else
                {
                    tObj.pointA.tag = "firstRulerPoint";

                    Debug.Log("this!");
                }

                tObj.SetInits(rulerPosSave);
                tObj.SetMainCam(mainCam);
                rulerPointPoolList.Add(tObj);
                curRulerPoint = tObj;

            }
            else
            {
                curRulerPoint = null;
            }
        }


    }

}


