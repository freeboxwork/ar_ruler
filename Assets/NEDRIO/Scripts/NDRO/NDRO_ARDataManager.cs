using UnityEngine;
using System.Collections.Generic;


namespace NDRO.Ruler
{

    public class NDRO_ARDataManager : MonoBehaviour
    {
        NDRO_TapeRulerSaveData tapeRulerSaveData;

        void Start()
        {

        }


        public void SaveTapeRulerData(List<NDRO_RulerPoints> rulerPoints, string dataName, string customorCode)
        {
            tapeRulerSaveData = new NDRO_TapeRulerSaveData();
            tapeRulerSaveData.dataName = dataName;
            tapeRulerSaveData.customorCode = customorCode;

            //TODO: 서버에서 받아온 날짜로 변경
            tapeRulerSaveData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            for (int i = 0; i < rulerPoints.Count; i++)
            {
                NDRO_RulerData rulerData = new NDRO_RulerData();
                rulerData.pointA = rulerPoints[i].pointA.position;
                rulerData.pointB = rulerPoints[i].pointB.position;
                rulerData.distance = rulerPoints[i].distance;
                tapeRulerSaveData.rulerDatas.Add(rulerData);
            }

            string json = JsonUtility.ToJson(tapeRulerSaveData);

            //TODO: 서버에 데이터 전송
        }
    }



    // 줄자 모드 데이터
    [System.Serializable]
    public class NDRO_TapeRulerSaveData
    {
        public List<NDRO_RulerData> rulerDatas = new List<NDRO_RulerData>();
        public string dataName;
        public string date;
        public string dataType = "TapeRuler";
        public string customorCode;
    }

    [System.Serializable]
    public class NDRO_RulerData
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public float distance;
    }


}







