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
        public List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 scrCenterVec;
        public Transform trPivitObj;
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


