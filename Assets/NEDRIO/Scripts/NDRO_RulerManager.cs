using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;


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

        void Start()
        {
            scrCenterVec = new Vector2(Screen.width / 2, Screen.height / 2);
        }


        // Update is called once per frame
        void Update()
        {

            if (rayManager.Raycast(scrCenterVec, hits, TrackableType.PlaneWithinPolygon))
            {
                UpdateDistance();
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
    }


}


